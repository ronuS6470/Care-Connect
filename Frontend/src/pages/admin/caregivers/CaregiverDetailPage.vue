<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'

import AppButton from '@/components/common/AppButton.vue'
import AppCard from '@/components/common/AppCard.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useAsyncData } from '@/composables/useAsyncData'
import { useCaregivers } from '@/composables/useCaregivers'
import { getCaregiverById } from '@/services/caregiverService'
import { formatCurrency } from '@/utils/currency'
import { formatDate } from '@/utils/date'
import { activeStatusTone } from '@/utils/status'

const route = useRoute()
const router = useRouter()

const id = computed(() => Number(route.params.id))
const { data: caregiver, loading, error, load } = useAsyncData(() => getCaregiverById(id.value))

onMounted(load)

// Only toggleActive/togglingId are used here — the list-fetching half of useCaregivers stays
// inert (nothing calls its own `load`), see the composable's own doc comment.
const { toggleActive, togglingId } = useCaregivers()

async function handleToggleActive() {
  if (!caregiver.value) return
  if (await toggleActive(caregiver.value)) await load()
}
</script>

<template>
  <div>
    <PageHeader :title="caregiver?.fullName ?? 'Caregiver'">
      <template v-if="caregiver" #actions>
        <AppButton variant="outline" :loading="togglingId === caregiver.id" @click="handleToggleActive">
          {{ caregiver.isActive ? 'Deactivate' : 'Activate' }}
        </AppButton>
        <AppButton @click="router.push(`/admin/caregivers/${caregiver.id}/edit`)">Edit</AppButton>
      </template>
    </PageHeader>

    <AppCard v-if="loading" :padded="false"><LoadingState class="py-16" /></AppCard>
    <AppCard v-else-if="error" :padded="false"><ErrorState :description="error" @retry="load" /></AppCard>
    <AppCard v-else-if="!caregiver" :padded="false">
      <EmptyState title="Caregiver not found" description="This caregiver may have been removed." />
    </AppCard>

    <AppCard v-else class="max-w-2xl">
      <dl class="grid gap-x-6 gap-y-5 sm:grid-cols-2">
        <div>
          <dt class="text-xs font-medium text-ink-muted">Status</dt>
          <dd class="mt-1"><StatusBadge :label="caregiver.isActive ? 'Active' : 'Inactive'" :tone="activeStatusTone(caregiver.isActive)" /></dd>
        </div>
        <div>
          <dt class="text-xs font-medium text-ink-muted">Email</dt>
          <dd class="mt-1 text-sm text-ink">{{ caregiver.email }}</dd>
        </div>
        <div>
          <dt class="text-xs font-medium text-ink-muted">Phone</dt>
          <dd class="mt-1 text-sm text-ink">{{ caregiver.phoneNumber ?? '—' }}</dd>
        </div>
        <div>
          <dt class="text-xs font-medium text-ink-muted">License Number</dt>
          <dd class="mt-1 text-sm text-ink">{{ caregiver.licenseNumber ?? '—' }}</dd>
        </div>
        <div>
          <dt class="text-xs font-medium text-ink-muted">Hourly Rate</dt>
          <dd class="mt-1 text-sm text-ink">{{ formatCurrency(caregiver.hourlyRate) }}</dd>
        </div>
        <div>
          <dt class="text-xs font-medium text-ink-muted">Years of Experience</dt>
          <dd class="mt-1 text-sm text-ink">{{ caregiver.yearsOfExperience }}</dd>
        </div>
        <div>
          <dt class="text-xs font-medium text-ink-muted">Hire Date</dt>
          <dd class="mt-1 text-sm text-ink">{{ formatDate(caregiver.hireDate) }}</dd>
        </div>
        <div>
          <dt class="text-xs font-medium text-ink-muted">Date of Birth</dt>
          <dd class="mt-1 text-sm text-ink">{{ formatDate(caregiver.dateOfBirth) }}</dd>
        </div>
      </dl>
    </AppCard>
  </div>
</template>
