import { http } from './api'
import type { PagedResponse } from '@/types/common'
import type { UserRole } from '@/types/enums'
import type { ResetUserPasswordPayload, UpdateUserRolePayload, User } from '@/types/user'

/**
 * CareConnect.Controller.Controllers.UsersController — Admin-only in full. GET /api/users supports
 * real server-side search (name/email), role, and active filtering.
 */

export interface GetUsersParams {
  page?: number
  pageSize?: number
  search?: string
  role?: UserRole | null
  isActive?: boolean | null
}

export async function getUsers(params: GetUsersParams = {}): Promise<PagedResponse<User>> {
  const { data } = await http.get<PagedResponse<User>>('/users', {
    params: {
      page: params.page ?? 1,
      pageSize: params.pageSize ?? 20,
      search: params.search || undefined,
      role: params.role ?? undefined,
      isActive: params.isActive ?? undefined,
    },
  })
  return data
}

/**
 * Takes effect at the user's next sign-in — their current access token keeps the old role claim
 * until it expires, because this API issues stateless JWTs with no revocation list.
 */
export async function updateUserRole(userId: number, payload: UpdateUserRolePayload): Promise<void> {
  await http.put(`/users/${userId}/role`, payload)
}

export async function activateUser(userId: number): Promise<void> {
  await http.post(`/users/${userId}/activate`)
}

/** Blocks the user's next sign-in; a token already issued to them keeps working until it expires. */
export async function deactivateUser(userId: number): Promise<void> {
  await http.post(`/users/${userId}/deactivate`)
}

/** Admin sets a password without knowing the current one — the point of a reset. */
export async function resetUserPassword(userId: number, payload: ResetUserPasswordPayload): Promise<void> {
  await http.post(`/users/${userId}/reset-password`, payload)
}

export const userService = {
  getAll: getUsers,
  updateRole: updateUserRole,
  activate: activateUser,
  deactivate: deactivateUser,
  resetPassword: resetUserPassword,
}
