<script setup lang="ts">
import { onMounted, ref } from 'vue'

import AppButton from '@/components/common/AppButton.vue'
import AppSelect from '@/components/common/AppSelect.vue'
import DataTable, { type DataTableColumn } from '@/components/common/DataTable.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import Pagination from '@/components/common/Pagination.vue'
import SearchInput from '@/components/common/SearchInput.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import CareTaskFormModal from '@/components/careTasks/CareTaskFormModal.vue'
import { useApiError } from '@/composables/useApiError'
import { useCareTasks, type CareTaskStatusFilter } from '@/composables/useCareTasks'
import { useToast } from '@/composables/useToast'
import { createCareTask, updateCareTask } from '@/services/careTaskService'
import type { CareTask, CareTaskPayload } from '@/types/careTask'
import type { SelectOption } from '@/types/common'
import { activeStatusTone } from '@/utils/status'

const { rows, loading, error, load, search, statusFilter, pagination, deactivate, deactivatingId } = useCareTasks()

onMounted(load)

const statusFilterOptions: SelectOption<CareTaskStatusFilter>[] = [
  { value: 'all', label: 'All statuses' },
  { value: 'active', label: 'Active only' },
  { value: 'inactive', label: 'Inactive only' },
]

const columns: DataTableColumn<CareTask>[] = [
  { key: 'name', label: 'Name', value: (row) => row.name },
  { key: 'description', label: 'Description', value: (row) => row.description ?? '—' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions', align: 'right' },
]

// Create/edit modal orchestration stays page-specific — the composable only owns the list and
// the one-shot "deactivate" action, which is the part that was actually duplicated elsewhere.
const toast = useToast()
const { getMessage } = useApiError()

const modalOpen = ref(false)
const modalMode = ref<'create' | 'edit'>('create')
const editingTask = ref<CareTask | null>(null)
const submitting = ref(false)
const apiError = ref<string | null>(null)

function openCreate() {
  modalMode.value = 'create'
  editingTask.value = null
  apiError.value = null
  modalOpen.value = true
}

function openEdit(task: CareTask) {
  modalMode.value = 'edit'
  editingTask.value = task
  apiError.value = null
  modalOpen.value = true
}

async function handleSubmit(payload: CareTaskPayload) {
  submitting.value = true
  apiError.value = null
  try {
    if (modalMode.value === 'create') {
      await createCareTask(payload)
      toast.success('Care task created.')
    } else if (editingTask.value) {
      await updateCareTask(editingTask.value.id, payload)
      toast.success('Care task updated.')
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
    <PageHeader title="Care Tasks" description="The task catalog available to attach to visits.">
      <template #actions>
        <AppButton @click="openCreate">New Care Task</AppButton>
      </template>
    </PageHeader>

    <div class="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center">
      <div class="max-w-xs flex-1">
        <SearchInput v-model="search" placeholder="Search by name or description…" />
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
        empty-title="No care tasks found"
        :empty-description="search ? 'Try a different search.' : 'Create the first care task to get started.'"
      >
        <template #cell-status="{ row }">
          <StatusBadge :label="row.isActive ? 'Active' : 'Inactive'" :tone="activeStatusTone(row.isActive)" />
        </template>

        <template #cell-actions="{ row }">
          <div class="flex justify-end gap-2">
            <AppButton size="sm" variant="ghost" @click="openEdit(row)">Edit</AppButton>
            <AppButton
              v-if="row.isActive"
              size="sm"
              variant="outline"
              :loading="deactivatingId === row.id"
              @click="deactivate(row)"
            >
              Deactivate
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

    <CareTaskFormModal
      v-model="modalOpen"
      :mode="modalMode"
      :initial="editingTask"
      :submitting="submitting"
      :api-error="apiError"
      @submit="handleSubmit"
    />
  </div>
</template>
