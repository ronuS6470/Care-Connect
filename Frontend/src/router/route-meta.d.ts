import type { BreadcrumbItem } from '@/types/common'
import type { UserRole } from '@/types/enums'

declare module 'vue-router' {
  interface RouteMeta {
    requiresAuth?: boolean
    guestOnly?: boolean
    roles?: UserRole[]
    /** Page title, used for document.title and breadcrumb/header display. */
    title?: string
    /** Full trail for AppHeader's Breadcrumb; last entry is the current page (no `to`). */
    breadcrumb?: BreadcrumbItem[]
  }
}

export {}
