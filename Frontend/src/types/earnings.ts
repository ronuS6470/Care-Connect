/** Mirrors CareConnect.DTOs.Reporting.CaregiverEarningsVisitDto. */
export interface CaregiverEarningsVisit {
  visitId: number
  clientFullName: string
  checkInUtc: string
  checkOutUtc: string
  workedHours: number
  earnings: number
}

/** Mirrors CareConnect.DTOs.Reporting.CaregiverEarningsDto. */
export interface CaregiverEarnings {
  caregiverId: number
  caregiverFullName: string
  periodStart: string
  periodEnd: string
  hourlyRate: number
  totalHoursWorked: number
  totalEarnings: number
  completedVisitCount: number
  visits: CaregiverEarningsVisit[] | null
}
