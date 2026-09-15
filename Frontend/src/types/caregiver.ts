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
  hireDate: string
  yearsOfExperience: number
  isActive: boolean
}

/**
 * Mirrors CareConnect.DTOs.Caregivers.CaregiverAvailabilityDto. dayOfWeek is .NET's DayOfWeek
 * enum (Sunday=0..Saturday=6, NOT ISO's Monday=0) — see DAY_OF_WEEK_OPTIONS in utils/date.ts.
 * startTime/endTime are TimeOnly, serialized as "HH:mm:ss".
 */
export interface CaregiverAvailability {
  id: number
  caregiverId: number
  dayOfWeek: number
  startTime: string
  endTime: string
  isActive: boolean
}

/** Mirrors CareConnect.DTOs.Caregivers.CreateCaregiverAvailabilityDto. */
export interface CreateCaregiverAvailabilityPayload {
  caregiverId: number
  dayOfWeek: number
  startTime: string
  endTime: string
}

/** Mirrors CareConnect.DTOs.Caregivers.UpdateCaregiverAvailabilityDto. */
export interface UpdateCaregiverAvailabilityPayload {
  dayOfWeek: number
  startTime: string
  endTime: string
  isActive: boolean
}
