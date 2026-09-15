/**
 * Every *Utc-suffixed field from the API is an ISO-8601 string in UTC (System.Text.Json's
 * default for DateTime). These format in the viewer's local timezone, which is what a
 * caregiver/client/admin actually wants to see.
 */

const dateFormatter = new Intl.DateTimeFormat(undefined, {
  month: 'short',
  day: 'numeric',
  year: 'numeric',
})

const dateTimeFormatter = new Intl.DateTimeFormat(undefined, {
  month: 'short',
  day: 'numeric',
  year: 'numeric',
  hour: 'numeric',
  minute: '2-digit',
})

const timeFormatter = new Intl.DateTimeFormat(undefined, {
  hour: 'numeric',
  minute: '2-digit',
})

const weekdayDateFormatter = new Intl.DateTimeFormat(undefined, {
  weekday: 'long',
  month: 'long',
  day: 'numeric',
})

export function formatDate(value: string | Date | null | undefined): string {
  if (!value) return '—'
  return dateFormatter.format(new Date(value))
}

export function formatDateTime(value: string | Date | null | undefined): string {
  if (!value) return '—'
  return dateTimeFormatter.format(new Date(value))
}

export function formatTime(value: string | Date | null | undefined): string {
  if (!value) return '—'
  return timeFormatter.format(new Date(value))
}

export function formatWeekdayDate(value: string | Date | null | undefined): string {
  if (!value) return '—'
  return weekdayDateFormatter.format(new Date(value))
}

/** Converts a Date/ISO string to the yyyy-MM-dd shape a native <input type="date"> expects. */
export function toDateInputValue(value: string | Date | null | undefined): string {
  if (!value) return ''
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return ''
  return date.toISOString().slice(0, 10)
}

/** Converts a Date/ISO string to the HH:mm shape a native <input type="time"> expects. */
export function toTimeInputValue(value: string | Date | null | undefined): string {
  if (!value) return ''
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return ''
  return date.toTimeString().slice(0, 5)
}

export function formatHours(hours: number | null | undefined): string {
  if (hours === null || hours === undefined) return '—'
  return `${hours.toFixed(2)} hrs`
}

/** Formats a bare TimeOnly string ("HH:mm" or "HH:mm:ss") without going through Date parsing. */
export function formatTimeOnly(value: string | null | undefined): string {
  if (!value) return '—'
  const [hourStr, minuteStr] = value.split(':')
  const hour = Number(hourStr)
  const minute = Number(minuteStr)
  if (Number.isNaN(hour) || Number.isNaN(minute)) return value

  const period = hour >= 12 ? 'PM' : 'AM'
  const twelveHour = hour % 12 === 0 ? 12 : hour % 12
  return `${twelveHour}:${String(minute).padStart(2, '0')} ${period}`
}

/**
 * .NET's DayOfWeek enum (Sunday=0..Saturday=6 — NOT ISO's Monday=0), which is what
 * CaregiverAvailabilityDto.DayOfWeek serializes as. Listed Monday-first for display since that's
 * how a weekly work schedule reads naturally; the underlying `value` is still .NET's numbering.
 */
export const DAY_OF_WEEK_OPTIONS: { value: number; label: string }[] = [
  { value: 1, label: 'Monday' },
  { value: 2, label: 'Tuesday' },
  { value: 3, label: 'Wednesday' },
  { value: 4, label: 'Thursday' },
  { value: 5, label: 'Friday' },
  { value: 6, label: 'Saturday' },
  { value: 0, label: 'Sunday' },
]

export function dayOfWeekLabel(value: number): string {
  return DAY_OF_WEEK_OPTIONS.find((d) => d.value === value)?.label ?? 'Unknown'
}

/**
 * Combines a yyyy-MM-dd date and HH:mm time (both read as the browser's local time, exactly what
 * the admin typed) into the UTC ISO-8601 string CreateVisitDto's ScheduledStartUtc/EndUtc expect.
 */
export function combineDateAndTimeToIso(date: string, time: string): string | null {
  if (!date || !time) return null

  const [year, month, day] = date.split('-').map(Number)
  const [hour, minute] = time.split(':').map(Number)
  if ([year, month, day, hour, minute].some((n) => n === undefined || Number.isNaN(n))) return null

  const local = new Date(year!, month! - 1, day!, hour!, minute!)
  return local.toISOString()
}
