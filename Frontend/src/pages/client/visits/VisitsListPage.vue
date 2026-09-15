<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'

import AppButton from '@/components/common/AppButton.vue'
import AppCard from '@/components/common/AppCard.vue'
import AppSelect from '@/components/common/AppSelect.vue'
import DataTable, { type DataTableColumn } from '@/components/common/DataTable.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import Pagination from '@/components/common/Pagination.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useVisits } from '@/composables/useVisits'
import type { SelectOption } from '@/types/common'
import { VisitStatus } from '@/types/enums'
import type { VisitSummary } from '@/types/visit'
import { formatDate, formatTime, toDateInputValue } from '@/utils/date'
import { visitStatusLabel, visitStatusTone } from '@/utils/status'

// GET /api/visits is already row-scoped server-side to this client's own visits (see
// VisitDapperRepository's authFilterSql) — no clientId needs to be passed or resolved.
const router = useRouter()

const { rows, loading, error, load, filters, pagination } = useVisits({ pageSize: 15 })

// "View" is a page-specific convenience over the composable's raw filters — a client thinks in
// terms of Upcoming/History, not fromDate/status.
type ViewFilter = 'all' | 'upcoming' | 'completed'
const view = ref<ViewFilter>('all')

const viewOptions: SelectOption<ViewFilter>[] = [
  { value: 'all', label: 'All Visits' },
  { value: 'upcoming', label: 'Upcoming' },
  { value: 'completed', label: 'History (Completed)' },
]

// No `immediate: true` — the composable's initial filters already match the "all" view's shape,
// so this only needs to react to the client changing the view afterward.
watch(view, (next) => {
  filters.value = {
    search: '',
    fromDate: next === 'upcoming' ? toDateInputValue(new Date()) : null,
    toDate: null,
    caregiverId: null,
    clientId: null,
    status: next === 'completed' ? VisitStatus.Completed : null,
  }
})

onMounted(load)

const columns: DataTableColumn<VisitSummary>[] = [
  { key: 'date', label: 'Date', value: (row) => formatDate(row.scheduledStartUtc) },
  { key: 'time', label: 'Time', value: (row) => `${formatTime(row.scheduledStartUtc)} – ${formatTime(row.scheduledEndUtc)}` },
  { key: 'caregiver', label: 'Caregiver', value: (row) => row.caregiverFullName },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions', align: 'right' },
]
</script>

<template>
  <div>
    <PageHeader title="My Visits" description="Every visit scheduled with your care team." />

    <AppCard class="mb-4" :padded="true">
      <div class="max-w-xs">
        <AppSelect v-model="view" label="View" :options="viewOptions" />
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
        :empty-description="view === 'upcoming' ? 'You have no upcoming visits.' : view === 'completed' ? 'No completed visits yet.' : 'No visits on file yet.'"
      >
        <template #cell-status="{ row }">
          <StatusBadge :label="visitStatusLabel(row.status)" :tone="visitStatusTone(row.status)" />
        </template>

        <template #cell-actions="{ row }">
          <AppButton size="sm" variant="ghost" @click="router.push(`/client/visits/${row.visitId}`)">View</AppButton>
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
