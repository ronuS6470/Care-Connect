import { computed, ref, watch } from 'vue'

import { useApiError } from './useApiError'
import { useClientFilteredList } from './useClientFilteredList'
import { useConfirm } from './useConfirm'
import { useToast } from './useToast'
import { activateCaregiver, deactivateCaregiver, getCaregivers } from '@/services/caregiverService'
import type { Caregiver } from '@/types/caregiver'

/**
 * Backs both CaregiversListPage (the `search` ref + `rows`/`pagination`) and
 * CaregiverDetailPage (just `toggleActive`, ignoring the list bits) — toggleActive used to be
 * copy-pasted between the two.
 */
export function useCaregivers() {
  const search = ref('')

  const predicate = computed(() => (caregiver: Caregiver) => {
    const query = search.value.trim().toLowerCase()
    if (!query) return true
    return (
      caregiver.fullName.toLowerCase().includes(query) ||
      caregiver.email.toLowerCase().includes(query) ||
      (caregiver.licenseNumber ?? '').toLowerCase().includes(query)
    )
  })

  const list = useClientFilteredList(() => getCaregivers().then((r) => r.data), predicate, { pageSize: 10 })

  watch(search, () => list.pagination.reset())

  const toast = useToast()
  const confirm = useConfirm()
  const { getMessage } = useApiError()
  const togglingId = ref<number | null>(null)

  /**
   * Returns whether the toggle actually happened, but deliberately does NOT reload anything
   * itself: CaregiversListPage wants its list reloaded, CaregiverDetailPage wants its single
   * record reloaded, and only the caller knows which `load` that is. Each just does
   * `if (await toggleActive(row)) await load()` with its own `load`.
   */
  async function toggleActive(caregiver: Caregiver): Promise<boolean> {
    if (caregiver.isActive) {
      const confirmed = await confirm.ask({
        title: 'Deactivate caregiver?',
        message: `${caregiver.fullName} will no longer be assignable to new visits.`,
        confirmLabel: 'Deactivate',
        tone: 'danger',
      })
      if (!confirmed) return false
    }

    togglingId.value = caregiver.id
    try {
      if (caregiver.isActive) {
        await deactivateCaregiver(caregiver.id)
        toast.success(`${caregiver.fullName} deactivated.`)
      } else {
        await activateCaregiver(caregiver.id)
        toast.success(`${caregiver.fullName} activated.`)
      }
      return true
    } catch (err) {
      toast.error(getMessage(err))
      return false
    } finally {
      togglingId.value = null
    }
  }

  return {
    ...list,
    search,
    toggleActive,
    togglingId,
  }
}
