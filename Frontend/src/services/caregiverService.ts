import { fetchOrNull, http } from './api'
import type { Caregiver, CreateCaregiverPayload, UpdateCaregiverPayload } from '@/types/caregiver'
import type { PagedResponse } from '@/types/common'

/**
 * GET /api/caregivers only supports page/pageSize — no free-text search, no isActive filter
 * (unlike Clients/CareTasks). List pages for this resource fetch everything in one call and
 * filter/paginate client-side; see pages/admin/caregivers/CaregiversListPage.vue.
 */
export async function getCaregivers(page = 1, pageSize = 200): Promise<PagedResponse<Caregiver>> {
  const { data } = await http.get<PagedResponse<Caregiver>>('/caregivers', { params: { page, pageSize } })
  return data
}

export async function getCaregiverById(id: number): Promise<Caregiver | null> {
  return fetchOrNull(() => http.get<Caregiver>(`/caregivers/${id}`))
}

export async function createCaregiver(payload: CreateCaregiverPayload): Promise<number> {
  const { data } = await http.post<number>('/caregivers', payload)
  return data
}

export async function updateCaregiver(id: number, payload: UpdateCaregiverPayload): Promise<void> {
  await http.put(`/caregivers/${id}`, payload)
}

export async function activateCaregiver(id: number): Promise<void> {
  await http.post(`/caregivers/${id}/activate`)
}

export async function deactivateCaregiver(id: number): Promise<void> {
  await http.post(`/caregivers/${id}/deactivate`)
}

export const caregiverService = {
  getAll: getCaregivers,
  getById: getCaregiverById,
  create: createCaregiver,
  update: updateCaregiver,
  activate: activateCaregiver,
  deactivate: deactivateCaregiver,
}
