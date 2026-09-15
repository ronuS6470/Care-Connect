<script setup lang="ts">
withDefaults(
  defineProps<{
    label?: string
    size?: 'sm' | 'md' | 'lg'
  }>(),
  {
    label: 'Loading…',
    size: 'md',
  },
)

const sizeClasses: Record<'sm' | 'md' | 'lg', string> = {
  sm: 'h-5 w-5',
  md: 'h-8 w-8',
  lg: 'h-10 w-10',
}
</script>

<template>
  <div class="flex flex-col items-center justify-center gap-3 text-ink-muted">
    <svg :class="['animate-spin text-brand-600', sizeClasses[size]]" viewBox="0 0 24 24" fill="none" aria-hidden="true">
      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z" />
    </svg>
    <p v-if="label" class="text-sm">{{ label }}</p>
  </div>
</template>
