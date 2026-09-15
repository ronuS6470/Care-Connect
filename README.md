# CareConnect

Non-medical home-care management. Agency admins schedule visits, caregivers execute them, and
clients follow along.

The repository holds two independently-runnable applications:

| | Stack | Port (dev) |
|---|---|---|
| [`Backend/`](Backend) | ASP.NET Core 10 Web API, SQL Server | `5035` |
| [`Frontend/`](Frontend) | Vue 3 + TypeScript + Vite + Tailwind 4 | `5173` |

---

## Table of contents

- [Getting started](#getting-started)
- [Authentication](#authentication)
- [Backend architecture](#backend-architecture)
- [Frontend architecture](#frontend-architecture)
- [API surface](#api-surface)
- [Conventions worth knowing](#conventions-worth-knowing)
- [Troubleshooting](#troubleshooting)

---

## Getting started

### Prerequisites

- **.NET SDK 10**
- **SQL Server LocalDB** (ships with the SQL Server Express / Visual Studio installers)
- **Node.js 20+**
- **`dotnet-ef`** tool (`dotnet tool install --global dotnet-ef`)
- `sqlcmd` on `PATH` for the database script

### 1. Database

The schema is managed by **EF Core migrations**, in
[`CareConnect.Infrastructure/Persistence/Migrations/`](Backend/src/CareConnect/CareConnect.Infrastructure/Persistence/Migrations).
Create or update your local database with:

```bash
cd Backend/src/CareConnect
dotnet ef database update --project CareConnect.Infrastructure --startup-project CareConnect.Controller
```

Then apply the checked-in SQL under [`Backend/db/`](Backend/db), in filename order:

```bash
cd Backend/db
sqlcmd -S "(localdb)\mssqllocaldb" -d CareConnectDb -i 001-add-password-auth.sql
```

Each script is written to be **idempotent** — safe to re-run.

> **Known gap:** `001-add-password-auth.sql` currently adds the `Users.PasswordHash` column itself,
> outside EF — there is no migration for it, and the model snapshot doesn't know it exists. Until
> that's moved into a proper migration, **always run the script after `database update`** (login
> fails with *Invalid column name 'PasswordHash'* otherwise), and expect the next
> `dotnet ef migrations add` to try to add the column again.
>
> The script also backfills a development password onto every existing user. It is seed data, not
> production-safe. See [Seeded accounts](#seeded-accounts).

### 2. Backend

```bash
cd Backend/src/CareConnect/CareConnect.Controller
dotnet run --launch-profile http
```

Serves on `http://localhost:5035`, with Swagger at `/swagger` and a health probe at `/health`.

### 3. Frontend

```bash
cd Frontend
npm install
npm run dev
```

Serves on `http://localhost:5173`. The Vite dev server proxies `/api` → `http://localhost:5035`
(see [`vite.config.ts`](Frontend/vite.config.ts)), so the browser talks to the API same-origin and
the backend needs no CORS policy in development.

| Command | Does |
|---|---|
| `npm run dev` | Dev server with HMR |
| `npm run build` | Type-check (`vue-tsc`) then production build |
| `npm run type-check` | Type-check only |
| `npm run preview` | Serve the production build locally |

Configuration lives in [`Frontend/.env`](Frontend/.env):

```ini
VITE_API_BASE_URL=/api
```

Relative in development so the proxy handles it; point it at the real API host for a production
build. No production URL is hardcoded anywhere.

---

## Authentication

The API issues and validates **its own JWTs**. It does not delegate to an external identity
provider. (It previously validated Auth0 tokens; the `Auth0UserId` column survives that history as
the stable subject identifier — see the note below.)

### The flow

```
LoginPage.vue
  └─ authStore.login()                      Pinia — stores/auth.ts
       └─ authService.login()               services/authService.ts
            └─ POST /api/auth/login         ← the only call that sends a credential
                 └─ AuthController
                      └─ IAuthAppService
                           └─ LoginCommand  (MediatR)
                                └─ LoginCommandHandler
                                     ├─ IUserRepository.GetByEmailAsync
                                     ├─ IPasswordHasher.Verify   (PBKDF2)
                                     └─ IJwtTokenGenerator.Generate
```

**Request**

```jsonc
POST /api/auth/login
{ "email": "...", "password": "..." }
```

**Response** — `200`

```jsonc
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "userId": 3,
  "fullName": "Alex Admin",
  "email": "admin@...",
  "role": "Admin",                       // string, not the underlying enum int
  "expiresAtUtc": "2026-09-15T18:03:02Z"
}
```

Any credential failure returns `401` with a single indistinguishable message.

### Two claims are load-bearing

Do not rename these casually — they are what the rest of the API is built on.

| Claim | Value | Why it matters |
|---|---|---|
| `sub` | `User.Auth0UserId` | Every handler resolves the caller via `ICurrentUserAccessor`, which reads this claim and looks the user up by that column. A different `sub` breaks every authenticated endpoint. |
| `role` | `"Admin"` / `"Caregiver"` / `"Client"` | What `[Authorize(Roles = "...")]` checks, via `RoleClaimType = "role"` configured next to token validation. |

### Password storage

PBKDF2-HMAC-SHA256 — 210,000 iterations, 16-byte per-hash salt, 32-byte digest, constant-time
comparison. Stored as `v1.{iterations}.{base64Salt}.{base64Hash}`.

The iteration count travels **with each hash** rather than being read from a constant at verify
time, so it can be raised later without invalidating passwords already stored at a lower count.

`Users.PasswordHash` is nullable: accounts provisioned before local sign-in existed have none and
simply cannot sign in with a password.

### Not an account-enumeration oracle

Wrong password, unknown email, deactivated account, and no-password-set all return the **same**
message. The unknown-email path deliberately burns an equivalent PBKDF2 round before refusing, so
response timing doesn't reveal which emails are registered.

### Configuration

```jsonc
// appsettings.json — non-secret
"Jwt": {
  "Issuer": "CareConnect.Api",
  "Audience": "CareConnect.Client",
  "AccessTokenMinutes": 60
}
```

`Jwt:SigningKey` is a **secret**. A development-only key sits in `appsettings.Development.json`.
Any real environment must supply it out-of-band:

```bash
dotnet user-secrets set "Jwt:SigningKey" "<32+ byte random string>"
# or
export Jwt__SigningKey="<32+ byte random string>"
```

Startup fails loudly if the key is shorter than 32 bytes (HS256 needs ≥256 bits).

#### Local testing bypass

`Jwt:BypassForLocalTesting` (Development only, default **off**) swaps real token validation for a
handler that auto-authenticates every request, with the caller chosen by headers:

```bash
curl http://localhost:5035/api/visits -H "X-Dev-Sub: dev-admin" -H "X-Dev-Role: Admin"
```

Useful for Postman against seeded data. It is double-gated — it needs both a Development
environment *and* the explicit flag — and while it is on, real login tokens are ignored entirely.

### Seeded accounts

All seeded users share one development password: **`Passw0rd!`**

| Role | Email | Lands on |
|---|---|---|
| Admin | `admin-20260915063354@careconnect.local` | `/admin/dashboard` |
| Caregiver | `james.wilson.cg1-20260915063354@careconnect.local` | `/caregiver/dashboard` |
| Client | `dorothy.miller.cl1-20260915063354@careconnect.local` | `/client/dashboard` |

> The email suffix is a timestamp from when the data was seeded; yours will differ. List them with:
> `sqlcmd -S "(localdb)\mssqllocaldb" -d CareConnectDb -Q "SELECT Id, Email, Role FROM Users"`

### The frontend side of the session

- **One Axios instance** ([`services/api.ts`](Frontend/src/services/api.ts)) owns `baseURL`, the
  JSON content type, and both interceptors. No service ever sets an `Authorization` header itself.
- **Request interceptor** attaches `Bearer <token>` from the Pinia store to every request.
- **Response interceptor** — `401` clears the session and redirects to `/login`, preserving the
  intended route as a `?redirect=` param; `403` routes to `/unauthorized` **without** logging the
  user out, since the token is still valid for everything else.
- **One storage abstraction** ([`utils/authStorage.ts`](Frontend/src/utils/authStorage.ts)) is the
  only place auth touches `localStorage`. The token lives in exactly one mechanism — never mirrored
  into `sessionStorage`, a cookie, or a module variable. It is never logged, never put in a URL,
  and never rendered.
- **`initializeAuth()`** runs once in [`main.ts`](Frontend/src/main.ts) before the router's first
  navigation, so the first guard check already knows the real auth state.

> **If the backend later issues httpOnly cookies**, only `authStorage.ts` changes (drop `token`
> from `StoredSession`), plus two lines in `api.ts` (`withCredentials: true`, delete the
> Authorization header). No service file changes, because no service ever handled a token.

### Frontend guards are UX, not security

Route guards and hidden nav links stop a user landing on a screen with nothing to show them. They
run entirely in the browser, on state the user controls. **Every endpoint enforces auth, role, and
row-level ownership server-side independently** — `[Authorize(Roles = ...)]` plus per-row requester
checks. Treat the client-side checks as convenience only.

---

## Backend architecture

Layered, with a one-way dependency graph:

```
Controller  →  AppServices  →  Commands   →  Infrastructure  →  DTOs
                           ↘   Queries    ↗
```

| Project | Responsibility |
|---|---|
| `CareConnect.Controller` | HTTP surface, middleware, DI composition root |
| `CareConnect.AppServices` | Orchestration — one method per controller action, dispatching via MediatR |
| `CareConnect.Commands` | Writes. MediatR handlers, FluentValidation validators, AutoMapper profiles |
| `CareConnect.Queries` | Reads. One folder per query call: its query, handler, `.sql`, and result types |
| `CareConnect.Infrastructure` | EF Core `DbContext`, entities, write repositories, auth primitives |
| `CareConnect.DTOs` | Wire contracts and enums. Depends on nothing |

Tests: `CareConnect.{AppServices,Commands,Queries}.Tests` and `CareConnect.IntegrationTests`.

**Reads and writes are split.** Writes go through EF Core repositories with change tracking. Reads
go through Dapper with hand-written SQL kept as embedded `.sql` resources, so read projections never
drag entity graphs around. Both share one connection string, so they can never drift onto different
databases.

### Queries: one folder per call

Every read is a self-contained folder. The handler runs its own SQL — there is no read-side
repository layer:

```
CareConnect.Queries/
  Clients/
    GetClients/
      GetClientsQuery.cs            MediatR request record
      GetClientsQueryHandler.cs     runs the SQL below via IDbConnectionFactory
      GetClientsQuery.sql
      GetClientsCountQuery.sql      paged calls carry a count query beside the data query
    GetClientById/
      GetClientByIdQuery.cs
      GetClientByIdQueryHandler.cs
      GetClientByIdQuery.sql
      CaregiverHasClientAssignmentQuery.sql
  Visits/
    VisitsSharedSql.cs              SQL used by more than one Visits call…
    GetVisitTasksByVisitIdQuery.sql …lives at the feature root
    GetVisitById/
      …
      GetVisitByIdQueryResult.cs    internal row type, when a call needs one
  Security/                         RequesterResolver, VisitAccessResolver + their SQL
```

The rules:

- **A file used by one call lives in that call's folder. A file shared by several lives at the
  feature root** — e.g. `VisitsSharedSql`, `ReportingSharedSql`, `Reports/ReportAccess.cs`.
- **Results are the existing DTOs.** A `<Call>QueryResult.cs` exists only when the SQL's row shape
  differs from the DTO — typically extra columns an ownership check needs before mapping.
- **Every `.sql` file is named `<Name>Query.sql`.**
- **Namespace must match folder path.** `SqlResourceLoader.Load(typeof(Handler), "X.sql")` finds a
  file by its anchor type's namespace, so moving a file without its handler — or renaming a
  namespace without moving the folder — breaks loading.

Because SQL loads in static initializers, a bad path compiles fine and would only fail on the first
request. `SqlResourceLoadingTests` (in `CareConnect.Queries.Tests`) runs every static initializer
and checks every embedded `.sql` sits in a folder that has code, so these mistakes fail `dotnet test`
instead of a live request.

**Multi-statement batches are read by position.** The dashboards and the availability-conflicts
report send several `SELECT`s in one round trip and read the result sets in order. Each of those
`.sql` files says so in its header — reordering a `SELECT` means updating the handler.

**Row-scoped list queries branch on a role parameter.** `GetClients`, `GetAssignments`, `GetVisits`,
and `GetUpcomingVisits` serve all three roles from one static statement:
`@RequesterRole = 1 OR (@RequesterRole = 3 AND …) OR (@RequesterRole = 2 AND …)`. Those statements
carry `OPTION (RECOMPILE)`, so a plan cached for one role is never reused for another.

> This layout deliberately departs from `.claude/NEW-PROJECT-REFERENCE.md`, which describes shared
> `Queries/Repositories/<Feature>DapperRepository` classes. The per-call layout keeps everything a
> read needs in one place.

**Known index opportunity.** The hours, earnings, and duration reports all filter on
`Visits.ActualStartUtc`, which has no index. Once visit volume warrants it:
`CREATE INDEX IX_Visits_ActualStartUtc ON Visits(ActualStartUtc) WHERE ActualStartUtc IS NOT NULL`.

### Error handling

`GlobalExceptionHandler` maps exceptions to status codes, and every error response uses the same
`ApiResponse` envelope:

| Exception | Status |
|---|---|
| FluentValidation `ValidationException` | `400` |
| `InvalidCredentialsException` | `401` |
| `ForbiddenException` | `403` |
| `NotFoundException` | `404` |
| `ConflictException`, `BusinessRuleException` | `409` |
| anything else | `500` (details logged, never returned) |

Two paths bypass the handler and are wired separately to keep the envelope consistent: model-binding
failures (via `InvalidModelStateResponseFactory`) and the auth middleware's own `401`/`403`
short-circuits (via `JwtBearerEvents`).

**Success responses are unwrapped** — a `200` returns the DTO directly, not an envelope. Only errors
are wrapped.

### The visit scheduling lock

`IVisitRepository.CreateScheduledVisitAsync` owns its transaction, a per-caregiver
`sp_getapplock`, the scheduling checks, and the insert as **one atomic method**. The lock, the
checks, and the write must share a single connection and transaction or `sp_getapplock` stops
serializing and the double-booking race reopens.

This is the one deliberate exception to the "one `SaveChangesAsync` per handler" convention used
everywhere else. Do not pull the transaction up into the handler to make it match.

---

## Frontend architecture

```
src/
  components/   common/ (design system) + feature folders
  composables/  reusable data-fetching & state logic
  layouts/      Admin / Caregiver / Client shells
  pages/        route targets, grouped by role
  router/       routes, guards, nav definitions
  services/     one Axios instance + typed API modules
  stores/       Pinia — auth, ui, theme, toast, confirm
  types/        TypeScript models mirroring backend DTOs
  utils/        date, currency, status, authStorage
  validation/   dependency-free form rules
```

**Strict conventions:**

- `<script setup lang="ts">` and the Composition API throughout. No Options API anywhere.
- Components never call Axios directly — they call a service or a composable.
- `strict`, `noUnusedLocals`, and `noUnusedParameters` are all on. There is **no `any`** in the
  codebase.
- Styling is Tailwind utilities plus a small set of `@layer components` primitives
  (`.btn-base`, `.card-base`, `.input-base`, …) in
  [`assets/main.css`](Frontend/src/assets/main.css). There are **no `<style>` blocks** in any
  `.vue` file.

### State: Pinia vs. composables

- **Pinia** — global application state: `auth`, `ui` (sidebar), `theme`, `toast`, `confirm`.
- **Composables** — reusable per-feature data logic: `useCaregivers`, `useClients`, `useVisits`,
  `useAssignments`, `useCareTasks`, `useDashboard`, `useEarnings`. Each exposes `data`, `loading`,
  `error`, and actions.

Two internal helpers back most list screens, because the backend supports filtering on some
resources but not others:

| Helper | Strategy | Used by |
|---|---|---|
| `useServerPagedList` | Real server-side filter + page; refetches on change | Clients, Visits |
| `useClientFilteredList` | Fetch once, filter and paginate in memory | Caregivers, Assignments, Care Tasks |

Page-specific logic (modal orchestration, form submit flows) deliberately stays in the page.

### Enums cross the wire as integers

The API registers no global `JsonStringEnumConverter`, so every enum serializes as its underlying
`int`. [`types/enums.ts`](Frontend/src/types/enums.ts) mirrors them value-for-value, and the
`*_LABELS` maps are the only place they become display text.

**The one exception is the login response's `role`**, which is annotated to serialize as a string
because the authentication contract is defined in role names. `stores/auth.ts` translates that
string into the internal numeric enum once, on the way in — so nothing downstream needs to know.

---

## API surface

All routes are under `/api`. Everything except `POST /api/auth/login` requires a bearer token.

| Controller | Route | Notes |
|---|---|---|
| `AuthController` | `/api/auth` | `login`. Anonymous |
| `CaregiversController` | `/api/caregivers` | CRUD, activate/deactivate |
| `CaregiverAvailabilityController` | `/api/caregiver-availability` | Weekly windows |
| `ClientsController` | `/api/clients` | CRUD. Supports `search` |
| `AssignmentsController` | `/api/assignments` | Caregiver↔client pairing |
| `CareTasksController` | `/api/care-tasks` | Task templates |
| `VisitsController` | `/api/visits` | Scheduling, check-in/out, complete, visit tasks |
| `VisitNotesController` | `/api/visit-notes` | Notes. Admin/Caregiver write, Client read |
| `DashboardController` | `/api/dashboard` | Per-role summaries |
| `ReportsController` | `/api/reports` | ~13 admin-only operational reports |
| `ReportingController` | `/api/reporting` | Caregiver earnings/hours, self-service |

Read endpoints are **row-scoped server-side** by the caller's identity: a caregiver requesting
`/api/visits` receives only their own, with no client-supplied filter involved.

---

## Conventions worth knowing

- **Frontend validation is UX only.** Required fields, formats, `end > start`. Complex business
  rules — double-booking, availability, assignment validity, visit state transitions, required task
  completion — are **not** re-implemented client-side. The backend decides; the client displays what
  it says.
- **404 on a detail fetch is not an error.** `GET /api/{resource}/{id}` returns a bare framework
  `404` with no envelope. The frontend's `fetchOrNull` turns exactly that case into `null`, rendering
  an empty state rather than an error banner.
- **A 409 on visit creation is a scheduling conflict** and is surfaced with its own heading. The
  frontend never re-derives that decision.
- Verify frontend changes with `npm run type-check` **and** `npm run build`; verify backend changes
  with `dotnet build` **and** `dotnet test` — a misplaced `.sql` file compiles fine and is only
  caught by the tests.

---

## Troubleshooting

**`dotnet build` fails with MSB3021 / "being used by another process"**
The API is running and holding its own output DLLs. Stop it (including any attached debugger) and
rebuild — it is a file lock, not a compile error.

**Frontend loads but every call 401s**
Expected when signed out. If it persists after signing in, confirm the backend is on `5035` and that
`vite.config.ts`'s proxy target matches — a stale port is the usual cause.

**Every request succeeds even without signing in**
`Jwt:BypassForLocalTesting` is `true`. Set it to `false` in `appsettings.Development.json` to
exercise real tokens.

**Startup throws about `Jwt:SigningKey`**
It is missing or under 32 bytes. Supply it via user-secrets or `Jwt__SigningKey`.

**Login fails with `Invalid column name 'PasswordHash'`**
The database was built from migrations alone. Run `Backend/db/001-add-password-auth.sql` — see the
known gap under [Database](#1-database).

**`Embedded SQL resource '…' was not found`**
A `.sql` file doesn't sit beside the type that loads it, or a namespace no longer matches its folder.
`dotnet test` on `CareConnect.Queries.Tests` names the exact file.

**`Invalid email or password.` for a user you know exists**
That account's `PasswordHash` is probably `NULL` — it predates local sign-in. Re-run
`001-add-password-auth.sql`, which backfills any row still missing one.

**NU1900 / private feed warnings on restore**
The vulnerability-audit lookup can't reach an unrelated internal NuGet feed. Warnings only; restore
still succeeds from nuget.org.
