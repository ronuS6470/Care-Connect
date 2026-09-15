import { http } from './api'
import type { AuthResponse, LoginRequest, RegisterRequest } from '@/types/auth'

/**
 * Matches CareConnect.DTOs.Auth.{LoginRequestDto,RegisterRequestDto,LoginResponseDto} exactly.
 * No AuthController exists on the backend yet (LoginResponseDto is defined but unused) — these
 * calls 404 until that endpoint is added there. The frontend is written against the real,
 * already-defined contract on purpose, so nothing here needs to change once it lands.
 */

export async function login(payload: LoginRequest): Promise<AuthResponse> {
  const { data } = await http.post<AuthResponse>('/auth/login', payload)
  return data
}

export async function register(payload: RegisterRequest): Promise<AuthResponse> {
  const { data } = await http.post<AuthResponse>('/auth/register', payload)
  return data
}

export async function logout(): Promise<void> {
  // The API issues stateless JWTs with no server-side session to revoke, so there's nothing to
  // call — clearing local state (done by the auth store) is the entire logout operation.
}

export const authService = { login, register, logout }
