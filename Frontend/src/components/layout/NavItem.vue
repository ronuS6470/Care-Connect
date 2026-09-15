<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

import { NAV_ICON_PATHS, type NavIconName } from './navIcons'

export interface NavLink {
  label: string
  to: string
  icon: NavIconName
}

const props = defineProps<{ item: NavLink }>()
const emit = defineEmits<{ navigate: [] }>()

const route = useRoute()

const isActive = computed(() => route.path === props.item.to || route.path.startsWith(`${props.item.to}/`))
</script>

<template>
  <RouterLink
    :to="item.to"
    class="group flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors"
    :class="
      isActive
        ? 'bg-sidebar-active text-white'
        : 'text-sidebar-ink-muted hover:bg-sidebar-active/60 hover:text-sidebar-ink'
    "
    @click="emit('navigate')"
  >
    <svg class="h-5 w-5 shrink-0" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" aria-hidden="true">
      <path stroke-linecap="round" stroke-linejoin="round" :d="NAV_ICON_PATHS[item.icon]" />
    </svg>
    <span class="truncate">{{ item.label }}</span>
  </RouterLink>
</template>
