<script setup lang="ts">
import { computed, onMounted } from 'vue'

import AppCard from '@/components/common/AppCard.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useClientDashboard } from '@/composables/useDashboard'
import { useAuthStore } from '@/stores/auth'
import { formatDateTime } from '@/utils/date'
import { initials } from '@/utils/string'
import { visitStatusLabel, visitStatusTone } from '@/utils/status'

const auth = useAuthStore()
const { data, loading, error, load } = useClientDashboard()

onMounted(load)

const firstName = computed(() => auth.fullName?.split(' ')[0] ?? 'there')
</script>

<template>
  <div>
    <PageHeader :title="`Welcome, ${firstName}`" description="Your care team, next visit, and recent notes." />

    <AppCard v-if="loading" :padded="false">
      <LoadingState class="py-16" label="Loading dashboard…" />
    </AppCard>

    <AppCard v-else-if="error" :padded="false">
      <ErrorState :description="error" @retry="load" />
    </AppCard>

    <template v-else-if="data">
      <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <AppCard class="sm:col-span-2">
          <p class="text-xs font-medium text-ink-muted">Next Visit</p>
          <div v-if="data.nextVisit" class="mt-2">
            <p class="text-sm font-medium text-ink">{{ data.nextVisit.caregiverFullName }}</p>
            <p class="mt-1 text-sm text-ink-muted">{{ formatDateTime(data.nextVisit.scheduledStartUtc) }}</p>
            <StatusBadge class="mt-3" :label="visitStatusLabel(data.nextVisit.status)" :tone="visitStatusTone(data.nextVisit.status)" />
          </div>
          <p v-else class="mt-2 text-sm text-ink-muted">No upcoming visit scheduled</p>
        </AppCard>

        <div class="card-base p-4">
          <p class="text-xs font-medium text-ink-muted">Upcoming Visits</p>
          <p class="mt-2 text-2xl font-semibold text-ink">{{ data.upcomingVisits.length }}</p>
        </div>

        <div class="card-base p-4">
          <p class="text-xs font-medium text-ink-muted">Completed Visits</p>
          <p class="mt-2 text-2xl font-semibold text-ink">{{ data.completedVisitCount }}</p>
        </div>
      </div>

      <div class="mt-4">
        <AppCard title="My Caregiver">
          <template #actions>
            <RouterLink to="/client/caregiver" class="text-sm font-medium text-brand-600 hover:text-brand-700">View details</RouterLink>
          </template>

          <ul v-if="data.assignedCaregivers.length" class="grid gap-3 sm:grid-cols-2">
            <li v-for="caregiver in data.assignedCaregivers" :key="caregiver.caregiverId" class="flex items-center gap-3 rounded-lg border border-border p-3">
              <span class="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-brand-700 text-xs font-semibold text-white">
                {{ initials(caregiver.fullName) }}
              </span>
              <div class="min-w-0">
                <p class="truncate text-sm font-medium text-ink">{{ caregiver.fullName }}</p>
                <p class="truncate text-xs text-ink-muted">{{ caregiver.phoneNumber ?? 'No phone on file' }}</p>
              </div>
            </li>
          </ul>
          <EmptyState v-else title="No caregiver assigned yet" />
        </AppCard>
      </div>

      <div class="mt-4 grid gap-4 lg:grid-cols-2">
        <AppCard title="Upcoming Visits">
          <template #actions>
            <RouterLink to="/client/visits" class="text-sm font-medium text-brand-600 hover:text-brand-700">View all</RouterLink>
          </template>

          <ul v-if="data.upcomingVisits.length" class="-mx-5 -my-5 divide-y divide-border">
            <li v-for="visit in data.upcomingVisits" :key="visit.visitId">
              <RouterLink
                :to="`/client/visits/${visit.visitId}`"
                class="flex items-center justify-between gap-3 px-5 py-3 hover:bg-surface-sunken"
              >
                <div class="min-w-0">
                  <p class="truncate text-sm font-medium text-ink">{{ visit.caregiverFullName }}</p>
                  <p class="text-xs text-ink-muted">{{ formatDateTime(visit.scheduledStartUtc) }}</p>
                </div>
                <StatusBadge :label="visitStatusLabel(visit.status)" :tone="visitStatusTone(visit.status)" />
              </RouterLink>
            </li>
          </ul>
          <EmptyState v-else title="No upcoming visits" />
        </AppCard>

        <AppCard title="Recent Visit Notes">
          <ul v-if="data.recentVisitNotes.length" class="-mx-5 -my-5 divide-y divide-border">
            <li v-for="note in data.recentVisitNotes" :key="note.id" class="px-5 py-3">
              <p class="text-sm text-ink">{{ note.content }}</p>
              <p class="mt-1 text-xs text-ink-muted">{{ note.authorFullName }} · {{ formatDateTime(note.createdAtUtc) }}</p>
            </li>
          </ul>
          <EmptyState v-else title="No visit notes yet" />
        </AppCard>
      </div>
    </template>
  </div>
</template>
