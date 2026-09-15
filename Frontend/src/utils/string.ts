export function initials(fullName: string | null | undefined): string {
  if (!fullName) return '?'

  const parts = fullName.trim().split(/\s+/).filter(Boolean)
  if (parts.length === 0) return '?'
  if (parts.length === 1) return parts[0]!.slice(0, 2).toUpperCase()

  return `${parts[0]![0]}${parts[parts.length - 1]![0]}`.toUpperCase()
}

export function capitalize(value: string): string {
  if (!value) return value
  return value[0]!.toUpperCase() + value.slice(1)
}

export function truncate(value: string, maxLength: number): string {
  if (value.length <= maxLength) return value
  return `${value.slice(0, Math.max(0, maxLength - 1)).trimEnd()}…`
}
