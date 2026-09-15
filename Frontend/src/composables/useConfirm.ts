import { useConfirmStore, type ConfirmOptions } from '@/stores/confirm'

/**
 * Promise-based confirmation, backed by the single ConfirmDialog mounted in App.vue:
 *
 *   const confirmed = await useConfirm().ask({ title: 'Cancel this visit?', tone: 'danger' })
 *   if (!confirmed) return
 */
export function useConfirm() {
  const store = useConfirmStore()

  return {
    ask: (options: ConfirmOptions) => store.request(options),
  }
}
