<script setup lang="ts">
import { onMounted } from 'vue'

import AppCard from '@/components/common/AppCard.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useCaregiverDashboard } from '@/composables/useDashboard'
import { formatCurrency } from '@/utils/currency'
import { formatDateTime } from '@/utils/date'
import { visitStatusLabel, visitStatusTone } from '@/utils/status'

const { data, loading, error, load } = useCaregiverDashboard()

onMounted(load)
</script>

<template>
  <div>
    <PageHeader title="My Dashboard" description="Today's schedule, upcoming visits, and your earnings." />

    <AppCard v-if="loading" :padded="false">
      <LoadingState class="py-16" label="Loading dashboard…" />
    </AppCard>

    <AppCard v-else-if="error" :padded="false">
      <ErrorState :description="error" @retry="load" />
    </AppCard>

    <template v-else-if="data">
      <div class="grid grid-cols-2 gap-4 lg:grid-cols-4">
        <div class="card-base p-4">
          <p class="text-xs font-medium text-ink-muted">Completed Visits</p>
          <p class="mt-2 text-2xl font-semibold text-ink">{{ data.completedVisitCount }}</p>
        </div>
        <div class="card-base p-4">
          <p class="text-xs font-medium text-ink-muted">Total Hours Worked</p>
          <p class="mt-2 text-2xl font-semibold text-ink">{{ data.totalHoursWorked.toFixed(2) }}</p>
        </div>
        <div class="card-base p-4">
          <p class="text-xs font-medium text-ink-muted">Actual Earnings</p>
          <p class="mt-2 text-2xl font-semibold text-ink">{{ formatCurrency(data.actualEarnings) }}</p>
        </div>
        <div class="card-base p-4">
          <p class="text-xs font-medium text-ink-muted">Estimated Upcoming</p>
          <p class="mt-2 text-2xl font-semibold text-ink">{{ formatCurrency(data.estimatedUpcomingEarnings) }}</p>
        </div>
      </div>

      <div class="mt-6 grid gap-4 lg:grid-cols-2">
        <AppCard title="Today's Visits">
          <ul v-if="data.todaysVisits.length" class="divide-y divide-border -mx-5 -my-5">
            <li v-for="visit in data.todaysVisits" :key="visit.visitId">
              <RouterLink
                :to="`/caregiver/visits/${visit.visitId}`"
                class="flex items-center justify-between gap-3 px-5 py-3 hover:bg-surface-sunken"
              >
                <div class="min-w-0">
                  <p class="truncate text-sm font-medium text-ink">{{ visit.clientFullName }}</p>
                  <p class="text-xs text-ink-muted">{{ formatDateTime(visit.scheduledStartUtc) }}</p>
                </div>
                <StatusBadge :label="visitStatusLabel(visit.status)" :tone="visitStatusTone(visit.status)" />
              </RouterLink>
            </li>
          </ul>
          <EmptyState v-else title="No visits today" />
        </AppCard>

        <AppCard title="Upcoming Visits">
          <ul v-if="data.upcomingVisits.length" class="divide-y divide-border -mx-5 -my-5">
            <li v-for="visit in data.upcomingVisits" :key="visit.visitId">
              <RouterLink
                :to="`/caregiver/visits/${visit.visitId}`"
                class="flex items-center justify-between gap-3 px-5 py-3 hover:bg-surface-sunken"
              >
                <div class="min-w-0">
                  <p class="truncate text-sm font-medium text-ink">{{ visit.clientFullName }}</p>
                  <p class="text-xs text-ink-muted">{{ formatDateTime(visit.scheduledStartUtc) }}</p>
                </div>
                <StatusBadge :label="visitStatusLabel(visit.status)" :tone="visitStatusTone(visit.status)" />
              </RouterLink>
            </li>
          </ul>
          <EmptyState v-else title="No upcoming visits" />
        </AppCard>
      </div>
    </template>
  </div>
</template>
