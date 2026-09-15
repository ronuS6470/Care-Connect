import { computed } from 'vue'

import { getAssignments } from '@/services/assignmentService'

import { useAsyncData } from './useAsyncData'

/**
 * Like useCurrentCaregiver, but deliberately NOT built the same way: GET /api/caregivers has no
 * auth restriction, so fetching it to find "mine" is safe (every caregiver's public profile is
 * already visible to any authenticated caller). GET /api/clients is the same, but "clients must
 * never see other clients" is explicit for this role, and a client's list row exposes address,
 * phone, and emergency contact — real PII the backend just doesn't restrict field-by-field.
 *
 * GET /api/assignments, by contrast, is already row-scoped server-side for a Client caller (every
 * row it returns has ClientId == the caller's own — see AssignmentDapperRepository's
 * authFilterSql) — so reading `clientId` off any returned row is safe by construction and never
 * transmits another client's data to this browser. The only gap: a client with zero assignments
 * yet has no row to read it from, and there's no backend endpoint to fall back to.
 */
export function useCurrentClientId() {
  const { data: assignments, loading, error, load } = useAsyncData(() => getAssignments().then((r) => r.data))

  const clientId = computed(() => assignments.value?.[0]?.clientId ?? null)

  return { clientId, loading, error, load }
}
