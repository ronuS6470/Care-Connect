import type { InvoiceStatus } from './enums'

/**
 * Mirrors CareConnect.Infrastructure.Entities.Invoice — there is no InvoiceDto, no controller,
 * and no AppService for it anywhere in the backend (only the EF entity + InvoiceConfiguration
 * exist), so there is currently nothing for an invoiceService to call. Modeled here so the shape
 * is ready the moment that endpoint exists; field names follow the same camelCase-over-JSON
 * convention every other type in this folder uses once such a DTO is added.
 */
export interface Invoice {
  id: number
  clientId: number
  visitId: number | null
  invoiceNumber: string
  amount: number
  status: InvoiceStatus
  issuedDate: string
  dueDate: string | null
  paidDate: string | null
}
