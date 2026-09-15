<script setup lang="ts">
import { useUiStore } from '@/stores/ui'

import NavItem, { type NavLink } from './NavItem.vue'

defineProps<{ items: NavLink[] }>()

const ui = useUiStore()
</script>

<template>
  <!-- Mobile overlay -->
  <Transition
    enter-active-class="transition duration-150 ease-out"
    enter-from-class="opacity-0"
    enter-to-class="opacity-100"
    leave-active-class="transition duration-100 ease-in"
    leave-from-class="opacity-100"
    leave-to-class="opacity-0"
  >
    <div
      v-if="ui.mobileSidebarOpen"
      class="fixed inset-0 z-40 bg-black/40 lg:hidden"
      @click="ui.closeMobileSidebar()"
    />
  </Transition>

  <aside
    class="fixed inset-y-0 left-0 z-50 flex w-64 flex-col bg-sidebar transition-transform duration-200 lg:sticky lg:top-0 lg:h-screen lg:translate-x-0"
    :class="ui.mobileSidebarOpen ? 'translate-x-0' : '-translate-x-full'"
  >
    <div class="flex h-16 shrink-0 items-center gap-2 px-5">
      <span class="flex h-9 w-9 items-center justify-center rounded-lg bg-accent-500 text-sm font-bold text-white">CC</span>
      <span class="text-lg font-semibold tracking-tight text-white">CareConnect</span>
    </div>

    <nav class="flex-1 space-y-1 overflow-y-auto px-3 py-2">
      <NavItem v-for="item in items" :key="item.to" :item="item" @navigate="ui.closeMobileSidebar()" />
    </nav>

    <div class="border-t border-white/10 px-4 py-4">
      <p class="text-xs text-sidebar-ink-muted">CareConnect &copy; {{ new Date().getFullYear() }}</p>
    </div>
  </aside>
</template>
