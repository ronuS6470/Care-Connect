import { http } from './api'
import type { CaregiverEarnings } from '@/types/earnings'
import type {
  CaregiverEarningsReportRow,
  CaregiverHoursReportRow,
  ClientWithoutActiveCaregiverRow,
  VisitsPerCaregiverReportRow,
} from '@/types/report'
import type { VisitSummary } from '@/types/visit'

/**
 * Two distinct backend controllers behind one service: ReportsController (Admin-only operational
 * reports, a curated subset of its ~13 endpoints — see ReportsPage.vue) and ReportingController
 * (Admin/Caregiver earnings, self-service for a Caregiver).
 */

/** CareConnect.Controller.Controllers.ReportsController — Admin-only. */
export async function getTodaysVisits(): Promise<VisitSummary[]> {
  const { data } = await http.get<VisitSummary[]>('/reports/todays-visits')
  return data
}

export async function getCompletedVisitsReport(fromDate: string, toDate: string): Promise<VisitSummary[]> {
  const { data } = await http.get<VisitSummary[]>('/reports/completed-visits', { params: { fromDate, toDate } })
  return data
}

export async function getVisitsPerCaregiverReport(fromDate: string, toDate: string): Promise<VisitsPerCaregiverReportRow[]> {
  const { data } = await http.get<VisitsPerCaregiverReportRow[]>('/reports/visits-per-caregiver', { params: { fromDate, toDate } })
  return data
}

export async function getCaregiverHoursReport(fromDate: string, toDate: string): Promise<CaregiverHoursReportRow[]> {
  const { data } = await http.get<CaregiverHoursReportRow[]>('/reports/caregiver-hours', { params: { fromDate, toDate } })
  return data
}

export async function getCaregiverEarningsReport(fromDate: string, toDate: string): Promise<CaregiverEarningsReportRow[]> {
  const { data } = await http.get<CaregiverEarningsReportRow[]>('/reports/caregiver-earnings', { params: { fromDate, toDate } })
  return data
}

export async function getClientsWithoutActiveCaregiverReport(): Promise<ClientWithoutActiveCaregiverRow[]> {
  const { data } = await http.get<ClientWithoutActiveCaregiverRow[]>('/reports/clients-without-active-caregiver')
  return data
}

/**
 * CareConnect.Controller.Controllers.ReportingController — [Authorize(Roles="Admin,Caregiver")], a
 * Caregiver may only request their own caregiverId (enforced server-side by CaregiverEarningsAuthorizer).
 */
export async function getCaregiverEarnings(
  caregiverId: number,
  fromDate: string,
  toDate: string,
  includeVisitBreakdown = true,
): Promise<CaregiverEarnings> {
  const { data } = await http.get<CaregiverEarnings>(`/reporting/caregivers/${caregiverId}/earnings`, {
    params: { fromDate, toDate, includeVisitBreakdown },
  })
  return data
}

export const reportService = {
  getTodaysVisits,
  getCompletedVisitsReport,
  getVisitsPerCaregiverReport,
  getCaregiverHoursReport,
  getCaregiverEarningsReport,
  getClientsWithoutActiveCaregiverReport,
  getCaregiverEarnings,
}
