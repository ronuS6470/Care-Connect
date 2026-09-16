import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'

import { useAuthStore } from '@/stores/auth'
import { ROLE_HOME_PATH, UserRole } from '@/types/enums'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    redirect: '/login',
  },

  // ---------------------------------------------------------------------------------------
  // Public
  // ---------------------------------------------------------------------------------------
  {
    path: '/login',
    name: 'login',
    component: () => import('@/pages/auth/LoginPage.vue'),
    meta: { guestOnly: true, title: 'Sign In' },
  },
  {
    path: '/unauthorized',
    name: 'unauthorized',
    component: () => import('@/pages/UnauthorizedPage.vue'),
    meta: { requiresAuth: true, title: 'Unauthorized' },
  },
  {
    path: '/not-found',
    name: 'not-found',
    component: () => import('@/pages/NotFoundPage.vue'),
    meta: { title: 'Not Found' },
  },

  // ---------------------------------------------------------------------------------------
  // Admin
  // ---------------------------------------------------------------------------------------
  {
    path: '/admin',
    component: () => import('@/layouts/AdminLayout.vue'),
    meta: { requiresAuth: true, roles: [UserRole.Admin] },
    children: [
      { path: '', redirect: '/admin/dashboard' },
      {
        path: 'dashboard',
        name: 'admin-dashboard',
        component: () => import('@/pages/admin/DashboardPage.vue'),
        meta: { title: 'Dashboard', breadcrumb: [{ label: 'Dashboard' }] },
      },
      {
        path: 'caregivers',
        name: 'admin-caregivers',
        component: () => import('@/pages/admin/caregivers/CaregiversListPage.vue'),
        meta: { title: 'Caregivers', breadcrumb: [{ label: 'Caregivers' }] },
      },
      {
        path: 'caregivers/new',
        name: 'admin-caregivers-new',
        component: () => import('@/pages/admin/caregivers/CaregiverNewPage.vue'),
        meta: {
          title: 'New Caregiver',
          breadcrumb: [{ label: 'Caregivers', to: '/admin/caregivers' }, { label: 'New' }],
        },
      },
      {
        path: 'caregivers/:id',
        name: 'admin-caregiver-details',
        component: () => import('@/pages/admin/caregivers/CaregiverDetailPage.vue'),
        meta: {
          title: 'Caregiver Details',
          breadcrumb: [{ label: 'Caregivers', to: '/admin/caregivers' }, { label: 'Details' }],
        },
      },
      {
        path: 'caregivers/:id/edit',
        name: 'admin-caregiver-edit',
        component: () => import('@/pages/admin/caregivers/CaregiverEditPage.vue'),
        meta: {
          title: 'Edit Caregiver',
          breadcrumb: [{ label: 'Caregivers', to: '/admin/caregivers' }, { label: 'Edit' }],
        },
      },
      {
        path: 'clients',
        name: 'admin-clients',
        component: () => import('@/pages/admin/clients/ClientsListPage.vue'),
        meta: { title: 'Clients', breadcrumb: [{ label: 'Clients' }] },
      },
      {
        path: 'clients/new',
        name: 'admin-clients-new',
        component: () => import('@/pages/admin/clients/ClientNewPage.vue'),
        meta: {
          title: 'New Client',
          breadcrumb: [{ label: 'Clients', to: '/admin/clients' }, { label: 'New' }],
        },
      },
      {
        path: 'clients/:id',
        name: 'admin-client-details',
        component: () => import('@/pages/admin/clients/ClientDetailPage.vue'),
        meta: {
          title: 'Client Details',
          breadcrumb: [{ label: 'Clients', to: '/admin/clients' }, { label: 'Details' }],
        },
      },
      {
        path: 'clients/:id/edit',
        name: 'admin-client-edit',
        component: () => import('@/pages/admin/clients/ClientEditPage.vue'),
        meta: {
          title: 'Edit Client',
          breadcrumb: [{ label: 'Clients', to: '/admin/clients' }, { label: 'Edit' }],
        },
      },
      {
        path: 'assignments',
        name: 'admin-assignments',
        component: () => import('@/pages/admin/assignments/AssignmentsPage.vue'),
        meta: { title: 'Assignments', breadcrumb: [{ label: 'Assignments' }] },
      },
      {
        path: 'visits',
        name: 'admin-visits',
        component: () => import('@/pages/admin/visits/VisitsListPage.vue'),
        meta: { title: 'Visits', breadcrumb: [{ label: 'Visits' }] },
      },
      {
        path: 'visits/new',
        name: 'admin-visits-new',
        component: () => import('@/pages/admin/visits/VisitCreatePage.vue'),
        meta: {
          title: 'New Visit',
          breadcrumb: [{ label: 'Visits', to: '/admin/visits' }, { label: 'New' }],
        },
      },
      {
        path: 'visits/:id',
        name: 'admin-visit-details',
        component: () => import('@/pages/admin/visits/VisitDetailPage.vue'),
        meta: {
          title: 'Visit Details',
          breadcrumb: [{ label: 'Visits', to: '/admin/visits' }, { label: 'Details' }],
        },
      },
      {
        path: 'care-tasks',
        name: 'admin-care-tasks',
        component: () => import('@/pages/admin/careTasks/CareTasksPage.vue'),
        meta: { title: 'Care Tasks', breadcrumb: [{ label: 'Care Tasks' }] },
      },
      {
        path: 'reports',
        name: 'admin-reports',
        component: () => import('@/pages/admin/ReportsPage.vue'),
        meta: { title: 'Reports', breadcrumb: [{ label: 'Reports' }] },
      },
      {
        path: 'users',
        name: 'admin-users',
        component: () => import('@/pages/admin/users/UsersPage.vue'),
        meta: { title: 'Users', breadcrumb: [{ label: 'Users' }] },
      },
    ],
  },

  // ---------------------------------------------------------------------------------------
  // Caregiver
  // ---------------------------------------------------------------------------------------
  {
    path: '/caregiver',
    component: () => import('@/layouts/CaregiverLayout.vue'),
    meta: { requiresAuth: true, roles: [UserRole.Caregiver] },
    children: [
      { path: '', redirect: '/caregiver/dashboard' },
      {
        path: 'dashboard',
        name: 'caregiver-dashboard',
        component: () => import('@/pages/caregiver/DashboardPage.vue'),
        meta: { title: 'Dashboard', breadcrumb: [{ label: 'Dashboard' }] },
      },
      {
        path: 'clients',
        name: 'caregiver-clients',
        component: () => import('@/pages/caregiver/ClientsPage.vue'),
        meta: { title: 'My Clients', breadcrumb: [{ label: 'My Clients' }] },
      },
      {
        path: 'visits/today',
        name: 'caregiver-visits-today',
        component: () => import('@/pages/caregiver/visits/TodaysVisitsPage.vue'),
        meta: { title: "Today's Visits", breadcrumb: [{ label: 'Visits' }, { label: 'Today' }] },
      },
      {
        path: 'visits/upcoming',
        name: 'caregiver-visits-upcoming',
        component: () => import('@/pages/caregiver/visits/UpcomingVisitsPage.vue'),
        meta: { title: 'Upcoming Visits', breadcrumb: [{ label: 'Visits' }, { label: 'Upcoming' }] },
      },
      {
        path: 'visits/:id',
        name: 'caregiver-visit-details',
        component: () => import('@/pages/caregiver/visits/VisitDetailPage.vue'),
        meta: { title: 'Visit Details', breadcrumb: [{ label: 'Visits' }, { label: 'Details' }] },
      },
      {
        path: 'availability',
        name: 'caregiver-availability',
        component: () => import('@/pages/caregiver/AvailabilityPage.vue'),
        meta: { title: 'Availability', breadcrumb: [{ label: 'Availability' }] },
      },
      {
        path: 'earnings',
        name: 'caregiver-earnings',
        component: () => import('@/pages/caregiver/EarningsPage.vue'),
        meta: { title: 'Earnings', breadcrumb: [{ label: 'Earnings' }] },
      },
    ],
  },

  // ---------------------------------------------------------------------------------------
  // Client
  // ---------------------------------------------------------------------------------------
  {
    path: '/client',
    component: () => import('@/layouts/ClientLayout.vue'),
    meta: { requiresAuth: true, roles: [UserRole.Client] },
    children: [
      { path: '', redirect: '/client/dashboard' },
      {
        path: 'dashboard',
        name: 'client-dashboard',
        component: () => import('@/pages/client/DashboardPage.vue'),
        meta: { title: 'Dashboard', breadcrumb: [{ label: 'Dashboard' }] },
      },
      {
        path: 'caregiver',
        name: 'client-caregiver',
        component: () => import('@/pages/client/CaregiverPage.vue'),
        meta: { title: 'My Caregiver', breadcrumb: [{ label: 'My Caregiver' }] },
      },
      {
        path: 'visits',
        name: 'client-visits',
        component: () => import('@/pages/client/visits/VisitsListPage.vue'),
        meta: { title: 'My Visits', breadcrumb: [{ label: 'Visits' }] },
      },
      {
        path: 'visits/:id',
        name: 'client-visit-details',
        component: () => import('@/pages/client/visits/VisitDetailPage.vue'),
        meta: {
          title: 'Visit Details',
          breadcrumb: [{ label: 'Visits', to: '/client/visits' }, { label: 'Details' }],
        },
      },
      {
        path: 'profile',
        name: 'client-profile',
        component: () => import('@/pages/client/ProfilePage.vue'),
        meta: { title: 'Profile', breadcrumb: [{ label: 'Profile' }] },
      },
    ],
  },

  // ---------------------------------------------------------------------------------------
  // Catch-all
  // ---------------------------------------------------------------------------------------
  {
    path: '/:pathMatch(.*)*',
    redirect: '/not-found',
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior: () => ({ top: 0 }),
})

/**
 * Frontend route guards are a UX convenience only — they stop a legitimate user from landing on
 * a screen that has nothing to show them, and stop the app from ever sending an obviously
 * doomed request. They are NOT a security boundary: every one of these checks runs entirely in
 * the browser, from state (the JWT, its decoded role) the user fully controls. Every endpoint
 * behind these routes must independently enforce auth/role/ownership server-side (as
 * CareConnect.Controller's [Authorize(Roles=...)] + RequesterResolver row-level checks already
 * do) — a hidden nav link or a blocked client-side route is never a substitute for that.
 */
router.beforeEach((to) => {
  const auth = useAuthStore()

  if (to.meta.guestOnly && auth.isAuthenticated) {
    return ROLE_HOME_PATH[auth.role!]
  }

  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return { path: '/login', query: { redirect: to.fullPath } }
  }

  const allowedRoles = to.meta.roles
  if (allowedRoles && auth.role !== null && !allowedRoles.includes(auth.role)) {
    return '/unauthorized'
  }

  return true
})

router.afterEach((to) => {
  document.title = to.meta.title ? `${to.meta.title} · CareConnect` : 'CareConnect'
})

export default router
