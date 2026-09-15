import { computed, ref, watch } from 'vue'

import { useApiError } from './useApiError'
import { useClientFilteredList } from './useClientFilteredList'
import { useConfirm } from './useConfirm'
import { useToast } from './useToast'
import { deactivateCareTask, getCareTasks } from '@/services/careTaskService'
import type { CareTask } from '@/types/careTask'

export type CareTaskStatusFilter = 'all' | 'active' | 'inactive'

/** GET /api/care-tasks has an isActive filter but no free-text search — see useClientFilteredList. */
export function useCareTasks() {
  const search = ref('')
  const statusFilter = ref<CareTaskStatusFilter>('all')

  const predicate = computed(() => (task: CareTask) => {
    if (statusFilter.value === 'active' && !task.isActive) return false
    if (statusFilter.value === 'inactive' && task.isActive) return false

    const query = search.value.trim().toLowerCase()
    if (!query) return true
    return task.name.toLowerCase().includes(query) || (task.description ?? '').toLowerCase().includes(query)
  })

  const list = useClientFilteredList(() => getCareTasks().then((r) => r.data), predicate, { pageSize: 10 })

  watch([search, statusFilter], () => list.pagination.reset())

  const toast = useToast()
  const confirm = useConfirm()
  const { getMessage } = useApiError()
  const deactivatingId = ref<number | null>(null)

  async function deactivate(task: CareTask): Promise<boolean> {
    const confirmed = await confirm.ask({
      title: 'Deactivate care task?',
      message: `"${task.name}" will no longer be available to add to new visits.`,
      confirmLabel: 'Deactivate',
      tone: 'danger',
    })
    if (!confirmed) return false

    deactivatingId.value = task.id
    try {
      await deactivateCareTask(task.id)
      toast.success('Care task deactivated.')
      await list.load()
      return true
    } catch (err) {
      toast.error(getMessage(err))
      return false
    } finally {
      deactivatingId.value = null
    }
  }

  return {
    ...list,
    search,
    statusFilter,
    deactivate,
    deactivatingId,
  }
}
