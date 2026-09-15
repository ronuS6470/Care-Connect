import { ref, watch } from 'vue'

import { useApiError } from './useApiError'
import { useCurrentCaregiver } from './useCurrentCaregiver'
import { getCaregiverEarnings } from '@/services/reportService'
import type { CaregiverEarnings } from '@/types/earnings'
import { toDateInputValue } from '@/utils/date'

function firstOfMonth(): string {
  const now = new Date()
  return toDateInputValue(new Date(now.getFullYear(), now.getMonth(), 1))
}

/**
 * The backend computes every figure here (SUM/COUNT over actual check-in/check-out, at the
 * caregiver's HourlyRate) — this composable only fetches and re-fetches on a date-range change,
 * it never sums hours or multiplies a rate itself; EarningsPage.vue just formats and tabulates
 * what comes back.
 */
export function useEarnings() {
  const { current: caregiver, loading: caregiverLoading, error: caregiverError, load: loadCaregiver } = useCurrentCaregiver()

  const filters = ref({ fromDate: firstOfMonth(), toDate: toDateInputValue(new Date()) })

  const earnings = ref<CaregiverEarnings | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const { getMessage } = useApiError()

  async function load() {
    if (!caregiver.value) return

    loading.value = true
    error.value = null
    try {
      earnings.value = await getCaregiverEarnings(caregiver.value.id, filters.value.fromDate, filters.value.toDate, true)
    } catch (err) {
      error.value = getMessage(err)
    } finally {
      loading.value = false
    }
  }

  async function init() {
    await loadCaregiver()
    await load()
  }

  watch(
    () => [filters.value.fromDate, filters.value.toDate],
    () => {
      if (filters.value.fromDate && filters.value.toDate) load()
    },
  )

  return { caregiver, caregiverLoading, caregiverError, loadCaregiver, filters, earnings, loading, error, load, init }
}
