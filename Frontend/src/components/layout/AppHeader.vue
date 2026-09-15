<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

import Breadcrumb from '@/components/common/Breadcrumb.vue'
import UserMenu from '@/components/common/UserMenu.vue'
import { useUiStore } from '@/stores/ui'

const ui = useUiStore()
const route = useRoute()

const breadcrumbs = computed(() => route.meta.breadcrumb ?? [])
const pageTitle = computed(() => route.meta.title ?? 'CareConnect')
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
      <Breadcrumb v-if="breadcrumbs.length > 1" :items="breadcrumbs" class="mb-0.5 hidden sm:flex" />
      <h1 class="truncate text-base font-semibold text-ink sm:text-lg">{{ pageTitle }}</h1>
    </div>

    <div class="flex shrink-0 items-center gap-1.5">
      <UserMenu />
    </div>
  </header>
</template>
