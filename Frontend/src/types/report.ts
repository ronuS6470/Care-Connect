/** Mirrors CareConnect.DTOs.Reports.VisitsPerCaregiverReportRowDto. */
export interface VisitsPerCaregiverReportRow {
  caregiverId: number
  caregiverFullName: string
  totalVisits: number
  completedVisits: number
  cancelledVisits: number
  noShowVisits: number
}

/** Mirrors CareConnect.DTOs.Reports.CaregiverHoursReportRowDto. */
export interface CaregiverHoursReportRow {
  caregiverId: number
  caregiverFullName: string
  completedVisitCount: number
  totalHoursWorked: number
}

/** Mirrors CareConnect.DTOs.Reports.CaregiverEarningsReportRowDto. */
export interface CaregiverEarningsReportRow {
  caregiverId: number
  caregiverFullName: string
  hourlyRate: number
  totalHoursWorked: number
  totalEarnings: number
}

/** Mirrors CareConnect.DTOs.Reports.ClientWithoutActiveCaregiverDto. */
export interface ClientWithoutActiveCaregiverRow {
  clientId: number
  clientFullName: string
  isActive: boolean
}
