import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

import router from '@/router'
import * as authService from '@/services/authService'
import type { AuthResponse, LoginRequest, RegisterRequest } from '@/types/auth'
import type { UserRole } from '@/types/enums'

const STORAGE_KEY = 'careconnect.auth'

/**
 * What's persisted across a refresh. The JWT itself has to be here for "refresh doesn't log you
 * out" to work at all — there's no httpOnly-cookie session or refresh-token endpoint on this API,
 * so localStorage (readable by any script on the page) is the only place a SPA can keep it. That
 * is a real XSS exposure tradeoff, not a security best practice; it's what a bearer-JWT-only
 * backend forces. Nothing beyond the token and the display fields the UI needs is stored — no
 * password ever passes through here.
 */
interface StoredAuth {
  token: string
  userId: number
  fullName: string
  email: string
  role: UserRole
  expiresAtUtc: string
}

function readStoredAuth(): StoredAuth | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (!raw) return null

    const parsed = JSON.parse(raw) as StoredAuth
    if (new Date(parsed.expiresAtUtc).getTime() <= Date.now()) {
      localStorage.removeItem(STORAGE_KEY)
      return null
    }
    return parsed
  } catch {
    return null
  }
}

export const useAuthStore = defineStore('auth', () => {
  const initial = readStoredAuth()

  const token = ref<string | null>(initial?.token ?? null)
  const userId = ref<number | null>(initial?.userId ?? null)
  const fullName = ref<string | null>(initial?.fullName ?? null)
  const email = ref<string | null>(initial?.email ?? null)
  const role = ref<UserRole | null>(initial?.role ?? null)
  const expiresAtUtc = ref<string | null>(initial?.expiresAtUtc ?? null)

  const isAuthenticated = computed(() => token.value !== null)

  function applySession(response: AuthResponse) {
    token.value = response.token
    userId.value = response.userId
    fullName.value = response.fullName
    email.value = response.email
    role.value = response.role
    expiresAtUtc.value = response.expiresAtUtc

    const toStore: StoredAuth = {
      token: response.token,
      userId: response.userId,
      fullName: response.fullName,
      email: response.email,
      role: response.role,
      expiresAtUtc: response.expiresAtUtc,
    }
    localStorage.setItem(STORAGE_KEY, JSON.stringify(toStore))
  }

  function clearSession() {
    token.value = null
    userId.value = null
    fullName.value = null
    email.value = null
    role.value = null
    expiresAtUtc.value = null
    localStorage.removeItem(STORAGE_KEY)
  }

  async function login(payload: LoginRequest) {
    const response = await authService.login(payload)
    applySession(response)
    return response
  }

  async function register(payload: RegisterRequest) {
    const response = await authService.register(payload)
    applySession(response)
    return response
  }

  async function logout() {
    await authService.logout()
    clearSession()
    router.push('/login')
  }

  return {
    token,
    userId,
    fullName,
    email,
    role,
    isAuthenticated,
    login,
    register,
    logout,
    clearSession,
  }
})
