import { defineStore } from 'pinia'
import { ref } from 'vue'

export type ToastType = 'success' | 'error' | 'info' | 'warning'

export interface Toast {
  id: number
  type: ToastType
  title: string
  description?: string
  duration: number
}

export interface ToastInput {
  title: string
  description?: string
  duration?: number
}

let nextId = 1

export const useToastStore = defineStore('toast', () => {
  const toasts = ref<Toast[]>([])

  function push(type: ToastType, input: ToastInput | string) {
    const options: ToastInput = typeof input === 'string' ? { title: input } : input
    const toast: Toast = {
      id: nextId++,
      type,
      title: options.title,
      description: options.description,
      duration: options.duration ?? 5000,
    }

    toasts.value.push(toast)
    return toast.id
  }

  function dismiss(id: number) {
    toasts.value = toasts.value.filter((toast) => toast.id !== id)
  }

  return {
    toasts,
    dismiss,
    success: (input: ToastInput | string) => push('success', input),
    error: (input: ToastInput | string) => push('error', input),
    info: (input: ToastInput | string) => push('info', input),
    warning: (input: ToastInput | string) => push('warning', input),
  }
})
