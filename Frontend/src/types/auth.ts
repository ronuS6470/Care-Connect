import type { UserRole } from './enums'

/**
 * The API has no password-based login endpoint (LoginRequestDto exists in the backend but is
 * unused — auth is Auth0 JWT bearer only, see CareConnect.Infrastructure's JwtBearer setup).
 * Until the real Auth0 SPA flow is wired in, the frontend authenticates against the backend's
 * local-testing bypass (X-Dev-Sub / X-Dev-Role headers, gated server-side to Development +
 * Auth0:BypassForLocalTesting) via this session shape. Swapping in real Auth0 later only touches
 * services/auth.service.ts and this file — everything downstream just consumes AuthSession.
 */
export interface AuthSession {
  /** Sent as X-Dev-Sub today; will become the Auth0 `sub` claim once real login is wired in. */
  subject: string
  role: UserRole
  displayName: string
  email: string
}

export interface LoginPayload {
  email: string
  role: UserRole
}
