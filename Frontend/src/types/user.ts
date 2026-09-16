import type { UserRole } from './enums'

/**
 * Mirrors CareConnect.DTOs.Users.UserDto — the account rows the Admin user-management screen lists.
 *
 * `caregiverId`/`clientId` say whether the account actually has the profile its role implies. A
 * user can hold Role=Caregiver with no caregiver profile, because the backend requires the role to
 * be set *before* that profile can be created.
 */
export interface User {
  id: number
  email: string
  fullName: string
  phoneNumber: string | null
  role: UserRole
  isActive: boolean
  /** Whether a password is set at all — never the hash, which the API does not expose. */
  hasPassword: boolean
  caregiverId: number | null
  clientId: number | null
  createdAtUtc: string
}

/** Mirrors CareConnect.DTOs.Users.UpdateUserRoleDto. */
export interface UpdateUserRolePayload {
  role: UserRole
}

/** Mirrors CareConnect.DTOs.Users.ResetUserPasswordDto — an Admin setting someone else's password. */
export interface ResetUserPasswordPayload {
  newPassword: string
}
