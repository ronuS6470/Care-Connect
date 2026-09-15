import type { UserRole } from './enums'

/** Mirrors CareConnect.DTOs.Auth.LoginRequestDto. */
export interface LoginRequest {
  email: string
  password: string
}

/** Mirrors CareConnect.DTOs.Auth.RegisterRequestDto. */
export interface RegisterRequest {
  email: string
  password: string
  firstName: string
  lastName: string
  phoneNumber?: string | null
  role: UserRole
}

/** Mirrors CareConnect.DTOs.Auth.LoginResponseDto — the shape both /auth/login and /auth/register return. */
export interface AuthResponse {
  token: string
  userId: number
  fullName: string
  email: string
  role: UserRole
  expiresAtUtc: string
}
