/**
 * The authentication contract for POST /api/auth/login. This endpoint does not exist on the
 * backend yet (no AuthController — LoginResponseDto sits unused in CareConnect.DTOs), so these
 * types are written against the response shape the backend is expected to return, exactly as
 * specified, so nothing here needs to change once it lands: `{ token, userId, fullName, email,
 * role }` with `role` as one of the three role names below (a string, e.g. "Admin") — not the
 * numeric value CareConnect.DTOs.Enums.UserRole serializes to today by default. Getting a real
 * backend response into this shape requires either a JsonStringEnumConverter on that enum or a
 * dedicated login DTO; that's a backend decision, out of scope here.
 *
 * The rest of this app (route guards, nav, badges) is already built on the numeric UserRole enum
 * in `types/enums.ts`. Rather than cascade that rename through the whole app, `stores/auth.ts`
 * translates a LoginResponse's string `role` into that internal numeric enum once, at the one
 * point a login response enters the app — everything downstream of the store is untouched.
 */

/** The three roles the backend issues. Nothing else is ever accepted. */
export type UserRole = 'Admin' | 'Caregiver' | 'Client'

export interface LoginRequest {
  email: string
  password: string
}

/**
 * The raw response body from POST /api/auth/login, exactly as the backend sends it — this type
 * exists to describe that wire shape, not the app's internal user state (see AuthenticatedUser).
 */
export interface LoginResponse {
  token: string
  userId: number
  fullName: string
  email: string
  role: UserRole
  expiresAtUtc: string
}

/**
 * The signed-in user's identity, as the rest of the app reasons about it — a LoginResponse minus
 * the bearer token itself (callers that need the token read it from the auth store directly,
 * never pass it around as part of a "user").
 */
export interface AuthenticatedUser {
  userId: number
  fullName: string
  email: string
  role: UserRole
}
