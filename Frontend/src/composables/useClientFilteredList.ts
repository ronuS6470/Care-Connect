import { computed, watch, type ComputedRef, type Ref } from 'vue'

import { useAsyncData } from './useAsyncData'
import { usePagination } from './usePagination'

/**
 * Shared shape behind useCaregivers/useAssignments/useCareTasks: their list endpoints
 * (GET /api/caregivers, /api/assignments, /api/care-tasks) support page/pageSize but no
 * free-text search, so each fetches its full set once and does search + pagination in the
 * browser — see the matching note in each services/*.ts file. This is that "fetch once, filter
 * client-side, paginate client-side" glue, written once instead of three times.
 *
 * The caller owns its own filter state (search text, a status toggle, ...) and passes it in as a
 * single reactive `predicate` — this composable only needs to know how to fetch and how to test
 * one row against the current filters.
 */
export function useClientFilteredList<T>(
  fetchAll: () => Promise<T[]>,
  predicate: ComputedRef<(row: T) => boolean> | Ref<(row: T) => boolean>,
  options: { pageSize?: number } = {},
) {
  const { data, loading, error, load } = useAsyncData(fetchAll)
  const pagination = usePagination({ pageSize: options.pageSize ?? 10 })

  const filtered = computed(() => (data.value ?? []).filter(predicate.value))

  // Re-page whenever the filtered set's size changes (a new search/status filter, or a reload) —
  // callers still own resetting `pagination.page` back to 1 when *they* change a filter value,
  // since only they know which of their refs should do that.
  watch(filtered, (filteredRows) => pagination.setTotal(filteredRows.length), { immediate: true })

  const rows = computed(() => {
    const start = (pagination.page.value - 1) * pagination.pageSize.value
    return filtered.value.slice(start, start + pagination.pageSize.value)
  })

  return { allRows: data, filtered, rows, loading, error, pagination, load }
}
