<script setup lang="ts">
import { watch } from 'vue'

import { cn } from '@/utils/cn'

const props = withDefaults(
  defineProps<{
    modelValue: boolean
    title?: string
    size?: 'sm' | 'md' | 'lg' | 'xl'
    closeOnOverlay?: boolean
  }>(),
  {
    size: 'md',
    closeOnOverlay: true,
  },
)

const emit = defineEmits<{ 'update:modelValue': [boolean] }>()

const sizeClasses: Record<NonNullable<typeof props.size>, string> = {
  sm: 'max-w-sm',
  md: 'max-w-lg',
  lg: 'max-w-2xl',
  xl: 'max-w-4xl',
}

function close() {
  emit('update:modelValue', false)
}

watch(
  () => props.modelValue,
  (open) => {
    document.documentElement.classList.toggle('overflow-hidden', open)
  },
)
</script>

<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition duration-150 ease-out"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition duration-100 ease-in"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="modelValue"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4"
        @mousedown.self="closeOnOverlay && close()"
      >
        <Transition
          enter-active-class="transition duration-150 ease-out"
          enter-from-class="opacity-0 scale-95"
          enter-to-class="opacity-100 scale-100"
          leave-active-class="transition duration-100 ease-in"
          leave-from-class="opacity-100 scale-100"
          leave-to-class="opacity-0 scale-95"
        >
          <div
            v-if="modelValue"
            role="dialog"
            aria-modal="true"
            :class="cn('card-base w-full shadow-popover', sizeClasses[size])"
          >
            <div v-if="title || $slots.header" class="flex items-center justify-between border-b border-border px-5 py-4">
              <slot name="header">
                <h2 class="text-base font-semibold text-ink">{{ title }}</h2>
              </slot>
              <button
                type="button"
                class="rounded-md p-1 text-ink-muted hover:bg-surface-sunken hover:text-ink"
                aria-label="Close"
                @click="close"
              >
                <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                  <path
                    d="M6.28 5.22a.75.75 0 00-1.06 1.06L8.94 10l-3.72 3.72a.75.75 0 101.06 1.06L10 11.06l3.72 3.72a.75.75 0 101.06-1.06L11.06 10l3.72-3.72a.75.75 0 00-1.06-1.06L10 8.94 6.28 5.22z"
                  />
                </svg>
              </button>
            </div>

            <div class="px-5 py-4">
              <slot />
            </div>

            <div v-if="$slots.footer" class="flex items-center justify-end gap-2 border-t border-border px-5 py-4">
              <slot name="footer" />
            </div>
          </div>
        </Transition>
      </div>
    </Transition>
  </Teleport>
</template>
