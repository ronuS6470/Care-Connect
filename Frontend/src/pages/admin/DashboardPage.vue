<script setup lang="ts">
import { onMounted } from 'vue'

import AppButton from '@/components/common/AppButton.vue'
import AppCard from '@/components/common/AppCard.vue'
import DataTable, { type DataTableColumn } from '@/components/common/DataTable.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useAsyncData } from '@/composables/useAsyncData'
import { useAdminDashboard } from '@/composables/useDashboard'
import { getTodaysVisits } from '@/services/reportService'
import { formatCurrency } from '@/utils/currency'
import { formatDate, formatTime } from '@/utils/date'
import { visitStatusLabel, visitStatusTone, type BadgeTone } from '@/utils/status'
import type { VisitSummary } from '@/types/visit'

const dashboard = useAdminDashboard()
const todaysVisits = useAsyncData(getTodaysVisits)

onMounted(() => {
  dashboard.load()
  todaysVisits.load()
})

// Every number here is read straight off AdminDashboardDto — the backend already aggregates
// them (SQL SUM/COUNT in DashboardDapperRepository); this page only formats and labels them,
// never recomputes a total from row-level data.
const stats = (d: NonNullable<typeof dashboard.data.value>) => [
  { label: 'Total Clients', value: d.totalClients, tone: 'brand' as BadgeTone },
  { label: 'Active Caregivers', value: d.activeCaregivers, tone: 'success' as BadgeTone },
  { label: "Today's Visits", value: d.todaysVisitCount, tone: 'info' as BadgeTone },
  { label: 'Completed Visits', value: d.completedVisitsToday, tone: 'success' as BadgeTone },
  { label: 'Pending Visits', value: d.pendingVisitsToday, tone: 'warning' as BadgeTone },
  { label: 'Cancelled Visits', value: d.cancelledVisitsToday, tone: 'neutral' as BadgeTone },
]

const toneClasses: Record<BadgeTone, string> = {
  brand: 'bg-brand-50 text-brand-700 dark:bg-brand-500/10 dark:text-brand-300',
  success: 'bg-success-50 text-success-700 dark:bg-success-500/10 dark:text-success-500',
  info: 'bg-info-50 text-info-700 dark:bg-info-500/10 dark:text-info-500',
  warning: 'bg-warning-50 text-warning-700 dark:bg-warning-500/10 dark:text-warning-500',
  danger: 'bg-danger-50 text-danger-700 dark:bg-danger-500/10 dark:text-danger-500',
  neutral: 'bg-surface-sunken text-ink-muted',
}

const visitColumns: DataTableColumn<VisitSummary>[] = [
  { key: 'client', label: 'Client', value: (row) => row.clientFullName },
  { key: 'caregiver', label: 'Caregiver', value: (row) => row.caregiverFullName },
  { key: 'date', label: 'Date', value: (row) => formatDate(row.scheduledStartUtc) },
  { key: 'start', label: 'Start Time', value: (row) => formatTime(row.scheduledStartUtc) },
  { key: 'end', label: 'End Time', value: (row) => formatTime(row.scheduledEndUtc) },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions', align: 'right' },
]
</script>

<template>
  <div>
    <PageHeader title="Admin Dashboard" description="An organization-wide snapshot of CareConnect activity." />

    <AppCard v-if="dashboard.loading.value" :padded="false">
      <LoadingState class="py-16" label="Loading dashboard…" />
    </AppCard>

    <AppCard v-else-if="dashboard.error.value" :padded="false">
      <ErrorState :description="dashboard.error.value" @retry="dashboard.load" />
    </AppCard>

    <template v-else-if="dashboard.data.value">
      <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-6">
        <div v-for="stat in stats(dashboard.data.value)" :key="stat.label" class="card-base p-4">
          <span :class="['badge-base', toneClasses[stat.tone]]">{{ stat.label }}</span>
          <p class="mt-3 text-2xl font-semibold text-ink">{{ stat.value }}</p>
        </div>
      </div>

      <div class="mt-4 grid gap-4 sm:grid-cols-2">
        <AppCard title="Total Hours Worked" description="All-time, across every completed visit">
          <p class="text-3xl font-semibold text-ink">
            {{ dashboard.data.value.totalHoursWorked.toFixed(2) }}
            <span class="text-base font-normal text-ink-muted">hrs</span>
          </p>
        </AppCard>

        <AppCard title="Total Caregiver Earnings" description="All-time, at each caregiver's current rate">
          <p class="text-3xl font-semibold text-ink">{{ formatCurrency(dashboard.data.value.totalCaregiverEarnings) }}</p>
        </AppCard>
      </div>
    </template>

    <div class="mt-8">
      <h2 class="mb-3 text-sm font-semibold text-ink">Today's Visits</h2>

      <ErrorState
        v-if="todaysVisits.error.value"
        :description="todaysVisits.error.value"
        class="card-base"
        @retry="todaysVisits.load"
      />

      <DataTable
        v-else
        :columns="visitColumns"
        :rows="todaysVisits.data.value ?? []"
        :row-key="(row) => row.visitId"
        :loading="todaysVisits.loading.value"
        empty-title="No visits scheduled today"
        empty-description="Once a visit is scheduled for today, it will show up here."
      >
        <template #cell-status="{ row }">
          <StatusBadge :label="visitStatusLabel(row.status)" :tone="visitStatusTone(row.status)" />
        </template>

        <template #cell-actions="{ row }">
          <AppButton size="sm" variant="outline" @click="$router.push(`/admin/visits/${row.visitId}`)">View</AppButton>
        </template>
      </DataTable>
    </div>
  </div>
</template>
