import { defineStore } from 'pinia'
import { ref } from 'vue'

export interface ConfirmOptions {
  title: string
  message?: string
  confirmLabel?: string
  cancelLabel?: string
  tone?: 'danger' | 'brand'
}

interface PendingConfirm extends Required<ConfirmOptions> {
  resolve: (confirmed: boolean) => void
}

/** Backs the single global ConfirmDialog mounted in App.vue; drive it via useConfirm(). */
export const useConfirmStore = defineStore('confirm', () => {
  const pending = ref<PendingConfirm | null>(null)

  function request(options: ConfirmOptions): Promise<boolean> {
    return new Promise((resolve) => {
      pending.value = {
        title: options.title,
        message: options.message ?? '',
        confirmLabel: options.confirmLabel ?? 'Confirm',
        cancelLabel: options.cancelLabel ?? 'Cancel',
        tone: options.tone ?? 'brand',
        resolve,
      }
    })
  }

  function resolve(confirmed: boolean) {
    pending.value?.resolve(confirmed)
    pending.value = null
  }

  return { pending, request, resolve }
})
