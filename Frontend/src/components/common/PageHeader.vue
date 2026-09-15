<script setup lang="ts">
import type { BreadcrumbItem } from '@/types/common'

import Breadcrumb from './Breadcrumb.vue'

withDefaults(
  defineProps<{
    title: string
    description?: string
    breadcrumbs?: BreadcrumbItem[]
  }>(),
  {},
)
</script>

<template>
  <div class="mb-6 flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
    <div class="min-w-0">
      <Breadcrumb v-if="breadcrumbs?.length" :items="breadcrumbs" class="mb-2" />
      <h1 class="truncate text-xl font-semibold text-ink sm:text-2xl">{{ title }}</h1>
      <p v-if="description" class="mt-1 text-sm text-ink-muted">{{ description }}</p>
    </div>

    <div v-if="$slots.actions" class="flex shrink-0 items-center gap-2">
      <slot name="actions" />
    </div>
  </div>
</template>
