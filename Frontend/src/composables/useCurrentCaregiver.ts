import { computed } from 'vue'

import { getCaregivers } from '@/services/caregiverService'
import { useAuthStore } from '@/stores/auth'

import { useAsyncData } from './useAsyncData'

/**
 * There's no "/me" endpoint for a caregiver to resolve their own CaregiverId — every
 * caregiver-scoped endpoint (availability, earnings) needs that id explicitly, but only the
 * User.Id is known from the session. GET /api/caregivers has no auth restriction beyond being
 * signed in and no filter params, so this fetches the list once and finds the row whose userId
 * matches — the same "fetch once, look up client-side" approach already used for this resource
 * elsewhere (it has no search/filter query params either).
 */
export function useCurrentCaregiver() {
  const auth = useAuthStore()
  const { data: caregivers, loading, error, load } = useAsyncData(() => getCaregivers().then((r) => r.data))

  const current = computed(() => caregivers.value?.find((c) => c.userId === auth.userId) ?? null)

  return { current, loading, error, load }
}
