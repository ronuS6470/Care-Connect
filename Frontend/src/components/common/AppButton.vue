<script setup lang="ts">
import { computed } from 'vue'

import { cn } from '@/utils/cn'

export type ButtonVariant = 'primary' | 'accent' | 'secondary' | 'outline' | 'ghost' | 'danger'
export type ButtonSize = 'sm' | 'md' | 'lg'

const props = withDefaults(
  defineProps<{
    variant?: ButtonVariant
    size?: ButtonSize
    type?: 'button' | 'submit' | 'reset'
    loading?: boolean
    disabled?: boolean
    block?: boolean
  }>(),
  {
    variant: 'primary',
    size: 'md',
    type: 'button',
    loading: false,
    disabled: false,
    block: false,
  },
)

defineEmits<{ click: [MouseEvent] }>()

const variantClasses: Record<ButtonVariant, string> = {
  primary: 'bg-brand-700 text-white hover:bg-brand-800 focus-visible:ring-brand-600 dark:bg-brand-600 dark:hover:bg-brand-500',
  accent: 'bg-accent-500 text-white hover:bg-accent-600 focus-visible:ring-accent-500',
  secondary:
    'bg-surface-sunken text-ink hover:bg-border focus-visible:ring-brand-500 dark:bg-surface-elevated dark:hover:bg-border',
  outline: 'border border-border bg-transparent text-ink hover:bg-surface-sunken focus-visible:ring-brand-500',
  ghost: 'bg-transparent text-ink hover:bg-surface-sunken focus-visible:ring-brand-500',
  danger: 'bg-danger-600 text-white hover:bg-danger-700 focus-visible:ring-danger-600',
}

const sizeClasses: Record<ButtonSize, string> = {
  sm: 'h-8 px-3 text-xs',
  md: 'h-10 px-4 text-sm',
  lg: 'h-12 px-6 text-base',
}

const classes = computed(() =>
  cn('btn-base', variantClasses[props.variant], sizeClasses[props.size], props.block && 'w-full'),
)
</script>

<template>
  <button :type="type" :class="classes" :disabled="disabled || loading" @click="$emit('click', $event)">
    <svg
      v-if="loading"
      class="h-4 w-4 animate-spin"
      viewBox="0 0 24 24"
      fill="none"
      aria-hidden="true"
    >
      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z" />
    </svg>
    <slot />
  </button>
</template>
