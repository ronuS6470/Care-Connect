<script setup lang="ts">
import { computed } from 'vue'

import { cn } from '@/utils/cn'

export type AlertTone = 'info' | 'success' | 'warning' | 'danger'

const props = withDefaults(
  defineProps<{
    tone?: AlertTone
    title?: string
    dismissible?: boolean
  }>(),
  {
    tone: 'info',
    dismissible: false,
  },
)

defineEmits<{ dismiss: [] }>()

const toneClasses: Record<AlertTone, string> = {
  info: 'border-info-200 bg-info-50 text-info-700 dark:border-info-500/20 dark:bg-info-500/10 dark:text-info-500',
  success:
    'border-success-200 bg-success-50 text-success-700 dark:border-success-500/20 dark:bg-success-500/10 dark:text-success-500',
  warning:
    'border-warning-200 bg-warning-50 text-warning-700 dark:border-warning-500/20 dark:bg-warning-500/10 dark:text-warning-500',
  danger:
    'border-danger-200 bg-danger-50 text-danger-700 dark:border-danger-500/20 dark:bg-danger-500/10 dark:text-danger-500',
}

const iconPaths: Record<AlertTone, string> = {
  info: 'M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a.75.75 0 000 1.5h.253a.25.25 0 01.244.304l-.459 2.066A1.75 1.75 0 0010.747 15H11a.75.75 0 000-1.5h-.253a.25.25 0 01-.244-.304l.459-2.066A1.75 1.75 0 009.253 9H9z',
  success:
    'M16.704 4.153a.75.75 0 01.143 1.052l-8 10.5a.75.75 0 01-1.127.075l-4.5-4.5a.75.75 0 011.06-1.06l3.894 3.893 7.48-9.817a.75.75 0 011.05-.143z',
  warning:
    'M8.485 2.495c.673-1.167 2.357-1.167 3.03 0l6.28 10.875c.673 1.167-.17 2.63-1.516 2.63H3.72c-1.347 0-2.189-1.463-1.515-2.63L8.485 2.495zM10 6a.75.75 0 01.75.75v3.5a.75.75 0 01-1.5 0v-3.5A.75.75 0 0110 6zm0 8a1 1 0 100-2 1 1 0 000 2z',
  danger: 'M10 18a8 8 0 100-16 8 8 0 000 16zM8.28 7.22a.75.75 0 00-1.06 1.06L8.94 10l-1.72 1.72a.75.75 0 101.06 1.06L10 11.06l1.72 1.72a.75.75 0 101.06-1.06L11.06 10l1.72-1.72a.75.75 0 00-1.06-1.06L10 8.94 8.28 7.22z',
}

const classes = computed(() => cn('alert-base', toneClasses[props.tone]))
</script>

<template>
  <div role="alert" :class="classes">
    <svg class="mt-0.5 h-4 w-4 shrink-0" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
      <path fill-rule="evenodd" :d="iconPaths[tone]" clip-rule="evenodd" />
    </svg>

    <div class="min-w-0 flex-1">
      <p v-if="title" class="font-medium">{{ title }}</p>
      <div :class="title && 'mt-0.5 opacity-90'">
        <slot />
      </div>
    </div>

    <button v-if="dismissible" type="button" class="shrink-0 opacity-70 hover:opacity-100" aria-label="Dismiss" @click="$emit('dismiss')">
      <svg class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
        <path d="M6.28 5.22a.75.75 0 00-1.06 1.06L8.94 10l-3.72 3.72a.75.75 0 101.06 1.06L10 11.06l3.72 3.72a.75.75 0 101.06-1.06L11.06 10l3.72-3.72a.75.75 0 00-1.06-1.06L10 8.94 6.28 5.22z" />
      </svg>
    </button>
  </div>
</template>
