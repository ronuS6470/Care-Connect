import { fetchOrNull, http } from './api'
import type { Client, CreateClientPayload, UpdateClientPayload } from '@/types/client'
import type { PagedResponse } from '@/types/common'

export interface GetClientsParams {
  page?: number
  pageSize?: number
  search?: string
  isActive?: boolean | null
}

/** GET /api/clients supports real server-side search + isActive filtering, unlike the other three resources. */
export async function getClients(params: GetClientsParams = {}): Promise<PagedResponse<Client>> {
  const { data } = await http.get<PagedResponse<Client>>('/clients', {
    params: {
      page: params.page ?? 1,
      pageSize: params.pageSize ?? 10,
      search: params.search || undefined,
      isActive: params.isActive ?? undefined,
    },
  })
  return data
}

export async function getClientById(id: number): Promise<Client | null> {
  return fetchOrNull(() => http.get<Client>(`/clients/${id}`))
}

export async function createClient(payload: CreateClientPayload): Promise<number> {
  const { data } = await http.post<number>('/clients', payload)
  return data
}

export async function updateClient(id: number, payload: UpdateClientPayload): Promise<void> {
  await http.put(`/clients/${id}`, payload)
}

export const clientService = {
  getAll: getClients,
  getById: getClientById,
  create: createClient,
  update: updateClient,
}
