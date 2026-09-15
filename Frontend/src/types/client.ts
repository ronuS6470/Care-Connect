/** Mirrors CareConnect.DTOs.Clients.ClientDto. */
export interface Client {
  id: number
  userId: number
  fullName: string
  email: string
  phoneNumber: string | null
  addressLine1: string
  addressLine2: string | null
  city: string
  state: string
  postalCode: string
  emergencyContactName: string | null
  emergencyContactPhone: string | null
  isActive: boolean
}

/** Mirrors CareConnect.DTOs.Clients.CreateClientDto. */
export interface CreateClientPayload {
  userId: number
  addressLine1: string
  addressLine2?: string | null
  city: string
  state: string
  postalCode: string
  emergencyContactName?: string | null
  emergencyContactPhone?: string | null
}

/** Mirrors CareConnect.DTOs.Clients.UpdateClientDto. */
export interface UpdateClientPayload {
  addressLine1: string
  addressLine2?: string | null
  city: string
  state: string
  postalCode: string
  emergencyContactName?: string | null
  emergencyContactPhone?: string | null
}
