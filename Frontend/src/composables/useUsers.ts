import { ref } from 'vue'

import { useApiError } from './useApiError'
import { useServerPagedList } from './useServerPagedList'
import { useToast } from './useToast'
import { activateUser, deactivateUser, getUsers } from '@/services/userService'
import type { UserRole } from '@/types/enums'
import type { User } from '@/types/user'

/** 'all' rather than null, because AppSelect binds string/number values only. */
export type UserStatusFilter = 'all' | 'active' | 'inactive'
export type UserRoleFilter = 'all' | UserRole

export interface UserFilters {
  search: string
  role: UserRoleFilter
  status: UserStatusFilter
}

const EMPTY_FILTERS: UserFilters = { search: '', role: 'all', status: 'all' }

/**
 * GET /api/users does real server-side search/role/active filtering, so this pages and filters on
 * the server (see useServerPagedList) rather than fetching everything and slicing in memory.
 */
export function useUsers(options: { pageSize?: number } = {}) {
  const filters = ref<UserFilters>({ ...EMPTY_FILTERS })

  const list = useServerPagedList(
    (params) =>
      getUsers({
        page: params.page,
        pageSize: params.pageSize,
        search: params.search,
        role: params.role === 'all' ? null : params.role,
        isActive: params.status === 'all' ? null : params.status === 'active',
      }),
    filters,
    { pageSize: options.pageSize ?? 20 },
  )

  const toast = useToast()
  const { getMessage } = useApiError()
  const togglingId = ref<number | null>(null)

  /**
   * Returns true when the account's state actually changed, so the caller decides what to reload.
   * The backend refuses to let an admin deactivate themselves (409) — that message is surfaced
   * as-is rather than being second-guessed here.
   */
  async function toggleActive(user: User): Promise<boolean> {
    togglingId.value = user.id
    try {
      if (user.isActive) {
        await deactivateUser(user.id)
        toast.success(`${user.fullName} can no longer sign in.`)
      } else {
        await activateUser(user.id)
        toast.success(`${user.fullName} can sign in again.`)
      }
      return true
    } catch (err) {
      toast.error(getMessage(err))
      return false
    } finally {
      togglingId.value = null
    }
  }

  return { ...list, filters, toggleActive, togglingId }
}
