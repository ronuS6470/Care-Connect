<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'

import AppButton from '@/components/common/AppButton.vue'
import DataTable, { type DataTableColumn } from '@/components/common/DataTable.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import Pagination from '@/components/common/Pagination.vue'
import SearchInput from '@/components/common/SearchInput.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useCaregivers } from '@/composables/useCaregivers'
import type { Caregiver } from '@/types/caregiver'
import { formatCurrency } from '@/utils/currency'
import { activeStatusTone } from '@/utils/status'

const router = useRouter()

const { rows, loading, error, load, search, pagination, toggleActive, togglingId } = useCaregivers()

onMounted(load)

const columns: DataTableColumn<Caregiver>[] = [
  { key: 'name', label: 'Name', value: (row) => row.fullName },
  { key: 'email', label: 'Email', value: (row) => row.email },
  { key: 'rate', label: 'Hourly Rate', value: (row) => formatCurrency(row.hourlyRate), align: 'right' },
  { key: 'experience', label: 'Experience', value: (row) => `${row.yearsOfExperience} yr(s)`, align: 'right' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions', align: 'right' },
]

async function handleToggleActive(row: Caregiver) {
  if (await toggleActive(row)) await load()
}
</script>

<template>
  <div>
    <PageHeader title="Caregivers" description="Every caregiver profile in CareConnect.">
      <template #actions>
        <AppButton @click="router.push('/admin/caregivers/new')">New Caregiver</AppButton>
      </template>
    </PageHeader>

    <div class="mb-4 max-w-xs">
      <SearchInput v-model="search" placeholder="Search by name, email, or license…" />
    </div>

    <ErrorState v-if="error" class="card-base" :description="error" @retry="load" />

    <template v-else>
      <DataTable
        :columns="columns"
        :rows="rows"
        :row-key="(row) => row.id"
        :loading="loading"
        empty-title="No caregivers found"
        :empty-description="search ? 'Try a different search.' : 'Create the first caregiver profile to get started.'"
      >
        <template #cell-status="{ row }">
          <StatusBadge :label="row.isActive ? 'Active' : 'Inactive'" :tone="activeStatusTone(row.isActive)" />
        </template>

        <template #cell-actions="{ row }">
          <div class="flex justify-end gap-2">
            <AppButton size="sm" variant="ghost" @click="router.push(`/admin/caregivers/${row.id}`)">View</AppButton>
            <AppButton size="sm" variant="ghost" @click="router.push(`/admin/caregivers/${row.id}/edit`)">Edit</AppButton>
            <AppButton
              size="sm"
              variant="outline"
              :loading="togglingId === row.id"
              @click="handleToggleActive(row)"
            >
              {{ row.isActive ? 'Deactivate' : 'Activate' }}
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
  </div>
</template>
