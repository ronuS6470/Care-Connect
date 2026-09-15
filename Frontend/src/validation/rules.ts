/**
 * Plain, dependency-free validation rules (no yup/zod/vee-validate — not in the required stack).
 * A rule is `(value) => string | true`: a string is the error message, `true` means valid.
 * Pass an array of rules to AppInput/AppSelect/AppTextarea's `rules` prop; the field runs them
 * in order and shows the first failure.
 */
export type ValidationRule<T = unknown> = (value: T) => string | true

const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

export function required(message = 'This field is required.'): ValidationRule {
  return (value) => {
    if (value === null || value === undefined) return message
    if (typeof value === 'string' && value.trim().length === 0) return message
    return true
  }
}

export function email(message = 'Enter a valid email address.'): ValidationRule<string> {
  return (value) => (!value || EMAIL_PATTERN.test(value) ? true : message)
}

export function minLength(min: number, message = `Must be at least ${min} characters.`): ValidationRule<string> {
  return (value) => (!value || value.length >= min ? true : message)
}

export function maxLength(max: number, message = `Must be ${max} characters or fewer.`): ValidationRule<string> {
  return (value) => (!value || value.length <= max ? true : message)
}

export function numberRange(
  min: number,
  max: number,
  message = `Must be between ${min} and ${max}.`,
): ValidationRule<number | null | undefined> {
  return (value) => (value === null || value === undefined || (value >= min && value <= max) ? true : message)
}

export function pattern(regex: RegExp, message = 'Invalid format.'): ValidationRule<string> {
  return (value) => (!value || regex.test(value) ? true : message)
}

// Mirrors CareConnect.Commands.Validation.CommonValidationRules exactly (same regex, same
// wording) so a field never shows valid client-side only to be rejected by the API.
const US_PHONE_PATTERN = /^\+?1?[-.\s]?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$/
const US_ZIP_PATTERN = /^\d{5}(-\d{4})?$/

export function usPhoneNumber(message = 'Must be a valid US phone number.'): ValidationRule<string> {
  return pattern(US_PHONE_PATTERN, message)
}

export function usZipCode(message = 'Must be a valid US ZIP code (12345 or 12345-6789).'): ValidationRule<string> {
  return pattern(US_ZIP_PATTERN, message)
}

export function exactLength(length: number, message = `Must be exactly ${length} characters.`): ValidationRule<string> {
  return (value) => (!value || value.length === length ? true : message)
}

/** Runs a field's rules in order, returning the first error message or null if all pass. */
export function runRules<T>(value: T, rules: ValidationRule<T>[] = []): string | null {
  for (const rule of rules) {
    const result = rule(value)
    if (result !== true) return result
  }
  return null
}
