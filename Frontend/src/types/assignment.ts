import type { AssignmentStatus } from './enums'

/** Mirrors CareConnect.DTOs.Assignments.AssignmentDto. */
export interface Assignment {
  id: number
  caregiverId: number
  caregiverFullName: string
  clientId: number
  clientFullName: string
  status: AssignmentStatus
  startDate: string
  endDate: string | null
  notes: string | null
}

/** Mirrors CareConnect.DTOs.Assignments.CreateAssignmentDto. */
export interface CreateAssignmentPayload {
  caregiverId: number
  clientId: number
  startDate: string
  endDate?: string | null
  notes?: string | null
}

/** Mirrors CareConnect.DTOs.Assignments.UpdateAssignmentDto. */
export interface UpdateAssignmentPayload {
  endDate?: string | null
  notes?: string | null
}
