<script setup lang="ts">
import { cn } from '@/utils/cn'

withDefaults(
  defineProps<{
    title?: string
    description?: string
    padded?: boolean
  }>(),
  {
    padded: true,
  },
)
</script>

<template>
  <div class="card-base">
    <div v-if="title || description || $slots.actions" class="flex items-start justify-between gap-4 border-b border-border px-5 py-4">
      <div>
        <h3 v-if="title" class="text-sm font-semibold text-ink">{{ title }}</h3>
        <p v-if="description" class="mt-0.5 text-sm text-ink-muted">{{ description }}</p>
      </div>
      <div v-if="$slots.actions" class="shrink-0">
        <slot name="actions" />
      </div>
    </div>

    <div :class="cn(padded && 'p-5')">
      <slot />
    </div>
  </div>
</template>
