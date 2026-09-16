/**
 * Mirrors CareConnect.Commands.Auth.PasswordPolicy so the form states the same rule the API
 * enforces — a shorter password is rejected server-side regardless of what this file says.
 */
export const PASSWORD_MIN_LENGTH = 8

export const PASSWORD_MAX_LENGTH = 128
