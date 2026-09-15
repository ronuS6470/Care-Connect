<script setup lang="ts">
import { useRouter } from 'vue-router'

import AppButton from '@/components/common/AppButton.vue'
import { useAuthStore } from '@/stores/auth'
import { ROLE_HOME_PATH } from '@/types/enums'

const auth = useAuthStore()
const router = useRouter()

function goHome() {
  router.push(auth.role !== null ? ROLE_HOME_PATH[auth.role] : '/login')
}
</script>

<template>
  <div class="flex min-h-screen flex-col items-center justify-center gap-4 bg-surface px-6 text-center">
    <p class="text-sm font-semibold text-danger-600">403</p>
    <h1 class="text-2xl font-semibold text-ink">You don't have access to this page</h1>
    <p class="max-w-sm text-sm text-ink-muted">Your account role doesn't include this section of CareConnect.</p>
    <AppButton @click="goHome">Back to my dashboard</AppButton>
  </div>
</template>
