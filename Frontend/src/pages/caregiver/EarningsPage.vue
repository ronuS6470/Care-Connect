<script setup lang="ts">
import { onMounted } from 'vue'

import AppCard from '@/components/common/AppCard.vue'
import AppDateInput from '@/components/common/AppDateInput.vue'
import DataTable, { type DataTableColumn } from '@/components/common/DataTable.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import { useEarnings } from '@/composables/useEarnings'
import type { CaregiverEarningsVisit } from '@/types/earnings'
import { formatCurrency } from '@/utils/currency'
import { formatDate, formatTime, toDateInputValue } from '@/utils/date'

const { caregiver, caregiverLoading, caregiverError, loadCaregiver, filters, earnings, loading, error, load, init } = useEarnings()

onMounted(init)

const columns: DataTableColumn<CaregiverEarningsVisit>[] = [
  { key: 'date', label: 'Date', value: (row) => formatDate(row.checkInUtc) },
  { key: 'client', label: 'Client', value: (row) => row.clientFullName },
  { key: 'checkIn', label: 'Check-In', value: (row) => formatTime(row.checkInUtc) },
  { key: 'checkOut', label: 'Check-Out', value: (row) => formatTime(row.checkOutUtc) },
  { key: 'hours', label: 'Worked Hours', value: (row) => row.workedHours.toFixed(2), align: 'right' },
  { key: 'rate', label: 'Hourly Rate', value: () => formatCurrency(earnings.value?.hourlyRate ?? 0), align: 'right' },
  { key: 'earnings', label: 'Earnings', value: (row) => formatCurrency(row.earnings), align: 'right' },
]
</script>

<template>
  <div>
    <PageHeader title="My Earnings" description="Earnings from completed visits, based on actual check-in/check-out time." />

    <AppCard v-if="caregiverLoading" :padded="false"><LoadingState class="py-16" /></AppCard>
    <AppCard v-else-if="caregiverError" :padded="false"><ErrorState :description="caregiverError" @retry="loadCaregiver" /></AppCard>
    <AppCard v-else-if="!caregiver" :padded="false">
      <p class="p-5 text-sm text-ink-muted">This account isn't linked to a caregiver profile.</p>
    </AppCard>

    <template v-else>
      <AppCard class="mb-4" title="Date Range">
        <div class="grid gap-4 sm:grid-cols-2 sm:max-w-md">
          <AppDateInput v-model="filters.fromDate" label="From Date" :max="filters.toDate" />
          <AppDateInput v-model="filters.toDate" label="To Date" :min="filters.fromDate" :max="toDateInputValue(new Date())" />
        </div>
      </AppCard>

      <ErrorState v-if="error" class="card-base" :description="error" @retry="load" />

      <template v-else-if="loading">
        <AppCard :padded="false"><LoadingState class="py-16" label="Loading earnings…" /></AppCard>
      </template>

      <template v-else-if="earnings">
        <div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
          <div class="card-base p-4">
            <p class="text-xs font-medium text-ink-muted">Total Hours</p>
            <p class="mt-2 text-2xl font-semibold text-ink">{{ earnings.totalHoursWorked.toFixed(2) }}</p>
          </div>
          <div class="card-base p-4">
            <p class="text-xs font-medium text-ink-muted">Total Earnings</p>
            <p class="mt-2 text-2xl font-semibold text-ink">{{ formatCurrency(earnings.totalEarnings) }}</p>
          </div>
          <div class="card-base p-4">
            <p class="text-xs font-medium text-ink-muted">Completed Visits</p>
            <p class="mt-2 text-2xl font-semibold text-ink">{{ earnings.completedVisitCount }}</p>
          </div>
        </div>

        <div class="mt-6">
          <h2 class="mb-3 text-sm font-semibold text-ink">Visit Breakdown</h2>
          <DataTable
            :columns="columns"
            :rows="earnings.visits ?? []"
            :row-key="(row) => row.visitId"
            empty-title="No completed visits in this range"
            empty-description="Try a wider date range."
          />
        </div>
      </template>
    </template>
  </div>
</template>
