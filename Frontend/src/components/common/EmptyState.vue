<script setup lang="ts">
withDefaults(
  defineProps<{
    title: string
    description?: string
  }>(),
  {},
)
</script>

<template>
  <div class="flex flex-col items-center justify-center gap-3 px-6 py-12 text-center">
    <div class="flex h-12 w-12 items-center justify-center rounded-full bg-surface-sunken text-ink-muted">
      <slot name="icon">
        <svg class="h-6 w-6" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" aria-hidden="true">
          <path stroke-linecap="round" stroke-linejoin="round" d="M3.75 9.75h16.5M3.75 9.75v8.25A2.25 2.25 0 006 20.25h12a2.25 2.25 0 002.25-2.25V9.75M3.75 9.75l1.928-4.821A1.5 1.5 0 017.06 4h9.88a1.5 1.5 0 011.382.929L20.25 9.75" />
        </svg>
      </slot>
    </div>
    <div>
      <p class="text-sm font-semibold text-ink">{{ title }}</p>
      <p v-if="description" class="mt-1 text-sm text-ink-muted">{{ description }}</p>
    </div>
    <div v-if="$slots.action" class="mt-1">
      <slot name="action" />
    </div>
  </div>
</template>
