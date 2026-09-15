import axios, { AxiosError } from 'axios'

import { useAuthStore } from '@/stores/auth'
import type { ApiResponse } from '@/types/common'

/** Normalized shape every failed request throws, built from the backend's ApiResponse envelope. */
export class ApiError extends Error {
  readonly status: number
  readonly errors: string[]

  constructor(message: string, status: number, errors: string[] = []) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.errors = errors
  }
}

export const http = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: 15_000,
})

http.interceptors.request.use((config) => {
  const auth = useAuthStore()

  if (auth.session) {
    // Backend's local-testing bypass (CareConnect.Infrastructure.Auth.DevelopmentAuthenticationHandler).
    // Swapping to real Auth0 later replaces this block with `Authorization: Bearer <token>`.
    config.headers['X-Dev-Sub'] = auth.session.subject
    config.headers['X-Dev-Role'] = auth.session.role
  }

  return config
})

http.interceptors.response.use(
  (response) => response,
  (error: AxiosError<ApiResponse>) => {
    const status = error.response?.status ?? 0
    const body = error.response?.data

    if (status === 401) {
      const auth = useAuthStore()
      auth.clearSession()
    }

    const message = body?.message ?? error.message ?? 'Something went wrong. Please try again.'
    const errors = body?.errors ?? []

    return Promise.reject(new ApiError(message, status, errors))
  },
)

/**
 * Success responses (2xx) return the resource DTO directly, unwrapped — see the doc comment on
 * CareConnect.DTOs.Common.ApiResponse<T>. Only error responses (4xx/5xx, written by
 * GlobalExceptionHandler) use the ApiResponse envelope, which the response interceptor above
 * already turns into ApiError. So a typed call is just:
 *
 *   const { data } = await http.get<CaregiverDto>(`/caregivers/${id}`)
 *
 * with `data` already the DTO — no separate unwrap step needed.
 */
