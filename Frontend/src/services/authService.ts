import { http } from './api'
import type { ChangePasswordRequest, LoginRequest, LoginResponse } from '@/types/auth'

/**
 * POST /api/auth/login does not exist on the backend yet — see the doc comment on
 * types/auth.ts for the exact contract this is written against. This call 404s until that
 * endpoint lands there; nothing here needs to change once it does.
 *
 * This is the ONLY place a credential ever leaves the browser, and the only place a token ever
 * enters it. Whether `payload.email`/`payload.password` are correct is decided entirely by the
 * backend's response (200 + a token, or a 4xx caught by the centralized error handling in
 * services/api.ts) — nothing here inspects, validates, or second-guesses that decision.
 */
export async function login(payload: LoginRequest): Promise<LoginResponse> {
  const { data } = await http.post<LoginResponse>('/auth/login', payload)
  return data
}

/**
 * Changes the signed-in user's own password. A wrong current password comes back as a 400 with a
 * field-level error — deliberately not a 401, which services/api.ts would treat as an expired
 * session and sign the user out mid-form.
 *
 * The session survives the change: this API has no token revocation, so the token in hand stays
 * valid until it expires.
 */
export async function changePassword(payload: ChangePasswordRequest): Promise<void> {
  await http.post('/auth/change-password', payload)
}

/** The API issues stateless JWTs with no server-side session to revoke — there's nothing to call. */
export async function logout(): Promise<void> {}

export const authService = { login, changePassword, logout }
