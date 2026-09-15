import { http } from './api'
import type { AdminDashboard, CaregiverDashboard, ClientDashboard } from '@/types/dashboard'

export async function getAdminDashboard(): Promise<AdminDashboard> {
  const { data } = await http.get<AdminDashboard>('/dashboard/admin')
  return data
}

export async function getCaregiverDashboard(): Promise<CaregiverDashboard> {
  const { data } = await http.get<CaregiverDashboard>('/dashboard/caregiver')
  return data
}

export async function getClientDashboard(): Promise<ClientDashboard> {
  const { data } = await http.get<ClientDashboard>('/dashboard/client')
  return data
}

export const dashboardService = {
  getAdmin: getAdminDashboard,
  getCaregiver: getCaregiverDashboard,
  getClient: getClientDashboard,
}
