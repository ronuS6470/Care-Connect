import { fetchOrNull, http } from './api'
import type { Assignment, CreateAssignmentPayload, UpdateAssignmentPayload } from '@/types/assignment'
import type { PagedResponse } from '@/types/common'

/**
 * GET /api/assignments supports a status filter but no free-text search — the list page fetches
 * everything in one call and filters/paginates client-side (see AssignmentsPage.vue).
 */
export async function getAssignments(page = 1, pageSize = 200): Promise<PagedResponse<Assignment>> {
  const { data } = await http.get<PagedResponse<Assignment>>('/assignments', { params: { page, pageSize } })
  return data
}

export async function getAssignmentById(id: number): Promise<Assignment | null> {
  return fetchOrNull(() => http.get<Assignment>(`/assignments/${id}`))
}

export async function createAssignment(payload: CreateAssignmentPayload): Promise<number> {
  const { data } = await http.post<number>('/assignments', payload)
  return data
}

export async function updateAssignment(id: number, payload: UpdateAssignmentPayload): Promise<void> {
  await http.put(`/assignments/${id}`, payload)
}

export async function cancelAssignment(id: number): Promise<void> {
  await http.post(`/assignments/${id}/cancel`)
}

export const assignmentService = {
  getAll: getAssignments,
  getById: getAssignmentById,
  create: createAssignment,
  update: updateAssignment,
  cancel: cancelAssignment,
}
