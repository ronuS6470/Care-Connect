import { useAsyncData } from './useAsyncData'
import { getAdminDashboard, getCaregiverDashboard, getClientDashboard } from '@/services/dashboardService'

/**
 * Three thin, role-specific wrappers rather than one generic `useDashboard(role)` — each backend
 * dashboard endpoint returns a genuinely different shape (AdminDashboard/CaregiverDashboard/
 * ClientDashboard), and a page always knows which one it wants; a single generic version would
 * only widen the return type to a union and force every consumer to narrow it back down.
 */

export function useAdminDashboard() {
  return useAsyncData(getAdminDashboard)
}

export function useCaregiverDashboard() {
  return useAsyncData(getCaregiverDashboard)
}

export function useClientDashboard() {
  return useAsyncData(getClientDashboard)
}
