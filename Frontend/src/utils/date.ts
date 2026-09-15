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
