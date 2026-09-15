<script setup lang="ts">
import { useToastStore, type Toast, type ToastType } from '@/stores/toast'
import { cn } from '@/utils/cn'

const store = useToastStore()

const iconWrapClasses: Record<ToastType, string> = {
  success: 'bg-success-50 text-success-600 dark:bg-success-500/10',
  error: 'bg-danger-50 text-danger-600 dark:bg-danger-500/10',
  info: 'bg-info-50 text-info-600 dark:bg-info-500/10',
  warning: 'bg-warning-50 text-warning-600 dark:bg-warning-500/10',
}

function scheduleDismiss(toast: Toast) {
  setTimeout(() => store.dismiss(toast.id), toast.duration)
}
</script>

<template>
  <Teleport to="body">
    <div class="pointer-events-none fixed inset-x-0 top-4 z-[60] flex flex-col items-center gap-2 px-4 sm:items-end sm:right-4 sm:left-auto">
      <TransitionGroup
        enter-active-class="transition duration-200 ease-out"
        enter-from-class="opacity-0 -translate-y-2"
        enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition duration-150 ease-in"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0"
      >
        <div
          v-for="toast in store.toasts"
          :key="toast.id"
          v-memo="[toast.id]"
          role="status"
          :class="cn('pointer-events-auto flex w-full max-w-sm items-start gap-3 rounded-xl border border-border bg-surface-elevated p-4 shadow-popover')"
          @vue:mounted="scheduleDismiss(toast)"
        >
          <span :class="cn('flex h-8 w-8 shrink-0 items-center justify-center rounded-full', iconWrapClasses[toast.type])">
            <svg v-if="toast.type === 'success'" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
              <path fill-rule="evenodd" d="M16.704 4.153a.75.75 0 01.143 1.052l-8 10.5a.75.75 0 01-1.127.075l-4.5-4.5a.75.75 0 011.06-1.06l3.894 3.893 7.48-9.817a.75.75 0 011.05-.143z" clip-rule="evenodd" />
            </svg>
            <svg v-else-if="toast.type === 'error'" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.28 7.22a.75.75 0 00-1.06 1.06L8.94 10l-1.72 1.72a.75.75 0 101.06 1.06L10 11.06l1.72 1.72a.75.75 0 101.06-1.06L11.06 10l1.72-1.72a.75.75 0 00-1.06-1.06L10 8.94 8.28 7.22z" clip-rule="evenodd" />
            </svg>
            <svg v-else-if="toast.type === 'warning'" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
              <path fill-rule="evenodd" d="M8.485 2.495c.673-1.167 2.357-1.167 3.03 0l6.28 10.875c.673 1.167-.17 2.63-1.516 2.63H3.72c-1.347 0-2.189-1.463-1.515-2.63L8.485 2.495zM10 6a.75.75 0 01.75.75v3.5a.75.75 0 01-1.5 0v-3.5A.75.75 0 0110 6zm0 8a1 1 0 100-2 1 1 0 000 2z" clip-rule="evenodd" />
            </svg>
            <svg v-else class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
              <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a.75.75 0 000 1.5h.253a.25.25 0 01.244.304l-.459 2.066A1.75 1.75 0 0010.747 15H11a.75.75 0 000-1.5h-.253a.25.25 0 01-.244-.304l.459-2.066A1.75 1.75 0 009.253 9H9z" clip-rule="evenodd" />
            </svg>
          </span>

          <div class="min-w-0 flex-1">
            <p class="text-sm font-medium text-ink">{{ toast.title }}</p>
            <p v-if="toast.description" class="mt-0.5 text-sm text-ink-muted">{{ toast.description }}</p>
          </div>

          <button type="button" class="shrink-0 text-ink-muted hover:text-ink" aria-label="Dismiss" @click="store.dismiss(toast.id)">
            <svg class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
              <path d="M6.28 5.22a.75.75 0 00-1.06 1.06L8.94 10l-3.72 3.72a.75.75 0 101.06 1.06L10 11.06l3.72 3.72a.75.75 0 101.06-1.06L11.06 10l3.72-3.72a.75.75 0 00-1.06-1.06L10 8.94 6.28 5.22z" />
            </svg>
          </button>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>
