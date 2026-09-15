<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'

import AppButton from '@/components/common/AppButton.vue'
import DataTable, { type DataTableColumn } from '@/components/common/DataTable.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import Pagination from '@/components/common/Pagination.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useApiError } from '@/composables/useApiError'
import { usePagination } from '@/composables/usePagination'
import { getUpcomingVisits } from '@/services/visitService'
import type { VisitSummary } from '@/types/visit'
import { formatDate, formatTime } from '@/utils/date'
import { visitStatusLabel, visitStatusTone } from '@/utils/status'

const router = useRouter()

// GET /api/visits/upcoming is a dedicated, already-row-scoped "what's next" feed — no filters of
// its own, just server-side pagination.
const rows = ref<VisitSummary[]>([])
const loading = ref(false)
const error = ref<string | null>(null)
const pagination = usePagination({ pageSize: 15 })
const { getMessage } = useApiError()

async function load() {
  loading.value = true
  error.value = null
  try {
    const result = await getUpcomingVisits(pagination.page.value, pagination.pageSize.value)
    rows.value = result.data
    pagination.setTotal(result.totalRecords)
  } catch (err) {
    error.value = getMessage(err)
  } finally {
    loading.value = false
  }
}

onMounted(load)
watch(pagination.page, load)

const columns: DataTableColumn<VisitSummary>[] = [
  { key: 'date', label: 'Date', value: (row) => formatDate(row.scheduledStartUtc) },
  { key: 'time', label: 'Time', value: (row) => `${formatTime(row.scheduledStartUtc)} – ${formatTime(row.scheduledEndUtc)}` },
  { key: 'client', label: 'Client' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions', align: 'right' },
]
</script>

<template>
  <div>
    <PageHeader title="Upcoming Visits" />

    <ErrorState v-if="error" class="card-base" :description="error" @retry="load" />

    <template v-else>
      <DataTable
        :columns="columns"
        :rows="rows"
        :row-key="(row) => row.visitId"
        :loading="loading"
        empty-title="No upcoming visits"
        empty-description="You have no visits scheduled ahead."
      >
        <template #cell-client="{ row }">{{ row.clientFullName }}</template>
        <template #cell-status="{ row }">
          <StatusBadge :label="visitStatusLabel(row.status)" :tone="visitStatusTone(row.status)" />
        </template>
        <template #cell-actions="{ row }">
          <AppButton size="sm" variant="ghost" @click="router.push(`/caregiver/visits/${row.visitId}`)">View</AppButton>
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
