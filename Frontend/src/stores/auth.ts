import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

import router from '@/router'
import * as authService from '@/services/authService'
import type { AuthenticatedUser, LoginRequest, LoginResponse } from '@/types/auth'
import { ROLE_HOME_PATH, UserRole } from '@/types/enums'
import { authStorage, type StoredSession } from '@/utils/authStorage'

/**
 * A LoginResponse's `role` is one of the three role names the backend contract defines ("Admin",
 * "Caregiver", "Client" — see types/auth.ts); the rest of this app (route guards, nav, badges)
 * is built on the numeric UserRole enum in types/enums.ts instead. This store is the one place a
 * LoginResponse enters the app, so it's the one place that translates between the two — nothing
 * downstream of it needs to know the wire contract used a string.
 */
const ROLE_ID_BY_NAME: Record<AuthenticatedUser['role'], UserRole> = {
  Admin: UserRole.Admin,
  Caregiver: UserRole.Caregiver,
  Client: UserRole.Client,
}

const ROLE_NAME_BY_ID: Record<UserRole, AuthenticatedUser['role']> = {
  [UserRole.Admin]: 'Admin',
  [UserRole.Caregiver]: 'Caregiver',
  [UserRole.Client]: 'Client',
}

/**
 * Whether a restored session has already run out. Where the session is *kept* is
 * utils/authStorage.ts's concern; whether it's still usable is this store's.
 */
function isExpired(expiresAtUtc: string): boolean {
  const expiresAt = new Date(expiresAtUtc).getTime()
  return Number.isNaN(expiresAt) || expiresAt <= Date.now()
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(null)
  const userId = ref<number | null>(null)
  const fullName = ref<string | null>(null)
  const email = ref<string | null>(null)
  const role = ref<UserRole | null>(null)
  const expiresAtUtc = ref<string | null>(null)
  const loading = ref(false)

  const isAuthenticated = computed(() => token.value !== null)
  const isAdmin = computed(() => role.value === UserRole.Admin)
  const isCaregiver = computed(() => role.value === UserRole.Caregiver)
  const isClient = computed(() => role.value === UserRole.Client)

  /** The signed-in user as the rest of the app's authentication contract describes it (see AuthenticatedUser). */
  const currentUser = computed<AuthenticatedUser | null>(() => {
    if (userId.value === null || fullName.value === null || email.value === null || role.value === null) {
      return null
    }
    return { userId: userId.value, fullName: fullName.value, email: email.value, role: ROLE_NAME_BY_ID[role.value] }
  })

  /** Applies a login response to in-memory state and persists it — the one place a LoginResponse becomes a session. */
  function setSession(response: LoginResponse) {
    const roleId = ROLE_ID_BY_NAME[response.role]

    token.value = response.token
    userId.value = response.userId
    fullName.value = response.fullName
    email.value = response.email
    role.value = roleId
    expiresAtUtc.value = response.expiresAtUtc

    const toStore: StoredSession = {
      token: response.token,
      userId: response.userId,
      fullName: response.fullName,
      email: response.email,
      role: roleId,
      expiresAtUtc: response.expiresAtUtc,
    }
    authStorage.save(toStore)
  }

  function clearSession() {
    token.value = null
    userId.value = null
    fullName.value = null
    email.value = null
    role.value = null
    expiresAtUtc.value = null
    authStorage.clear()
  }

  /**
   * Does not validate credentials, generate a token, or decide who's allowed in — it only sends
   * `payload` to POST /api/auth/login (via authService) and stores whatever the backend decides
   * to send back. A rejected login surfaces as a thrown ApiError for the caller (LoginPage) to
   * display; nothing here inspects *why* it failed, and nothing here runs on a failed attempt.
   *
   * On success, redirects to wherever the user was headed before being sent to /login (the
   * `redirect` query param a route guard or the 401 interceptor may have attached) or, absent
   * that, to their role's home dashboard — the one place this app decides that, so no page needs
   * its own copy of ROLE_HOME_PATH lookup logic.
   */
  async function login(payload: LoginRequest) {
    loading.value = true
    try {
      const response = await authService.login(payload)
      setSession(response)

      const intendedPath = router.currentRoute.value.query.redirect
      router.push(typeof intendedPath === 'string' ? intendedPath : ROLE_HOME_PATH[role.value!])

      return response
    } finally {
      loading.value = false
    }
  }

  async function logout() {
    await authService.logout()
    clearSession()
    router.push('/login')
  }

  /**
   * Called once at app startup (see main.ts), before the router's first navigation runs. Restores
   * whatever session authStorage has — but that's only ever a *hint*: a token surviving the local
   * expiresAtUtc check does not mean the backend still honors it (revoked, rotated secret, etc.).
   * The backend remains the sole authority on that; the request/response interceptors in
   * services/api.ts catch a stale-but-locally-fresh-looking token on its first real use (401 →
   * cleared session + redirect to /login) exactly like any other expired session.
   */
  function initializeAuth() {
    loading.value = true
    try {
      const stored = authStorage.read()
      if (!stored || isExpired(stored.expiresAtUtc)) {
        // clearSession() also wipes the expired entry out of storage.
        clearSession()
        return
      }

      token.value = stored.token
      userId.value = stored.userId
      fullName.value = stored.fullName
      email.value = stored.email
      role.value = stored.role
      expiresAtUtc.value = stored.expiresAtUtc
    } finally {
      loading.value = false
    }
  }

  return {
    token,
    userId,
    fullName,
    email,
    role,
    isAuthenticated,
    loading,
    isAdmin,
    isCaregiver,
    isClient,
    currentUser,
    login,
    logout,
    initializeAuth,
    setSession,
    clearSession,
  }
})
