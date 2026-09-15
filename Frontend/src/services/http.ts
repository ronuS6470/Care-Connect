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

  if (auth.token) {
    config.headers.Authorization = `Bearer ${auth.token}`
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

      // Only a *session* becoming invalid (expired/revoked token on an authenticated request)
      // triggers a forced logout + redirect. A 401 on the login/register call itself — bad
      // credentials — leaves auth.token untouched and is handled by the caller as a normal form
      // error, never a navigation.
      if (auth.isAuthenticated) {
        void auth.logout()
      }
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
