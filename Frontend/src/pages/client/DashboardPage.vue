<script setup lang="ts">
import { onMounted } from 'vue'

import AppCard from '@/components/common/AppCard.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useAsyncData } from '@/composables/useAsyncData'
import { getClientDashboard } from '@/services/dashboard.service'
import { formatDateTime } from '@/utils/date'
import { initials } from '@/utils/string'
import { visitStatusLabel, visitStatusTone } from '@/utils/status'

const { data, loading, error, load } = useAsyncData(getClientDashboard)

onMounted(load)
</script>

<template>
  <div>
    <PageHeader title="My Dashboard" description="Your care team, next visit, and recent notes." />

    <AppCard v-if="loading" :padded="false">
      <LoadingState class="py-16" label="Loading dashboard…" />
    </AppCard>

    <AppCard v-else-if="error" :padded="false">
      <ErrorState :description="error" @retry="load" />
    </AppCard>

    <template v-else-if="data">
      <div class="grid gap-4 lg:grid-cols-3">
        <AppCard title="Next Visit" class="lg:col-span-1">
          <div v-if="data.nextVisit">
            <p class="text-sm font-medium text-ink">{{ data.nextVisit.caregiverFullName }}</p>
            <p class="mt-1 text-sm text-ink-muted">{{ formatDateTime(data.nextVisit.scheduledStartUtc) }}</p>
            <StatusBadge class="mt-3" :label="visitStatusLabel(data.nextVisit.status)" :tone="visitStatusTone(data.nextVisit.status)" />
          </div>
          <EmptyState v-else title="No upcoming visit scheduled" />
        </AppCard>

        <AppCard title="My Caregivers" class="lg:col-span-2">
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

      <div class="mt-6 grid gap-4 lg:grid-cols-2">
        <AppCard title="Upcoming Visits">
          <ul v-if="data.upcomingVisits.length" class="-mx-5 -my-5 divide-y divide-border">
            <li v-for="visit in data.upcomingVisits" :key="visit.visitId" class="flex items-center justify-between gap-3 px-5 py-3">
              <div class="min-w-0">
                <p class="truncate text-sm font-medium text-ink">{{ visit.caregiverFullName }}</p>
                <p class="text-xs text-ink-muted">{{ formatDateTime(visit.scheduledStartUtc) }}</p>
              </div>
              <StatusBadge :label="visitStatusLabel(visit.status)" :tone="visitStatusTone(visit.status)" />
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
