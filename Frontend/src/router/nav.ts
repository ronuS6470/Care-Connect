import type { NavLink } from '@/components/layout/NavItem.vue'

export const ADMIN_NAV: NavLink[] = [
  { label: 'Dashboard', to: '/admin/dashboard', icon: 'dashboard' },
  { label: 'Caregivers', to: '/admin/caregivers', icon: 'caregivers' },
  { label: 'Clients', to: '/admin/clients', icon: 'clients' },
  { label: 'Assignments', to: '/admin/assignments', icon: 'assignments' },
  { label: 'Visits', to: '/admin/visits', icon: 'visits' },
  { label: 'Care Tasks', to: '/admin/care-tasks', icon: 'careTasks' },
  { label: 'Reports', to: '/admin/reports', icon: 'reports' },
  { label: 'Users', to: '/admin/users', icon: 'users' },
]

export const CAREGIVER_NAV: NavLink[] = [
  { label: 'Dashboard', to: '/caregiver/dashboard', icon: 'dashboard' },
  { label: 'My Clients', to: '/caregiver/clients', icon: 'clients' },
  { label: "Today's Visits", to: '/caregiver/visits/today', icon: 'visits' },
  { label: 'Upcoming Visits', to: '/caregiver/visits/upcoming', icon: 'visits' },
  { label: 'Availability', to: '/caregiver/availability', icon: 'availability' },
  { label: 'Earnings', to: '/caregiver/earnings', icon: 'earnings' },
]

export const CLIENT_NAV: NavLink[] = [
  { label: 'Dashboard', to: '/client/dashboard', icon: 'dashboard' },
  { label: 'My Caregiver', to: '/client/caregiver', icon: 'caregivers' },
  { label: 'My Visits', to: '/client/visits', icon: 'visits' },
  { label: 'Profile', to: '/client/profile', icon: 'profile' },
]
