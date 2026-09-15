<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'

import AppButton from '@/components/common/AppButton.vue'
import AppCard from '@/components/common/AppCard.vue'
import AppDateInput from '@/components/common/AppDateInput.vue'
import AppSelect from '@/components/common/AppSelect.vue'
import DataTable, { type DataTableColumn } from '@/components/common/DataTable.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import Pagination from '@/components/common/Pagination.vue'
import SearchInput from '@/components/common/SearchInput.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useVisits } from '@/composables/useVisits'
import { getCaregivers } from '@/services/caregiverService'
import { getClients } from '@/services/clientService'
import type { SelectOption } from '@/types/common'
import { VisitStatus, VISIT_STATUS_LABELS } from '@/types/enums'
import type { VisitSummary } from '@/types/visit'
import { formatDate, formatTime } from '@/utils/date'
import { visitStatusLabel, visitStatusTone } from '@/utils/status'

const router = useRouter()

const { rows, loading, error, load, filters, pagination, clearFilters } = useVisits({ pageSize: 15 })

const caregiverOptions = ref<SelectOption<number>[]>([])
const clientOptions = ref<SelectOption<number>[]>([])

const statusOptions: SelectOption<VisitStatus>[] = [
  { value: VisitStatus.Scheduled, label: VISIT_STATUS_LABELS[VisitStatus.Scheduled] },
  { value: VisitStatus.InProgress, label: VISIT_STATUS_LABELS[VisitStatus.InProgress] },
  { value: VisitStatus.Completed, label: VISIT_STATUS_LABELS[VisitStatus.Completed] },
  { value: VisitStatus.Cancelled, label: VISIT_STATUS_LABELS[VisitStatus.Cancelled] },
  { value: VisitStatus.NoShow, label: VISIT_STATUS_LABELS[VisitStatus.NoShow] },
]

async function loadFilterOptions() {
  try {
    const [caregivers, clients] = await Promise.all([getCaregivers(), getClients({ pageSize: 200 })])
    caregiverOptions.value = caregivers.data.map((c) => ({ value: c.id, label: c.fullName }))
    clientOptions.value = clients.data.map((c) => ({ value: c.id, label: c.fullName }))
  } catch {
    caregiverOptions.value = []
    clientOptions.value = []
  }
}

onMounted(() => {
  void loadFilterOptions()
  load()
})

const columns: DataTableColumn<VisitSummary>[] = [
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
    <PageHeader title="Visits" description="Every scheduled, in-progress, and completed visit.">
      <template #actions>
        <AppButton @click="router.push('/admin/visits/new')">New Visit</AppButton>
      </template>
    </PageHeader>

    <AppCard class="mb-4" title="Filters">
      <template #actions>
        <button type="button" class="text-sm font-medium text-brand-600 hover:text-brand-700" @click="clearFilters">
          Clear all
        </button>
      </template>

      <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <SearchInput v-model="filters.search" placeholder="Search by caregiver or client name…" class="lg:col-span-3" />
        <AppDateInput v-model="filters.fromDate" label="From Date" />
        <AppDateInput v-model="filters.toDate" label="To Date" :min="filters.fromDate || undefined" />
        <AppSelect v-model="filters.status" label="Status" placeholder="All statuses" :options="statusOptions" />
        <AppSelect v-model="filters.caregiverId" label="Caregiver" placeholder="All caregivers" :options="caregiverOptions" />
        <AppSelect v-model="filters.clientId" label="Client" placeholder="All clients" :options="clientOptions" />
      </div>
    </AppCard>

    <ErrorState v-if="error" class="card-base" :description="error" @retry="load" />

    <template v-else>
      <DataTable
        :columns="columns"
        :rows="rows"
        :row-key="(row) => row.visitId"
        :loading="loading"
        empty-title="No visits found"
        empty-description="Try adjusting the filters above, or schedule a new visit."
      >
        <template #cell-status="{ row }">
          <StatusBadge :label="visitStatusLabel(row.status)" :tone="visitStatusTone(row.status)" />
        </template>

        <template #cell-actions="{ row }">
          <AppButton size="sm" variant="ghost" @click="router.push(`/admin/visits/${row.visitId}`)">View</AppButton>
        </template>
      </DataTable>

      <Pagination
        v-if="pagination.totalPages.value > 1"
        class="mt-4"
        :page="pagination.page.value"
        :total-pages="pagination.totalPages.value"
        :total-records="pagination.totalRecords.value"
        :page-size="pagination.pageSize.value"
        @update:page="pagination.goTo"
      />
    </template>
  </div>
</template>
