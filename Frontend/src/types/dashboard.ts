import type { VisitNote, VisitSummary } from './visit'

/** Mirrors CareConnect.DTOs.Dashboards.AdminDashboardDto. */
export interface AdminDashboard {
  totalClients: number
  activeCaregivers: number
  todaysVisitCount: number
  completedVisitsToday: number
  pendingVisitsToday: number
  cancelledVisitsToday: number
  totalHoursWorked: number
  totalCaregiverEarnings: number
}

/** Mirrors CareConnect.DTOs.Dashboards.CaregiverDashboardDto. */
export interface CaregiverDashboard {
  todaysVisits: VisitSummary[]
  upcomingVisits: VisitSummary[]
  completedVisitCount: number
  totalHoursWorked: number
  actualEarnings: number
  estimatedUpcomingEarnings: number
}

/** Mirrors CareConnect.DTOs.Dashboards.AssignedCaregiverDto. */
export interface AssignedCaregiver {
  caregiverId: number
  fullName: string
  phoneNumber: string | null
}

/** Mirrors CareConnect.DTOs.Dashboards.ClientDashboardDto. */
export interface ClientDashboard {
  assignedCaregivers: AssignedCaregiver[]
  nextVisit: VisitSummary | null
  upcomingVisits: VisitSummary[]
  completedVisitCount: number
  recentVisitNotes: VisitNote[]
}
