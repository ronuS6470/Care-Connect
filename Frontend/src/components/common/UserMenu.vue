<script setup lang="ts">
import { ref } from 'vue'

import { useClickOutside } from '@/composables/useClickOutside'
import { useTheme } from '@/composables/useTheme'
import { useAuthStore } from '@/stores/auth'
import { USER_ROLE_LABELS } from '@/types/enums'
import { initials } from '@/utils/string'

const auth = useAuthStore()
const { isDark, toggle } = useTheme()

const open = ref(false)
const menuRef = ref<HTMLElement | null>(null)

useClickOutside(menuRef, () => {
  open.value = false
})
</script>

<template>
  <div ref="menuRef" class="relative">
    <button
      type="button"
      class="flex items-center gap-2 rounded-full p-1 pr-2 hover:bg-surface-sunken focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand-500"
      :aria-expanded="open"
      @click="open = !open"
    >
      <span class="flex h-8 w-8 items-center justify-center rounded-full bg-brand-700 text-xs font-semibold text-white">
        {{ initials(auth.fullName) }}
      </span>
      <span class="hidden text-left sm:block">
        <span class="block text-sm font-medium leading-tight text-ink">{{ auth.fullName ?? 'Guest' }}</span>
        <span class="block text-xs leading-tight text-ink-muted">{{ auth.role !== null ? USER_ROLE_LABELS[auth.role] : '' }}</span>
      </span>
    </button>

    <Transition
      enter-active-class="transition duration-100 ease-out"
      enter-from-class="opacity-0 scale-95"
      enter-to-class="opacity-100 scale-100"
      leave-active-class="transition duration-75 ease-in"
      leave-from-class="opacity-100 scale-100"
      leave-to-class="opacity-0 scale-95"
    >
      <div v-if="open" class="dropdown-panel absolute right-0 z-40 mt-2 w-56 origin-top-right">
        <div class="px-2.5 py-2 sm:hidden">
          <p class="text-sm font-medium text-ink">{{ auth.fullName ?? 'Guest' }}</p>
          <p class="text-xs text-ink-muted">{{ auth.email }}</p>
        </div>

        <button type="button" class="dropdown-item justify-between" @click="toggle">
          <span>{{ isDark ? 'Dark mode' : 'Light mode' }}</span>
          <span
            class="relative inline-flex h-5 w-9 shrink-0 items-center rounded-full transition-colors"
            :class="isDark ? 'bg-brand-700' : 'bg-border'"
          >
            <span
              class="inline-block h-4 w-4 transform rounded-full bg-white transition-transform"
              :class="isDark ? 'translate-x-4' : 'translate-x-0.5'"
            />
          </span>
        </button>

        <div class="my-1 h-px bg-border" />

        <button
          type="button"
          class="dropdown-item text-danger-600 hover:bg-danger-50 dark:hover:bg-danger-500/10"
          @click="auth.logout()"
        >
          <svg class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
            <path fill-rule="evenodd" d="M3 4.25A2.25 2.25 0 015.25 2h5.5A2.25 2.25 0 0113 4.25v2a.75.75 0 01-1.5 0v-2a.75.75 0 00-.75-.75h-5.5a.75.75 0 00-.75.75v11.5c0 .414.336.75.75.75h5.5a.75.75 0 00.75-.75v-2a.75.75 0 011.5 0v2A2.25 2.25 0 0110.75 18h-5.5A2.25 2.25 0 013 15.75V4.25z" clip-rule="evenodd" />
            <path fill-rule="evenodd" d="M6 10a.75.75 0 01.75-.75h9.546l-1.048-.943a.75.75 0 111.004-1.114l2.5 2.25a.75.75 0 010 1.114l-2.5 2.25a.75.75 0 11-1.004-1.114l1.048-.943H6.75A.75.75 0 016 10z" clip-rule="evenodd" />
          </svg>
          Sign out
        </button>
      </div>
    </Transition>
  </div>
</template>
