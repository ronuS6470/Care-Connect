import { fetchOrNull, http } from './api'
import type { CareTask, CareTaskPayload } from '@/types/careTask'
import type { PagedResponse } from '@/types/common'

/**
 * GET /api/care-tasks supports isActive but no free-text search — the list page fetches
 * everything in one call and filters/paginates client-side (see CareTasksPage.vue).
 */
export async function getCareTasks(page = 1, pageSize = 200): Promise<PagedResponse<CareTask>> {
  const { data } = await http.get<PagedResponse<CareTask>>('/care-tasks', { params: { page, pageSize } })
  return data
}

export async function getCareTaskById(id: number): Promise<CareTask | null> {
  return fetchOrNull(() => http.get<CareTask>(`/care-tasks/${id}`))
}

export async function createCareTask(payload: CareTaskPayload): Promise<number> {
  const { data } = await http.post<number>('/care-tasks', payload)
  return data
}

export async function updateCareTask(id: number, payload: CareTaskPayload): Promise<void> {
  await http.put(`/care-tasks/${id}`, payload)
}

/** The backend decides soft-deactivate vs. hard-delete internally (soft when referenced by a VisitTask). */
export async function deactivateCareTask(id: number): Promise<void> {
  await http.delete(`/care-tasks/${id}`)
}

export const careTaskService = {
  getAll: getCareTasks,
  getById: getCareTaskById,
  create: createCareTask,
  update: updateCareTask,
  deactivate: deactivateCareTask,
}
