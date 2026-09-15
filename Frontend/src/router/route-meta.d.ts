import type { BreadcrumbItem } from '@/types/common'
import type { UserRole } from '@/types/enums'

declare module 'vue-router' {
  interface RouteMeta {
    requiresAuth?: boolean
    guestOnly?: boolean
    roles?: UserRole[]
    /** Page title, shown by ComingSoonPage and available for <title>/analytics later. */
    title?: string
    /** Full trail for AppHeader's Breadcrumb; last entry is the current page (no `to`). */
    breadcrumb?: BreadcrumbItem[]
  }
}

export {}
