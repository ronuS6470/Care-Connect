<script setup lang="ts">
import { computed } from 'vue'

import { useConfirmStore } from '@/stores/confirm'

import AppButton from './AppButton.vue'
import AppModal from './AppModal.vue'

const store = useConfirmStore()

const isOpen = computed({
  get: () => store.pending !== null,
  set: (value: boolean) => {
    if (!value) store.resolve(false)
  },
})
</script>

<template>
  <AppModal v-model="isOpen" size="sm" :title="store.pending?.title">
    <p class="text-sm text-ink-muted">{{ store.pending?.message }}</p>

    <template #footer>
      <AppButton variant="outline" @click="store.resolve(false)">
        {{ store.pending?.cancelLabel }}
      </AppButton>
      <AppButton :variant="store.pending?.tone === 'danger' ? 'danger' : 'primary'" @click="store.resolve(true)">
        {{ store.pending?.confirmLabel }}
      </AppButton>
    </template>
  </AppModal>
</template>
