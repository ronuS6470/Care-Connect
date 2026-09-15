import type { UserRole } from '@/types/enums'

/**
 * The single place this app persists an authenticated session. Nothing else — no store, no
 * service, no component — touches localStorage/sessionStorage for auth; they all go through the
 * `authStorage` object below, via the auth store.
 *
 * Why localStorage: the API issues bearer JWTs with no httpOnly-cookie session and no refresh
 * endpoint, so "refresh doesn't sign you out" leaves a SPA nowhere else to keep the token. That
 * is a real XSS exposure tradeoff, not a best practice — it's what a bearer-token-only backend
 * forces, and it's why the token lives in exactly one mechanism (never mirrored into
 * sessionStorage, a cookie, or a module-level variable).
 *
 * If the backend later issues httpOnly secure cookies, this file is the only one that changes:
 * drop `token` from StoredSession (the browser holds it and JS can no longer read it), and in
 * services/api.ts set `withCredentials: true` and delete the Authorization-header line. Every
 * service stays exactly as written, because none of them ever handled a token themselves.
 */

const STORAGE_KEY = 'careconnect.auth'

/**
 * The minimum needed to restore the UI session: the bearer token, the identity fields the chrome
 * renders (name/email/role), and the expiry the store checks on restore. No password, no password
 * hash, no anything else from the login response.
 */
export interface StoredSession {
  token: string
  userId: number
  fullName: string
  email: string
  role: UserRole
  expiresAtUtc: string
}

/**
 * localStorage is user-writable, so a stored entry is untrusted input: anything absent, malformed,
 * or not matching the expected shape reads back as `null` rather than as a half-populated session.
 */
function isStoredSession(value: unknown): value is StoredSession {
  if (typeof value !== 'object' || value === null) return false

  const candidate = value as Record<string, unknown>
  return (
    typeof candidate.token === 'string' &&
    typeof candidate.userId === 'number' &&
    typeof candidate.fullName === 'string' &&
    typeof candidate.email === 'string' &&
    typeof candidate.role === 'number' &&
    typeof candidate.expiresAtUtc === 'string'
  )
}

export const authStorage = {
  /** The persisted session, or `null` if there isn't a usable one. Never throws. */
  read(): StoredSession | null {
    try {
      const raw = localStorage.getItem(STORAGE_KEY)
      if (!raw) return null

      const parsed: unknown = JSON.parse(raw)
      return isStoredSession(parsed) ? parsed : null
    } catch {
      // Unparsable JSON, or storage unavailable entirely (private mode, storage disabled).
      return null
    }
  },

  /** Persists the session. A storage failure (quota, private mode) leaves the in-memory session working for this tab. */
  save(session: StoredSession): void {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(session))
    } catch {
      // Intentionally ignored — failing to persist must not fail the sign-in itself.
    }
  },

  clear(): void {
    try {
      localStorage.removeItem(STORAGE_KEY)
    } catch {
      // Nothing to do: if storage is unreachable there is nothing persisted to clear.
    }
  },
}
