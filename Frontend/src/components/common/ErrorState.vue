<script setup lang="ts">
import AppButton from './AppButton.vue'

withDefaults(
  defineProps<{
    title?: string
    description?: string
  }>(),
  {
    title: 'Something went wrong',
  },
)

defineEmits<{ retry: [] }>()
</script>

<template>
  <div class="flex flex-col items-center justify-center gap-3 px-6 py-12 text-center">
    <div class="flex h-12 w-12 items-center justify-center rounded-full bg-danger-50 text-danger-600 dark:bg-danger-500/10">
      <svg class="h-6 w-6" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" aria-hidden="true">
        <path stroke-linecap="round" stroke-linejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126zM12 15.75h.007v.008H12v-.008z" />
      </svg>
    </div>
    <div>
      <p class="text-sm font-semibold text-ink">{{ title }}</p>
      <p v-if="description" class="mt-1 text-sm text-ink-muted">{{ description }}</p>
    </div>
    <AppButton variant="outline" size="sm" @click="$emit('retry')">Try again</AppButton>
  </div>
</template>
