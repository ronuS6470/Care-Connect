<script setup lang="ts">
import { computed, onMounted } from 'vue'

import DataTable, { type DataTableColumn } from '@/components/common/DataTable.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useAsyncData } from '@/composables/useAsyncData'
import { getAssignments } from '@/services/assignmentService'
import type { Assignment } from '@/types/assignment'
import { AssignmentStatus } from '@/types/enums'
import { formatDate } from '@/utils/date'
import { assignmentStatusLabel, assignmentStatusTone } from '@/utils/status'

// GET /api/assignments is row-scoped server-side to this caregiver's own assignments
// (GetAssignmentsQueryHandler passes the caller's identity through to the read repository) — no
// caregiverId needs to be passed or resolved.
const { data, loading, error, load } = useAsyncData(async () => (await getAssignments()).data)

onMounted(load)

const clients = computed(() => data.value ?? [])
const activeCount = computed(() => clients.value.filter((a) => a.status === AssignmentStatus.Active).length)

const columns: DataTableColumn<Assignment>[] = [
  { key: 'client', label: 'Client', value: (row) => row.clientFullName },
  { key: 'status', label: 'Status' },
  { key: 'startDate', label: 'Since', value: (row) => formatDate(row.startDate) },
  { key: 'notes', label: 'Notes', value: (row) => row.notes || '—' },
]
</script>

<template>
  <div>
    <PageHeader title="My Clients" :description="`${activeCount} active assignment${activeCount === 1 ? '' : 's'}`" />

    <ErrorState v-if="error" class="card-base" :description="error" @retry="load" />

    <DataTable
      v-else
      :columns="columns"
      :rows="clients"
      :row-key="(row) => row.id"
      :loading="loading"
      empty-title="No clients assigned"
      empty-description="You don't have any client assignments yet."
    >
      <template #cell-status="{ row }">
        <StatusBadge :label="assignmentStatusLabel(row.status)" :tone="assignmentStatusTone(row.status)" />
      </template>
    </DataTable>
  </div>
</template>
