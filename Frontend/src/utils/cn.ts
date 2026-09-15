type ClassValue = string | number | null | undefined | false | ClassValue[] | Record<string, boolean | undefined>

/**
 * Minimal class-name combinator (no clsx/tailwind-merge dependency — Tailwind CSS is the only
 * required styling tool, and component variants here never produce genuinely conflicting
 * utilities, so last-value-wins merging isn't needed).
 */
export function cn(...values: ClassValue[]): string {
  const classes: string[] = []

  for (const value of values) {
    if (!value) continue

    if (typeof value === 'string' || typeof value === 'number') {
      classes.push(String(value))
      continue
    }

    if (Array.isArray(value)) {
      const nested = cn(...value)
      if (nested) classes.push(nested)
      continue
    }

    for (const [key, enabled] of Object.entries(value)) {
      if (enabled) classes.push(key)
    }
  }

  return classes.join(' ')
}
