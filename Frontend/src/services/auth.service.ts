import { UserRole } from '@/types/enums'
import type { AuthSession, LoginPayload } from '@/types/auth'

/**
 * Fixed Auth0UserId values seeded against real Users/Caregivers/Clients rows in the local
 * database (see CareConnect.Infrastructure.Auth.DevelopmentAuthenticationHandler and this
 * project's seed data) — picking a role here signs in as that already-provisioned identity.
 * Replace this whole module with real Auth0 SPA/redirect calls once that's wired up; nothing
 * outside services/auth.service.ts and stores/auth.ts needs to change.
 */
const DEV_SUBJECT_BY_ROLE: Record<UserRole, string> = {
  [UserRole.Admin]: 'dev-admin',
  [UserRole.Caregiver]: 'dev-caregiver-1',
  [UserRole.Client]: 'dev-client-1',
}

export async function login(payload: LoginPayload): Promise<AuthSession> {
  return {
    subject: DEV_SUBJECT_BY_ROLE[payload.role],
    role: payload.role,
    displayName: payload.email.split('@')[0] || 'User',
    email: payload.email,
  }
}

export async function logout(): Promise<void> {
  // No server-side session to invalidate under the dev bypass. Real Auth0 logout (clearing the
  // IdP session too) will live here once that flow replaces this module.
}
