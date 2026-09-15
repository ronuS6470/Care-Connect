import { defineStore } from 'pinia'
import { computed, ref, watchEffect } from 'vue'

export type ThemeMode = 'light' | 'dark' | 'system'

const STORAGE_KEY = 'careconnect.theme'

function readStoredMode(): ThemeMode {
  const stored = localStorage.getItem(STORAGE_KEY)
  return stored === 'light' || stored === 'dark' || stored === 'system' ? stored : 'system'
}

function systemPrefersDark(): boolean {
  return window.matchMedia('(prefers-color-scheme: dark)').matches
}

export const useThemeStore = defineStore('theme', () => {
  const mode = ref<ThemeMode>(readStoredMode())

  const isDark = computed(() => (mode.value === 'system' ? systemPrefersDark() : mode.value === 'dark'))

  function setMode(next: ThemeMode) {
    mode.value = next
    localStorage.setItem(STORAGE_KEY, next)
  }

  function toggle() {
    setMode(isDark.value ? 'light' : 'dark')
  }

  // Keeps the <html>.dark class in sync with mode/system-preference changes; called once from
  // App.vue's setup so it starts watching immediately on app boot.
  function init() {
    const media = window.matchMedia('(prefers-color-scheme: dark)')
    const syncSystemChange = () => {
      if (mode.value === 'system') applyClass()
    }
    media.addEventListener('change', syncSystemChange)

    watchEffect(applyClass)
  }

  function applyClass() {
    document.documentElement.classList.toggle('dark', isDark.value)
  }

  return { mode, isDark, setMode, toggle, init }
})
