/** Mirrors CareConnect.DTOs.CareTasks.CareTaskDto. */
export interface CareTask {
  id: number
  name: string
  description: string | null
  isActive: boolean
}

/** Mirrors CareConnect.DTOs.CareTasks.CreateCareTaskDto / UpdateCareTaskDto. */
export interface CareTaskPayload {
  name: string
  description?: string | null
}
