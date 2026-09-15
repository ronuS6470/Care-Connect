import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'

import { useAuthStore } from '@/stores/auth'
import { ROLE_HOME_PATH, UserRole } from '@/types/enums'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    redirect: '/login',
  },
  {
    path: '/login',
    name: 'login',
    component: () => import('@/pages/auth/LoginPage.vue'),
    meta: { guestOnly: true },
  },
  {
    path: '/admin',
    component: () => import('@/layouts/AdminLayout.vue'),
    meta: { requiresAuth: true, roles: [UserRole.Admin] },
    children: [
      { path: '', redirect: '/admin/dashboard' },
      { path: 'dashboard', name: 'admin-dashboard', component: () => import('@/pages/admin/DashboardPage.vue') },
      { path: 'caregivers', component: () => import('@/pages/ComingSoonPage.vue'), meta: { title: 'Caregivers' } },
      { path: 'clients', component: () => import('@/pages/ComingSoonPage.vue'), meta: { title: 'Clients' } },
      { path: 'assignments', component: () => import('@/pages/ComingSoonPage.vue'), meta: { title: 'Assignments' } },
      { path: 'visits', component: () => import('@/pages/ComingSoonPage.vue'), meta: { title: 'Visits' } },
      { path: 'care-tasks', component: () => import('@/pages/ComingSoonPage.vue'), meta: { title: 'Care Tasks' } },
      { path: 'reports', component: () => import('@/pages/ComingSoonPage.vue'), meta: { title: 'Reports' } },
    ],
  },
  {
    path: '/caregiver',
    component: () => import('@/layouts/CaregiverLayout.vue'),
    meta: { requiresAuth: true, roles: [UserRole.Caregiver] },
    children: [
      { path: '', redirect: '/caregiver/dashboard' },
      { path: 'dashboard', name: 'caregiver-dashboard', component: () => import('@/pages/caregiver/DashboardPage.vue') },
      { path: 'visits', component: () => import('@/pages/ComingSoonPage.vue'), meta: { title: 'My Visits' } },
      { path: 'availability', component: () => import('@/pages/ComingSoonPage.vue'), meta: { title: 'My Availability' } },
      { path: 'earnings', component: () => import('@/pages/ComingSoonPage.vue'), meta: { title: 'My Earnings' } },
    ],
  },
  {
    path: '/client',
    component: () => import('@/layouts/ClientLayout.vue'),
    meta: { requiresAuth: true, roles: [UserRole.Client] },
    children: [
      { path: '', redirect: '/client/dashboard' },
      { path: 'dashboard', name: 'client-dashboard', component: () => import('@/pages/client/DashboardPage.vue') },
      { path: 'visits', component: () => import('@/pages/ComingSoonPage.vue'), meta: { title: 'My Visits' } },
      { path: 'caregiver', component: () => import('@/pages/ComingSoonPage.vue'), meta: { title: 'My Caregiver' } },
      { path: 'notes', component: () => import('@/pages/ComingSoonPage.vue'), meta: { title: 'Visit Notes' } },
    ],
  },
  {
    path: '/unauthorized',
    name: 'unauthorized',
    component: () => import('@/pages/UnauthorizedPage.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'not-found',
    component: () => import('@/pages/NotFoundPage.vue'),
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior: () => ({ top: 0 }),
})

router.beforeEach((to) => {
  const auth = useAuthStore()

  if (to.meta.guestOnly && auth.isAuthenticated) {
    return ROLE_HOME_PATH[auth.role!]
  }

  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return { path: '/login', query: { redirect: to.fullPath } }
  }

  const allowedRoles = to.meta.roles as UserRole[] | undefined
  if (allowedRoles && auth.role !== null && !allowedRoles.includes(auth.role)) {
    return '/unauthorized'
  }

  return true
})

export default router
