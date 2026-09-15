<script setup lang="ts">
import { computed, onMounted } from 'vue'

import AppCard from '@/components/common/AppCard.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useAsyncData } from '@/composables/useAsyncData'
import { useClientDashboard } from '@/composables/useDashboard'
import { getAssignments } from '@/services/assignmentService'
import { assignmentStatusLabel, assignmentStatusTone } from '@/utils/status'
import { formatDate } from '@/utils/date'
import { initials } from '@/utils/string'

// Deliberately built from two already client-scoped, pre-filtered sources — never
// GET /api/caregivers/{id}, which returns HourlyRate/DateOfBirth/etc. that a client must never
// see. The dashboard's AssignedCaregiverDto only ever carries fullName/phoneNumber, and
// GET /api/assignments is row-scoped server-side to this client's own assignments.
const { data: dashboard, loading: dashboardLoading, error: dashboardError, load: loadDashboard } = useClientDashboard()
const { data: assignments, loading: assignmentsLoading, error: assignmentsError, load: loadAssignments } = useAsyncData(() =>
  getAssignments().then((r) => r.data),
)

onMounted(() => {
  loadDashboard()
  loadAssignments()
})

const loading = computed(() => dashboardLoading.value || assignmentsLoading.value)
const error = computed(() => dashboardError.value || assignmentsError.value)

const caregivers = computed(() => {
  const contacts = dashboard.value?.assignedCaregivers ?? []
  const rows = assignments.value ?? []

  return contacts.map((contact) => ({
    ...contact,
    assignment: rows.find((a) => a.caregiverId === contact.caregiverId) ?? null,
  }))
})
</script>

<template>
  <div>
    <PageHeader title="My Caregiver" description="Your assigned caregiver and care arrangement." />

    <AppCard v-if="loading" :padded="false"><LoadingState class="py-16" /></AppCard>
    <AppCard v-else-if="error" :padded="false"><ErrorState :description="error!" @retry="() => { loadDashboard(); loadAssignments() }" /></AppCard>
    <AppCard v-else-if="caregivers.length === 0" :padded="false">
      <EmptyState title="No caregiver assigned yet" description="Once a caregiver is assigned to you, they'll appear here." />
    </AppCard>

    <div v-else class="grid gap-4 lg:grid-cols-2">
      <AppCard v-for="caregiver in caregivers" :key="caregiver.caregiverId">
        <div class="flex items-center gap-3">
          <span class="flex h-11 w-11 shrink-0 items-center justify-center rounded-full bg-brand-700 text-sm font-semibold text-white">
            {{ initials(caregiver.fullName) }}
          </span>
          <div class="min-w-0">
            <p class="truncate text-base font-semibold text-ink">{{ caregiver.fullName }}</p>
            <StatusBadge
              v-if="caregiver.assignment"
              class="mt-1"
              :label="assignmentStatusLabel(caregiver.assignment.status)"
              :tone="assignmentStatusTone(caregiver.assignment.status)"
            />
          </div>
        </div>

        <dl class="mt-5 space-y-4">
          <div>
            <dt class="text-xs font-medium text-ink-muted">Phone</dt>
            <dd class="mt-1 text-sm text-ink">{{ caregiver.phoneNumber ?? 'Not on file' }}</dd>
          </div>

          <div v-if="caregiver.assignment">
            <dt class="text-xs font-medium text-ink-muted">Service Period</dt>
            <dd class="mt-1 text-sm text-ink">
              {{ formatDate(caregiver.assignment.startDate) }} –
              {{ caregiver.assignment.endDate ? formatDate(caregiver.assignment.endDate) : 'Ongoing' }}
            </dd>
          </div>

          <div v-if="caregiver.assignment?.notes">
            <dt class="text-xs font-medium text-ink-muted">Notes</dt>
            <dd class="mt-1 text-sm text-ink">{{ caregiver.assignment.notes }}</dd>
          </div>
        </dl>
      </AppCard>
    </div>
  </div>
</template>
