import type { NavLink } from '@/components/layout/NavItem.vue'

export const ADMIN_NAV: NavLink[] = [
  { label: 'Dashboard', to: '/admin/dashboard', icon: 'dashboard' },
  { label: 'Caregivers', to: '/admin/caregivers', icon: 'caregivers' },
  { label: 'Clients', to: '/admin/clients', icon: 'clients' },
  { label: 'Assignments', to: '/admin/assignments', icon: 'assignments' },
  { label: 'Visits', to: '/admin/visits', icon: 'visits' },
  { label: 'Care Tasks', to: '/admin/care-tasks', icon: 'careTasks' },
  { label: 'Reports', to: '/admin/reports', icon: 'reports' },
]

export const CAREGIVER_NAV: NavLink[] = [
  { label: 'Dashboard', to: '/caregiver/dashboard', icon: 'dashboard' },
  { label: 'My Visits', to: '/caregiver/visits', icon: 'visits' },
  { label: 'My Availability', to: '/caregiver/availability', icon: 'availability' },
  { label: 'My Earnings', to: '/caregiver/earnings', icon: 'earnings' },
]

export const CLIENT_NAV: NavLink[] = [
  { label: 'Dashboard', to: '/client/dashboard', icon: 'dashboard' },
  { label: 'My Visits', to: '/client/visits', icon: 'visits' },
  { label: 'My Caregiver', to: '/client/caregiver', icon: 'caregivers' },
  { label: 'Visit Notes', to: '/client/notes', icon: 'notes' },
]
