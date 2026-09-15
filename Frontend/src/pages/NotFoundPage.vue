<script setup lang="ts">
import AppButton from '@/components/common/AppButton.vue'
</script>

<template>
  <div class="flex min-h-screen flex-col items-center justify-center gap-4 bg-surface px-6 text-center">
    <p class="text-sm font-semibold text-brand-600">404</p>
    <h1 class="text-2xl font-semibold text-ink">Page not found</h1>
    <p class="max-w-sm text-sm text-ink-muted">The page you're looking for doesn't exist or may have been moved.</p>
    <AppButton @click="$router.push('/')">Back to home</AppButton>
  </div>
</template>
