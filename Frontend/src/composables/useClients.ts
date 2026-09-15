import { ref } from 'vue'

import { useServerPagedList } from './useServerPagedList'
import { getClients } from '@/services/clientService'

export type ClientStatusFilter = 'all' | 'active' | 'inactive'

export interface ClientFilters {
  search: string
  status: ClientStatusFilter
}

/** GET /api/clients does real server-side search + isActive filtering — see useServerPagedList. */
export function useClients() {
  const filters = ref<ClientFilters>({ search: '', status: 'all' })

  const list = useServerPagedList(
    (params) =>
      getClients({
        page: params.page,
        pageSize: params.pageSize,
        search: params.search,
        isActive: params.status === 'all' ? null : params.status === 'active',
      }),
    filters,
    { pageSize: 10 },
  )

  return { ...list, filters }
}
