import { ApiError } from '@/services/api'

const DEFAULT_MESSAGE = 'Something went wrong. Please try again.'

/**
 * Every page in this app catches a failed request and turns it into a friendly string with the
 * same one-liner: `err instanceof ApiError ? err.message : 'Something went wrong...'`. That
 * duplication is all this is — a single named place for it, so the fallback wording (and the
 * instanceof check itself) only lives once.
 */
export function useApiError() {
  function getMessage(err: unknown, fallback = DEFAULT_MESSAGE): string {
    return err instanceof ApiError ? err.message : fallback
  }

  return { getMessage }
}
