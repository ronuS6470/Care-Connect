import { ref, watch, type Ref } from 'vue'

import { useApiError } from './useApiError'
import { usePagination, type UsePaginationOptions } from './usePagination'
import type { PagedResponse } from '@/types/common'

/**
 * Shared shape behind useClients and useVisits: their list endpoints (GET /api/clients,
 * GET /api/visits) do real server-side filtering and pagination, so — unlike
 * useClientFilteredList's resources — every filter change means a real refetch, not a
 * browser-side re-slice. `filters` is whatever that resource's own filter shape is; this
 * composable only knows how to merge in page/pageSize and call the fetcher.
 */
export function useServerPagedList<T, F extends object>(
  fetcher: (params: F & { page: number; pageSize: number }) => Promise<PagedResponse<T>>,
  filters: Ref<F>,
  options: UsePaginationOptions = {},
) {
  const rows = ref<T[]>([]) as Ref<T[]>
  const loading = ref(false)
  const error = ref<string | null>(null)
  const pagination = usePagination(options)
  const { getMessage } = useApiError()

  async function load() {
    loading.value = true
    error.value = null
    try {
      const result = await fetcher({
        ...filters.value,
        page: pagination.page.value,
        pageSize: pagination.pageSize.value,
      })
      rows.value = result.data
      pagination.setTotal(result.totalRecords)
    } catch (err) {
      error.value = getMessage(err)
    } finally {
      loading.value = false
    }
  }

  // Any filter change refetches from page 1; paging within the current filters just refetches.
  watch(
    filters,
    () => {
      pagination.reset()
      load()
    },
    { deep: true },
  )
  watch(pagination.page, load)

  return { rows, loading, error, pagination, load }
}
