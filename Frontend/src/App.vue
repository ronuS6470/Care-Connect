<script setup lang="ts">
import { onMounted } from 'vue'

import ConfirmDialog from '@/components/common/ConfirmDialog.vue'
import ToastNotification from '@/components/common/ToastNotification.vue'
import { useThemeStore } from '@/stores/theme'

const theme = useThemeStore()

onMounted(() => {
  theme.init()
})
</script>

<template>
  <RouterView />
  <ToastNotification />
  <ConfirmDialog />
</template>
