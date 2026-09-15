import { computed, ref, watch } from 'vue'

import { useApiError } from './useApiError'
import { useClientFilteredList } from './useClientFilteredList'
import { useConfirm } from './useConfirm'
import { useToast } from './useToast'
import { cancelAssignment, getAssignments } from '@/services/assignmentService'
import type { Assignment } from '@/types/assignment'
import { AssignmentStatus } from '@/types/enums'

/** GET /api/assignments has a status filter but no free-text search — see useClientFilteredList. */
export function useAssignments() {
  const search = ref('')
  const statusFilter = ref<'all' | AssignmentStatus>('all')

  const predicate = computed(() => (assignment: Assignment) => {
    if (statusFilter.value !== 'all' && assignment.status !== statusFilter.value) return false

    const query = search.value.trim().toLowerCase()
    if (!query) return true
    return assignment.caregiverFullName.toLowerCase().includes(query) || assignment.clientFullName.toLowerCase().includes(query)
  })

  const list = useClientFilteredList(() => getAssignments().then((r) => r.data), predicate, { pageSize: 10 })

  watch([search, statusFilter], () => list.pagination.reset())

  const toast = useToast()
  const confirm = useConfirm()
  const { getMessage } = useApiError()
  const cancellingId = ref<number | null>(null)

  async function cancel(assignment: Assignment): Promise<boolean> {
    const confirmed = await confirm.ask({
      title: 'Cancel assignment?',
      message: `The assignment between ${assignment.caregiverFullName} and ${assignment.clientFullName} will be cancelled.`,
      confirmLabel: 'Cancel Assignment',
      tone: 'danger',
    })
    if (!confirmed) return false

    cancellingId.value = assignment.id
    try {
      await cancelAssignment(assignment.id)
      toast.success('Assignment cancelled.')
      await list.load()
      return true
    } catch (err) {
      toast.error(getMessage(err))
      return false
    } finally {
      cancellingId.value = null
    }
  }

  return {
    ...list,
    search,
    statusFilter,
    cancel,
    cancellingId,
  }
}
