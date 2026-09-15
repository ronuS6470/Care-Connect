/** Mirrors CareConnect.DTOs.Caregivers.CaregiverDto. Money as number (decimal over JSON), dates as ISO strings. */
export interface Caregiver {
  id: number
  userId: number
  fullName: string
  email: string
  phoneNumber: string | null
  licenseNumber: string | null
  hourlyRate: number
  hireDate: string
  dateOfBirth: string
  yearsOfExperience: number
  isActive: boolean
}

/** Mirrors CareConnect.DTOs.Caregivers.CreateCaregiverDto. */
export interface CreateCaregiverPayload {
  userId: number
  licenseNumber?: string | null
  hourlyRate: number
  hireDate: string
  dateOfBirth: string
  yearsOfExperience: number
}

/** Mirrors CareConnect.DTOs.Caregivers.UpdateCaregiverDto. */
export interface UpdateCaregiverPayload {
  licenseNumber?: string | null
  hourlyRate: number
  yearsOfExperience: number
}

/** Mirrors CareConnect.DTOs.Caregivers.CaregiverAvailabilityDto. */
export interface CaregiverAvailability {
  id: number
  caregiverId: number
  dayOfWeek: number
  startTime: string
  endTime: string
  isActive: boolean
}
