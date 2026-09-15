import { ref } from 'vue'

import { useServerPagedList } from './useServerPagedList'
import { getVisits } from '@/services/visitService'
import type { VisitStatus } from '@/types/enums'

export interface VisitFilters {
  search: string
  fromDate: string | null
  toDate: string | null
  caregiverId: number | null
  clientId: number | null
  status: VisitStatus | null
}

const EMPTY_FILTERS: VisitFilters = {
  search: '',
  fromDate: null,
  toDate: null,
  caregiverId: null,
  clientId: null,
  status: null,
}

/**
 * GET /api/visits does real server-side filtering on every dimension the Admin and Client visit
 * lists need (date range, caregiver, client, status, name search) — see useServerPagedList.
 * Both VisitsListPage.vue pages (admin/ and client/) use this; each only sets the subset of
 * `filters` it actually exposes UI for.
 */
export function useVisits(options: { pageSize?: number; initialFilters?: Partial<VisitFilters> } = {}) {
  const filters = ref<VisitFilters>({ ...EMPTY_FILTERS, ...options.initialFilters })

  const list = useServerPagedList((params) => getVisits(params), filters, { pageSize: options.pageSize ?? 20 })

  function clearFilters() {
    filters.value = { ...EMPTY_FILTERS }
  }

  return { ...list, filters, clearFilters }
}
