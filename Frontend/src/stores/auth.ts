import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

import * as authService from '@/services/auth.service'
import type { AuthSession, LoginPayload } from '@/types/auth'

const STORAGE_KEY = 'careconnect.session'

function readStoredSession(): AuthSession | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    return raw ? (JSON.parse(raw) as AuthSession) : null
  } catch {
    return null
  }
}

export const useAuthStore = defineStore('auth', () => {
  const session = ref<AuthSession | null>(readStoredSession())

  const isAuthenticated = computed(() => session.value !== null)
  const role = computed(() => session.value?.role ?? null)

  function persist(next: AuthSession | null) {
    session.value = next
    if (next) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(next))
    } else {
      localStorage.removeItem(STORAGE_KEY)
    }
  }

  async function login(payload: LoginPayload) {
    const next = await authService.login(payload)
    persist(next)
    return next
  }

  async function logout() {
    await authService.logout()
    persist(null)
  }

  function clearSession() {
    persist(null)
  }

  return { session, isAuthenticated, role, login, logout, clearSession }
})
