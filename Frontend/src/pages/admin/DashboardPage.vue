<script setup lang="ts">
import { onMounted } from 'vue'

import AppCard from '@/components/common/AppCard.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import { useAsyncData } from '@/composables/useAsyncData'
import { getAdminDashboard } from '@/services/dashboard.service'
import { formatCurrency } from '@/utils/currency'

const { data, loading, error, load } = useAsyncData(getAdminDashboard)

onMounted(load)

const stats = (dashboard: NonNullable<typeof data.value>) => [
  { label: 'Total Clients', value: dashboard.totalClients, tone: 'brand' as const },
  { label: 'Active Caregivers', value: dashboard.activeCaregivers, tone: 'success' as const },
  { label: "Today's Visits", value: dashboard.todaysVisitCount, tone: 'info' as const },
  { label: 'Completed Today', value: dashboard.completedVisitsToday, tone: 'success' as const },
  { label: 'Pending Today', value: dashboard.pendingVisitsToday, tone: 'warning' as const },
  { label: 'Cancelled Today', value: dashboard.cancelledVisitsToday, tone: 'neutral' as const },
]

const toneClasses = {
  brand: 'bg-brand-50 text-brand-700 dark:bg-brand-500/10 dark:text-brand-300',
  success: 'bg-success-50 text-success-700 dark:bg-success-500/10 dark:text-success-500',
  info: 'bg-info-50 text-info-700 dark:bg-info-500/10 dark:text-info-500',
  warning: 'bg-warning-50 text-warning-700 dark:bg-warning-500/10 dark:text-warning-500',
  neutral: 'bg-surface-sunken text-ink-muted',
} as const
</script>

<template>
  <div>
    <PageHeader title="Admin Dashboard" description="An organization-wide snapshot of CareConnect activity." />

    <AppCard v-if="loading" :padded="false">
      <LoadingState class="py-16" label="Loading dashboard…" />
    </AppCard>

    <AppCard v-else-if="error" :padded="false">
      <ErrorState :description="error" @retry="load" />
    </AppCard>

    <template v-else-if="data">
      <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-6">
        <div v-for="stat in stats(data)" :key="stat.label" class="card-base p-4">
          <span :class="['inline-flex rounded-lg px-2 py-1 text-xs font-medium', toneClasses[stat.tone]]">{{ stat.label }}</span>
          <p class="mt-3 text-2xl font-semibold text-ink">{{ stat.value }}</p>
        </div>
      </div>

      <div class="mt-6 grid gap-4 sm:grid-cols-2">
        <AppCard title="Total Hours Worked" description="All-time, across every completed visit">
          <p class="text-3xl font-semibold text-ink">{{ data.totalHoursWorked.toFixed(2) }} <span class="text-base font-normal text-ink-muted">hrs</span></p>
        </AppCard>

        <AppCard title="Total Caregiver Earnings" description="All-time, at each caregiver's current rate">
          <p class="text-3xl font-semibold text-ink">{{ formatCurrency(data.totalCaregiverEarnings) }}</p>
        </AppCard>
      </div>
    </template>
  </div>
</template>
