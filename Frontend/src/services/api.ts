import axios, { AxiosError } from 'axios'

import router from '@/router'
import { useAuthStore } from '@/stores/auth'
import type { ApiResponse } from '@/types/common'

/**
 * The one centralized Axios instance — every service in this folder imports `http` from here
 * rather than calling `axios` directly, so baseURL, auth, and error handling exist in exactly one
 * place. No service ever sets its own Authorization header or duplicates this error handling.
 */

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
  headers: {
    'Content-Type': 'application/json',
  },
})

// Authorization header, attached to every request from the auth store's current token — never
// hardcoded, never logged, never set ad hoc by an individual service call.
http.interceptors.request.use((config) => {
  const auth = useAuthStore()

  if (auth.token) {
    config.headers.Authorization = `Bearer ${auth.token}`
  }

  return config
})

/**
 * The path to send the user back to after logging in again — `null` before the router's first
 * navigation has resolved (`matched` is empty), which avoids ever preserving a bogus "redirect"
 * target from a request that raced app boot.
 */
function currentFullPath(): string | null {
  return router.currentRoute.value.matched.length > 0 ? router.currentRoute.value.fullPath : null
}

// Centralized response handling: every failure, from every service, is normalized into the same
// ApiError shape here — callers never branch on raw Axios/HTTP details themselves.
http.interceptors.response.use(
  (response) => response,
  (error: AxiosError<ApiResponse>) => {
    const status = error.response?.status ?? 0
    const body = error.response?.data

    if (status === 401) {
      const auth = useAuthStore()

      // A *session* becoming invalid (expired/revoked token on an authenticated request) clears
      // the stale state and sends the user back to sign in, preserving where they were headed so
      // they land there again after logging back in. A 401 on the login call itself — bad
      // credentials — leaves auth state untouched; that's a normal form error for the caller
      // (LoginPage) to display, never a forced navigation.
      if (auth.isAuthenticated) {
        auth.clearSession()

        const intendedPath = currentFullPath()
        router.push(
          intendedPath && intendedPath !== '/login'
            ? { path: '/login', query: { redirect: intendedPath } }
            : '/login',
        )
      }
    } else if (status === 403) {
      // A 403 means the session is valid but not permitted here — that's not a reason to log the
      // user out (their token is still good for everything else). Route to /unauthorized instead;
      // avoid pushing again if they're somehow already there.
      if (router.currentRoute.value.path !== '/unauthorized') {
        router.push('/unauthorized')
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

/**
 * Every GetXById action in the API returns a bare framework 404 (`return result is null ?
 * NotFound() : Ok(result);`) rather than throwing NotFoundException — so, unlike every other
 * error path, it never carries the ApiResponse envelope. Detail/Edit pages use this to turn that
 * specific case into `null` (rendered as a "not found" empty state) instead of an error banner;
 * any other failure (network, 500, 403) still rethrows.
 */
export async function fetchOrNull<T>(request: () => Promise<{ data: T }>): Promise<T | null> {
  try {
    const { data } = await request()
    return data
  } catch (err) {
    if (err instanceof ApiError && err.status === 404) return null
    throw err
  }
}
