/**
 * Mirrors CareConnect.DTOs.Common.ApiResponse<T> exactly (Success/Message/Data/Errors,
 * camelCase over the wire). Every non-2xx response from the API — validation, not found,
 * forbidden, conflict, or an unexpected 500 — comes back in this shape.
 */
export interface ApiResponse<T = unknown> {
  success: boolean
  message?: string | null
  data?: T | null
  errors?: string[] | null
}

/** Mirrors CareConnect.DTOs.Common.PagedResponseDto<T>. */
export interface PagedResponse<T> {
  data: T[]
  currentPage: number
  pageSize: number
  totalRecords: number
  totalPages: number
}

export interface SelectOption<T = string> {
  label: string
  value: T
  disabled?: boolean
}

export interface BreadcrumbItem {
  label: string
  to?: string
}
