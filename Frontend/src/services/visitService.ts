import { fetchOrNull, http } from './api'
import type { PagedResponse } from '@/types/common'
import type { VisitStatus } from '@/types/enums'
import type { CreateVisitPayload, Visit, VisitNote, VisitSummary } from '@/types/visit'

export interface GetVisitsParams {
  page?: number
  pageSize?: number
  fromDate?: string | null
  toDate?: string | null
  caregiverId?: number | null
  clientId?: number | null
  status?: VisitStatus | null
  search?: string
}

/** GET /api/visits supports real server-side date range, caregiver/client, status, and free-text (name) filtering. */
export async function getVisits(params: GetVisitsParams = {}): Promise<PagedResponse<VisitSummary>> {
  const { data } = await http.get<PagedResponse<VisitSummary>>('/visits', {
    params: {
      page: params.page ?? 1,
      pageSize: params.pageSize ?? 20,
      fromDate: params.fromDate || undefined,
      toDate: params.toDate || undefined,
      caregiverId: params.caregiverId ?? undefined,
      clientId: params.clientId ?? undefined,
      status: params.status ?? undefined,
      search: params.search || undefined,
    },
  })
  return data
}

export async function getVisitById(id: number): Promise<Visit | null> {
  return fetchOrNull(() => http.get<Visit>(`/visits/${id}`))
}

/**
 * GET /api/visits/upcoming — a dedicated, unfiltered "what's next" feed (distinct from GET
 * /api/visits, which supports the full filter set). Same row-scoping as every other visits read:
 * a Caregiver only ever sees their own.
 */
export async function getUpcomingVisits(page = 1, pageSize = 20): Promise<PagedResponse<VisitSummary>> {
  const { data } = await http.get<PagedResponse<VisitSummary>>('/visits/upcoming', { params: { page, pageSize } })
  return data
}

export async function createVisit(payload: CreateVisitPayload): Promise<number> {
  const { data } = await http.post<number>('/visits', payload)
  return data
}

export async function getVisitNotes(visitId: number): Promise<VisitNote[]> {
  const { data } = await http.get<VisitNote[]>('/visit-notes', { params: { visitId } })
  return data
}

/** Admin or the assigned Caregiver only — a Client is read-only for notes (see AddVisitNoteCommandHandler). */
export async function addVisitNote(visitId: number, content: string): Promise<number> {
  const { data } = await http.post<number>('/visit-notes', { visitId, content })
  return data
}

/** Notes only — completion is a separate status transition (completeVisitTask/uncompleteVisitTask). */
export async function updateVisitTask(visitId: number, taskId: number, notes: string | null): Promise<void> {
  await http.put(`/visits/${visitId}/tasks/${taskId}`, { notes })
}

export async function completeVisitTask(visitId: number, taskId: number): Promise<void> {
  await http.post(`/visits/${visitId}/tasks/${taskId}/complete`)
}

export async function uncompleteVisitTask(visitId: number, taskId: number): Promise<void> {
  await http.post(`/visits/${visitId}/tasks/${taskId}/uncomplete`)
}

/**
 * check-in/check-out/complete are Caregiver-only (CareConnect.Controller.Controllers.VisitsController
 * gates all three with [Authorize(Roles = "Caregiver")]) and each returns 204 No Content — the new
 * status/timestamps are never in the response, only ever read back via a fresh getVisitById call.
 * Whether a transition is even allowed (status, timing, ownership, incomplete tasks) is decided
 * entirely server-side; these are thin, unconditional calls.
 */

export async function checkInVisit(id: number): Promise<void> {
  await http.post(`/visits/${id}/check-in`)
}

export async function checkOutVisit(id: number): Promise<void> {
  await http.post(`/visits/${id}/check-out`)
}

export async function completeVisit(id: number, incompleteTasksReason?: string | null): Promise<void> {
  await http.post(`/visits/${id}/complete`, { incompleteTasksReason: incompleteTasksReason || null })
}

export const visitService = {
  getAll: getVisits,
  getById: getVisitById,
  getUpcoming: getUpcomingVisits,
  create: createVisit,
  getNotes: getVisitNotes,
  addNote: addVisitNote,
  updateTask: updateVisitTask,
  completeTask: completeVisitTask,
  uncompleteTask: uncompleteVisitTask,
  checkIn: checkInVisit,
  checkOut: checkOutVisit,
  complete: completeVisit,
}
