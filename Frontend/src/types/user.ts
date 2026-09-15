import type { UserRole } from './enums'

/**
 * There is no UsersController/UserDto exposed anywhere in the API (confirmed while building the
 * Caregiver/Client CRUD — GetCaregivers/GetClients return their own profile DTOs, never a raw
 * User). This models the underlying CareConnect.Infrastructure.Entities.User shape as CareConnect
 * DTOs reference it (LoginResponseDto's UserId/FullName/Email/Role, CaregiverDto/ClientDto's
 * userId) — useful for typing that concept consistently, even with no direct "get a User" call.
 */
export interface User {
  id: number
  fullName: string
  email: string
  phoneNumber: string | null
  role: UserRole
  isActive: boolean
}
