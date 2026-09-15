import { http } from './api'
import type {
  CaregiverAvailability,
  CreateCaregiverAvailabilityPayload,
  UpdateCaregiverAvailabilityPayload,
} from '@/types/caregiver'

/**
 * GET is open to any authenticated role; Create/Update/Delete are [Authorize(Roles="Admin")]
 * only on the backend — a Caregiver calling them gets a real 403, which the UI surfaces like any
 * other backend response (see pages/caregiver/AvailabilityPage.vue).
 */

export async function getCaregiverAvailability(caregiverId: number): Promise<CaregiverAvailability[]> {
  const { data } = await http.get<CaregiverAvailability[]>('/caregiver-availability', { params: { caregiverId } })
  return data
}

export async function createCaregiverAvailability(payload: CreateCaregiverAvailabilityPayload): Promise<number> {
  const { data } = await http.post<number>('/caregiver-availability', payload)
  return data
}

export async function updateCaregiverAvailability(id: number, payload: UpdateCaregiverAvailabilityPayload): Promise<void> {
  await http.put(`/caregiver-availability/${id}`, payload)
}

export async function deleteCaregiverAvailability(id: number): Promise<void> {
  await http.delete(`/caregiver-availability/${id}`)
}

export const caregiverAvailabilityService = {
  getAll: getCaregiverAvailability,
  create: createCaregiverAvailability,
  update: updateCaregiverAvailability,
  delete: deleteCaregiverAvailability,
}
