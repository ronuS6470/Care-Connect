<script setup lang="ts">
import { onMounted, ref } from 'vue'

import AppButton from '@/components/common/AppButton.vue'
import AppSelect from '@/components/common/AppSelect.vue'
import AssignmentFormModal from '@/components/assignments/AssignmentFormModal.vue'
import DataTable, { type DataTableColumn } from '@/components/common/DataTable.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import Pagination from '@/components/common/Pagination.vue'
import SearchInput from '@/components/common/SearchInput.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useApiError } from '@/composables/useApiError'
import { useAssignments } from '@/composables/useAssignments'
import { useToast } from '@/composables/useToast'
import { createAssignment, updateAssignment } from '@/services/assignmentService'
import type { Assignment, CreateAssignmentPayload, UpdateAssignmentPayload } from '@/types/assignment'
import type { SelectOption } from '@/types/common'
import { AssignmentStatus, ASSIGNMENT_STATUS_LABELS } from '@/types/enums'
import { formatDate } from '@/utils/date'
import { assignmentStatusLabel, assignmentStatusTone } from '@/utils/status'

const { rows, loading, error, load, search, statusFilter, pagination, cancel, cancellingId } = useAssignments()

onMounted(load)

const statusFilterOptions: SelectOption<'all' | AssignmentStatus>[] = [
  { value: 'all', label: 'All statuses' },
  { value: AssignmentStatus.Active, label: ASSIGNMENT_STATUS_LABELS[AssignmentStatus.Active] },
  { value: AssignmentStatus.Completed, label: ASSIGNMENT_STATUS_LABELS[AssignmentStatus.Completed] },
  { value: AssignmentStatus.Cancelled, label: ASSIGNMENT_STATUS_LABELS[AssignmentStatus.Cancelled] },
]

const columns: DataTableColumn<Assignment>[] = [
  { key: 'caregiver', label: 'Caregiver', value: (row) => row.caregiverFullName },
  { key: 'client', label: 'Client', value: (row) => row.clientFullName },
  { key: 'start', label: 'Start Date', value: (row) => formatDate(row.startDate) },
  { key: 'end', label: 'End Date', value: (row) => (row.endDate ? formatDate(row.endDate) : '—') },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions', align: 'right' },
]

// Create/edit modal orchestration stays page-specific — the composable only owns the list and
// the one-shot "cancel" action, which is the part that was actually duplicated elsewhere.
const toast = useToast()
const { getMessage } = useApiError()

const modalOpen = ref(false)
const modalMode = ref<'create' | 'edit'>('create')
const editingAssignment = ref<Assignment | null>(null)
const submitting = ref(false)
const apiError = ref<string | null>(null)

function openCreate() {
  modalMode.value = 'create'
  editingAssignment.value = null
  apiError.value = null
  modalOpen.value = true
}

function openEdit(assignment: Assignment) {
  modalMode.value = 'edit'
  editingAssignment.value = assignment
  apiError.value = null
  modalOpen.value = true
}

async function handleSubmit(payload: CreateAssignmentPayload | UpdateAssignmentPayload) {
  submitting.value = true
  apiError.value = null
  try {
    if (modalMode.value === 'create') {
      await createAssignment(payload as CreateAssignmentPayload)
      toast.success('Assignment created.')
    } else if (editingAssignment.value) {
      await updateAssignment(editingAssignment.value.id, payload as UpdateAssignmentPayload)
      toast.success('Assignment updated.')
    }
    modalOpen.value = false
    await load()
  } catch (err) {
    apiError.value = getMessage(err)
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div>
    <PageHeader title="Assignments" description="Caregiver-to-client care assignments.">
      <template #actions>
        <AppButton @click="openCreate">New Assignment</AppButton>
      </template>
    </PageHeader>

    <div class="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center">
      <div class="max-w-xs flex-1">
        <SearchInput v-model="search" placeholder="Search by caregiver or client…" />
      </div>
      <div class="w-full sm:w-48">
        <AppSelect v-model="statusFilter" :options="statusFilterOptions" />
      </div>
    </div>

    <ErrorState v-if="error" class="card-base" :description="error" @retry="load" />

    <template v-else>
      <DataTable
        :columns="columns"
        :rows="rows"
        :row-key="(row) => row.id"
        :loading="loading"
        empty-title="No assignments found"
        :empty-description="search ? 'Try a different search.' : 'Create the first assignment to get started.'"
      >
        <template #cell-status="{ row }">
          <StatusBadge :label="assignmentStatusLabel(row.status)" :tone="assignmentStatusTone(row.status)" />
        </template>

        <template #cell-actions="{ row }">
          <div class="flex justify-end gap-2">
            <AppButton size="sm" variant="ghost" @click="openEdit(row)">Edit</AppButton>
            <AppButton
              v-if="row.status === AssignmentStatus.Active"
              size="sm"
              variant="outline"
              :loading="cancellingId === row.id"
              @click="cancel(row)"
            >
              Cancel
            </AppButton>
          </div>
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

    <AssignmentFormModal
      v-model="modalOpen"
      :mode="modalMode"
      :initial="editingAssignment"
      :submitting="submitting"
      :api-error="apiError"
      @submit="handleSubmit"
    />
  </div>
</template>
