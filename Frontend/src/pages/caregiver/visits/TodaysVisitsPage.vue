<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'

import AppButton from '@/components/common/AppButton.vue'
import DataTable, { type DataTableColumn } from '@/components/common/DataTable.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useAsyncData } from '@/composables/useAsyncData'
import { getVisits } from '@/services/visitService'
import type { VisitSummary } from '@/types/visit'
import { formatTime, toDateInputValue } from '@/utils/date'
import { visitStatusLabel, visitStatusTone } from '@/utils/status'

const router = useRouter()

// GET /api/visits is already row-scoped server-side to this caregiver's own visits — restricting
// to today's date range is the only filtering this page needs to apply itself.
const today = toDateInputValue(new Date())
const { data, loading, error, load } = useAsyncData(
  async () => (await getVisits({ fromDate: today, toDate: today, pageSize: 100 })).data,
)

onMounted(load)

const columns: DataTableColumn<VisitSummary>[] = [
  { key: 'time', label: 'Time', value: (row) => `${formatTime(row.scheduledStartUtc)} – ${formatTime(row.scheduledEndUtc)}` },
  { key: 'client', label: 'Client' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions', align: 'right' },
]
</script>

<template>
  <div>
    <PageHeader title="Today's Visits" />

    <ErrorState v-if="error" class="card-base" :description="error" @retry="load" />

    <DataTable
      v-else
      :columns="columns"
      :rows="data ?? []"
      :row-key="(row) => row.visitId"
      :loading="loading"
      empty-title="No visits today"
      empty-description="You have no visits scheduled for today."
    >
      <template #cell-client="{ row }">{{ row.clientFullName }}</template>
      <template #cell-status="{ row }">
        <StatusBadge :label="visitStatusLabel(row.status)" :tone="visitStatusTone(row.status)" />
      </template>
      <template #cell-actions="{ row }">
        <AppButton size="sm" variant="ghost" @click="router.push(`/caregiver/visits/${row.visitId}`)">View</AppButton>
      </template>
    </DataTable>
  </div>
</template>
