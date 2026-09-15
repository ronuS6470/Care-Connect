<script setup lang="ts">
import UserMenu from '@/components/common/UserMenu.vue'
import { useUiStore } from '@/stores/ui'

const ui = useUiStore()
</script>

<template>
  <header class="sticky top-0 z-30 flex h-16 shrink-0 items-center gap-4 border-b border-border bg-surface-elevated/90 px-4 backdrop-blur sm:px-6">
    <button
      type="button"
      class="rounded-lg p-2 text-ink-muted hover:bg-surface-sunken lg:hidden"
      aria-label="Open menu"
      @click="ui.openMobileSidebar()"
    >
      <svg class="h-5 w-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" aria-hidden="true">
        <path stroke-linecap="round" stroke-linejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5M3.75 17.25h16.5" />
      </svg>
    </button>

    <div class="min-w-0 flex-1">
      <slot />
    </div>

    <div class="flex shrink-0 items-center gap-1.5">
      <button
        type="button"
        class="rounded-lg p-2 text-ink-muted hover:bg-surface-sunken"
        aria-label="Notifications"
      >
        <svg class="h-5 w-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" aria-hidden="true">
          <path stroke-linecap="round" stroke-linejoin="round" d="M14.857 17.082a23.848 23.848 0 005.454-1.31A8.967 8.967 0 0118 9.75V9A6 6 0 006 9v.75a8.967 8.967 0 01-2.312 6.022c1.733.64 3.56 1.085 5.455 1.31m5.714 0a24.255 24.255 0 01-5.714 0m5.714 0a3 3 0 11-5.714 0" />
        </svg>
      </button>

      <div class="mx-1 hidden h-6 w-px bg-border sm:block" />

      <UserMenu />
    </div>
  </header>
</template>
