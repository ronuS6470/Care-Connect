import type { VisitStatus } from './enums'

/** Mirrors CareConnect.DTOs.Visits.VisitTaskDto. */
export interface VisitTask {
  id: number
  visitId: number
  careTaskId: number
  careTaskName: string
  isCompleted: boolean
  completedAtUtc: string | null
  notes: string | null
}

/** Mirrors CareConnect.DTOs.Visits.VisitDto — full detail view. */
export interface Visit {
  id: number
  caregiverAssignmentId: number
  caregiverFullName: string
  clientFullName: string
  scheduledStartUtc: string
  scheduledEndUtc: string
  actualStartUtc: string | null
  actualEndUtc: string | null
  status: VisitStatus
  cancellationReason: string | null
  visitTasks: VisitTask[]
}

/** Mirrors CareConnect.DTOs.Reporting.VisitSummaryDto — lightweight row shape for lists/reports. */
export interface VisitSummary {
  visitId: number
  caregiverFullName: string
  clientFullName: string
  scheduledStartUtc: string
  scheduledEndUtc: string
  status: VisitStatus
}

/** Mirrors CareConnect.DTOs.Visits.VisitNoteDto. */
export interface VisitNote {
  id: number
  visitId: number
  authorUserId: number
  authorFullName: string
  content: string
  createdAtUtc: string
}

/** Mirrors CareConnect.DTOs.Visits.CreateVisitDto. */
export interface CreateVisitPayload {
  caregiverAssignmentId: number
  scheduledStartUtc: string
  scheduledEndUtc: string
  careTaskIds: number[]
}

/** Mirrors CareConnect.DTOs.Visits.CreateVisitNoteDto. */
export interface CreateVisitNotePayload {
  visitId: number
  content: string
}

/** Mirrors CareConnect.DTOs.Visits.UpdateVisitTaskDto. */
export interface UpdateVisitTaskPayload {
  notes: string | null
}
