<script setup lang="ts">
import { computed } from 'vue'

import { cn } from '@/utils/cn'
import type { BadgeTone } from '@/utils/status'

const props = withDefaults(
  defineProps<{
    label: string
    tone?: BadgeTone
  }>(),
  {
    tone: 'neutral',
  },
)

const toneClasses: Record<BadgeTone, string> = {
  success: 'bg-success-50 text-success-700 dark:bg-success-500/10 dark:text-success-500',
  warning: 'bg-warning-50 text-warning-700 dark:bg-warning-500/10 dark:text-warning-500',
  danger: 'bg-danger-50 text-danger-700 dark:bg-danger-500/10 dark:text-danger-500',
  info: 'bg-info-50 text-info-700 dark:bg-info-500/10 dark:text-info-500',
  brand: 'bg-brand-50 text-brand-700 dark:bg-brand-500/10 dark:text-brand-300',
  neutral: 'bg-surface-sunken text-ink-muted',
}

const classes = computed(() => cn('inline-flex items-center rounded-full px-2.5 py-1 text-xs font-medium', toneClasses[props.tone]))
</script>

<template>
  <span :class="classes">{{ label }}</span>
</template>
