import type { UserRole } from '@/types/enums'

declare module 'vue-router' {
  interface RouteMeta {
    requiresAuth?: boolean
    guestOnly?: boolean
    roles?: UserRole[]
    title?: string
  }
}

export {}
