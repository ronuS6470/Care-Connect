# New Project Reference (Based on EMS)

This reference captures the same architecture and standards used in this repository so you can quickly start a new backend project with consistent structure.

## 1) Recommended Solution Structure

Use one solution with separate projects by responsibility:

```text
<ProjectName>.sln
│
├─ <ProjectName>.Controller/         # Presentation/API layer
│  ├─ Controllers/
│  ├─ Middleware/
│  ├─ Properties/
│  ├─ Program.cs
│  ├─ appsettings.json
│  ├─ appsettings.Development.json
│  └─ <ProjectName>.Controller.csproj
│
├─ <ProjectName>.AppServices/        # Application orchestration layer
│  ├─ Mapping/
│  ├─ <Feature>Service.cs
│  └─ <ProjectName>.AppServices.csproj
│
├─ <ProjectName>.Commands/           # CQRS write side
│  ├─ <Feature>/
│  │  ├─ Add<Feature>Command.cs
│  │  ├─ Add<Feature>CommandHandler.cs
│  │  ├─ Update<Feature>Command.cs
│  │  ├─ Update<Feature>CommandHandler.cs
│  │  ├─ Delete<Feature>Command.cs
│  │  └─ Delete<Feature>CommandHandler.cs
│  ├─ ICommandHandler.cs
│  └─ <ProjectName>.Commands.csproj
│
├─ <ProjectName>.Queries/            # CQRS read side
│  ├─ <Feature>/
│  │  ├─ GetAll<Feature>sQuery.cs
│  │  ├─ GetAll<Feature>sQueryHandler.cs
│  │  ├─ Get<Feature>ByIdQuery.cs
│  │  └─ Get<Feature>ByIdQueryHandler.cs
│  ├─ Repositories/
│  │  ├─ <Feature>DapperRepository.cs
│  │  └─ *.sql                       # Embedded SQL
│  ├─ IQueryHandler.cs
│  ├─ I<Feature>ReadRepository.cs
│  └─ <ProjectName>.Queries.csproj
│
├─ <ProjectName>.Infrastructure/     # Data access for write side
│  ├─ Models/
│  ├─ Mapping/
│  ├─ Repositories/
│  │  ├─ I<Feature>Repository.cs
│  │  └─ <Feature>Repository.cs
│  ├─ AppDbContext.cs
│  └─ <ProjectName>.Infrastructure.csproj
│
└─ <ProjectName>.DTO/                # API contracts
   ├─ <Feature>Dto.cs
   └─ <ProjectName>.DTO.csproj
```

---

## 2) Layer Responsibilities (Standard)

### Controller Layer (`.Controller`)
- Accept/validate HTTP input.
- Return HTTP responses and status codes.
- Do not write business logic here.
- Use centralized global exception handling middleware.

### AppServices Layer (`.AppServices`)
- Orchestrate command/query handlers.
- Map DTOs to commands via AutoMapper.
- Put business validations that are application-level here.

### Commands Layer (`.Commands`)
- Write-only operations (create/update/delete).
- One command + one handler per action.
- Handlers call write repositories (`.Infrastructure`).

### Queries Layer (`.Queries`)
- Read-only operations.
- Prefer Dapper for query performance and SQL control.
- Keep SQL in `.sql` files as embedded resources.

### Infrastructure Layer (`.Infrastructure`)
- EF Core `DbContext`, entities/models, and write repositories.
- Persist and mutate data with explicit errors when entities are missing.

### DTO Layer (`.DTO`)
- API request/response contracts only.
- Keep DTOs independent from EF entities.

---

## 3) Naming Standards

- Solution: `<ProjectName>.sln`
- Project names: `<ProjectName>.Controller`, `<ProjectName>.AppServices`, etc.
- Namespace root: `ProjectName.<Layer>[.<Feature>]`
- Files:
  - `XxxCommand.cs`, `XxxCommandHandler.cs`
  - `XxxQuery.cs`, `XxxQueryHandler.cs`
  - `IXxxRepository.cs`, `XxxRepository.cs`
  - `XxxMappingProfile.cs`

---

## 4) Dependency Rules (Keep Clean Architecture Boundaries)

- `.Controller` references: `.AppServices`, `.Commands`, `.Queries`, `.Infrastructure`, `.DTO`
- `.AppServices` references: `.Commands`, `.Queries`, `.Infrastructure`, `.DTO`
- `.Commands` references: `.Infrastructure`, `.DTO`
- `.Queries` references: `.DTO`
- `.Infrastructure` references: `.DTO` (if needed)
- `.DTO` references: none

Do not create circular project references.

---

## 5) Coding Standards

- Enable nullable reference types and implicit usings in all `.csproj`.
- Register dependencies through DI in `Program.cs`.
- Keep handlers small and single-purpose.
- Throw explicit exceptions (`KeyNotFoundException`, `ArgumentException`, etc.) instead of silent failures.
- Use `AsNoTracking()` for read-only EF queries.
- Keep mapping configuration centralized under `.AppServices/Mapping`.
- Keep SQL file names aligned with repository method intent (for example `GetAllEmployees.sql`).

---

## 6) Configuration Standards

- Keep connection string under `ConnectionStrings:DefaultConnection`.
- Keep environment-specific overrides in `appsettings.Development.json`.
- Add a global exception handler using `IExceptionHandler` and `ProblemDetails`.

---

## 7) Quick Start Checklist for a New Project

1. Create the 6 projects shown above and add them to one solution.
2. Set all target frameworks consistently (for example `net9.0`).
3. Add project references following the dependency rules.
4. Add DI registrations for handlers/repositories/mappers in `Program.cs`.
5. Add one feature end-to-end (DTO -> Controller -> Service -> Command/Query -> Repository).
6. Keep read and write responsibilities separated (CQRS).

---

## 8) Optional Future Enhancements

- Add test projects:
  - `<ProjectName>.UnitTests`
  - `<ProjectName>.IntegrationTests`
- Add FluentValidation for request validation.
- Add API versioning and OpenAPI configuration.
