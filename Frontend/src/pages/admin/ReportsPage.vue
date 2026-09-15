<script setup lang="ts">
import { computed, ref, watch } from 'vue'

import AppAlert from '@/components/common/AppAlert.vue'
import AppButton from '@/components/common/AppButton.vue'
import AppCard from '@/components/common/AppCard.vue'
import AppDateInput from '@/components/common/AppDateInput.vue'
import AppSelect from '@/components/common/AppSelect.vue'
import DataTable, { type DataTableColumn } from '@/components/common/DataTable.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import { useApiError } from '@/composables/useApiError'
import {
  getCaregiverEarningsReport,
  getCaregiverHoursReport,
  getClientsWithoutActiveCaregiverReport,
  getCompletedVisitsReport,
  getTodaysVisits,
  getVisitsPerCaregiverReport,
} from '@/services/reportService'
import type { SelectOption } from '@/types/common'
import { formatCurrency } from '@/utils/currency'
import { formatDate, formatHours, formatTime, toDateInputValue } from '@/utils/date'
import { visitStatusLabel } from '@/utils/status'

/** A flattened, display-ready row — each report's `fetch` maps its own typed DTO into this shape. */
type ReportRow = Record<string, string | number>

interface ReportColumn {
  key: string
  label: string
  align?: 'left' | 'right'
}

interface ReportDefinition {
  value: string
  label: string
  needsDateRange: boolean
  columns: ReportColumn[]
  fetch: (fromDate: string, toDate: string) => Promise<ReportRow[]>
}

// A curated subset of CareConnect.Controller.Controllers.ReportsController's ~13 endpoints —
// enough to cover visits, caregiver hours/earnings, and a data-quality check, without building a
// bespoke page per report. Add another entry here (plus a reportService method) to expose more.
const REPORTS: ReportDefinition[] = [
  {
    value: 'todays-visits',
    label: "Today's Visits",
    needsDateRange: false,
    columns: [
      { key: 'time', label: 'Time' },
      { key: 'client', label: 'Client' },
      { key: 'caregiver', label: 'Caregiver' },
      { key: 'status', label: 'Status' },
    ],
    fetch: async () =>
      (await getTodaysVisits()).map((v, i) => ({
        _id: i,
        time: `${formatTime(v.scheduledStartUtc)} – ${formatTime(v.scheduledEndUtc)}`,
        client: v.clientFullName,
        caregiver: v.caregiverFullName,
        status: visitStatusLabel(v.status),
      })),
  },
  {
    value: 'completed-visits',
    label: 'Completed Visits',
    needsDateRange: true,
    columns: [
      { key: 'date', label: 'Date' },
      { key: 'time', label: 'Time' },
      { key: 'client', label: 'Client' },
      { key: 'caregiver', label: 'Caregiver' },
    ],
    fetch: async (fromDate, toDate) =>
      (await getCompletedVisitsReport(fromDate, toDate)).map((v, i) => ({
        _id: i,
        date: formatDate(v.scheduledStartUtc),
        time: `${formatTime(v.scheduledStartUtc)} – ${formatTime(v.scheduledEndUtc)}`,
        client: v.clientFullName,
        caregiver: v.caregiverFullName,
      })),
  },
  {
    value: 'visits-per-caregiver',
    label: 'Visits per Caregiver',
    needsDateRange: true,
    columns: [
      { key: 'caregiver', label: 'Caregiver' },
      { key: 'total', label: 'Total', align: 'right' },
      { key: 'completed', label: 'Completed', align: 'right' },
      { key: 'cancelled', label: 'Cancelled', align: 'right' },
      { key: 'noShow', label: 'No-Show', align: 'right' },
    ],
    fetch: async (fromDate, toDate) =>
      (await getVisitsPerCaregiverReport(fromDate, toDate)).map((r) => ({
        _id: r.caregiverId,
        caregiver: r.caregiverFullName,
        total: r.totalVisits,
        completed: r.completedVisits,
        cancelled: r.cancelledVisits,
        noShow: r.noShowVisits,
      })),
  },
  {
    value: 'caregiver-hours',
    label: 'Caregiver Hours',
    needsDateRange: true,
    columns: [
      { key: 'caregiver', label: 'Caregiver' },
      { key: 'visits', label: 'Completed Visits', align: 'right' },
      { key: 'hours', label: 'Total Hours', align: 'right' },
    ],
    fetch: async (fromDate, toDate) =>
      (await getCaregiverHoursReport(fromDate, toDate)).map((r) => ({
        _id: r.caregiverId,
        caregiver: r.caregiverFullName,
        visits: r.completedVisitCount,
        hours: formatHours(r.totalHoursWorked),
      })),
  },
  {
    value: 'caregiver-earnings',
    label: 'Caregiver Earnings',
    needsDateRange: true,
    columns: [
      { key: 'caregiver', label: 'Caregiver' },
      { key: 'rate', label: 'Hourly Rate', align: 'right' },
      { key: 'hours', label: 'Total Hours', align: 'right' },
      { key: 'earnings', label: 'Total Earnings', align: 'right' },
    ],
    fetch: async (fromDate, toDate) =>
      (await getCaregiverEarningsReport(fromDate, toDate)).map((r) => ({
        _id: r.caregiverId,
        caregiver: r.caregiverFullName,
        rate: formatCurrency(r.hourlyRate),
        hours: formatHours(r.totalHoursWorked),
        earnings: formatCurrency(r.totalEarnings),
      })),
  },
  {
    value: 'clients-without-caregiver',
    label: 'Clients Without an Active Caregiver',
    needsDateRange: false,
    columns: [
      { key: 'client', label: 'Client' },
      { key: 'status', label: 'Client Status' },
    ],
    fetch: async () =>
      (await getClientsWithoutActiveCaregiverReport()).map((r) => ({
        _id: r.clientId,
        client: r.clientFullName,
        status: r.isActive ? 'Active (no caregiver assigned)' : 'Inactive',
      })),
  },
]

const reportOptions: SelectOption<string>[] = REPORTS.map((r) => ({ value: r.value, label: r.label }))

const selectedKey = ref(REPORTS[0]!.value)
const selectedReport = computed(() => REPORTS.find((r) => r.value === selectedKey.value)!)

const today = toDateInputValue(new Date())
const fromDate = ref(toDateInputValue(new Date(new Date().setDate(new Date().getDate() - 30))))
const toDate = ref(today)

const rows = ref<ReportRow[]>([])
const loading = ref(false)
const error = ref<string | null>(null)
const hasRun = ref(false)
const { getMessage } = useApiError()

const columns = computed<DataTableColumn<ReportRow>[]>(() =>
  selectedReport.value.columns.map((c) => ({ key: c.key, label: c.label, align: c.align })),
)

async function runReport() {
  loading.value = true
  error.value = null
  try {
    rows.value = await selectedReport.value.fetch(fromDate.value, toDate.value)
    hasRun.value = true
  } catch (err) {
    error.value = getMessage(err)
  } finally {
    loading.value = false
  }
}

watch(selectedKey, () => {
  rows.value = []
  hasRun.value = false
  error.value = null
})
</script>

<template>
  <div>
    <PageHeader title="Reports" description="Operational reports across visits, caregivers, and clients." />

    <AppCard class="mb-4">
      <div class="grid gap-4 sm:grid-cols-[2fr_1fr_1fr_auto] sm:items-end">
        <AppSelect v-model="selectedKey" label="Report" :options="reportOptions" />
        <AppDateInput v-if="selectedReport.needsDateRange" v-model="fromDate" label="From" :max="toDate" />
        <AppDateInput v-if="selectedReport.needsDateRange" v-model="toDate" label="To" :min="fromDate" :max="today" />
        <AppButton :loading="loading" @click="runReport">Run Report</AppButton>
      </div>
    </AppCard>

    <AppAlert v-if="error" tone="danger" class="mb-4">{{ error }}</AppAlert>

    <DataTable
      v-if="hasRun && !error"
      :columns="columns"
      :rows="rows"
      :row-key="(row) => row._id"
      :loading="loading"
      empty-title="No results"
      empty-description="This report returned no rows for the selected range."
    />
  </div>
</template>
