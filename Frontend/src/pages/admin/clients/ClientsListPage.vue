<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'

import AppButton from '@/components/common/AppButton.vue'
import AppSelect from '@/components/common/AppSelect.vue'
import DataTable, { type DataTableColumn } from '@/components/common/DataTable.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import Pagination from '@/components/common/Pagination.vue'
import SearchInput from '@/components/common/SearchInput.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useClients, type ClientStatusFilter } from '@/composables/useClients'
import type { SelectOption } from '@/types/common'
import type { Client } from '@/types/client'
import { activeStatusTone } from '@/utils/status'

const router = useRouter()

const { rows, loading, error, load, filters, pagination } = useClients()

onMounted(load)

const statusFilterOptions: SelectOption<ClientStatusFilter>[] = [
  { value: 'all', label: 'All statuses' },
  { value: 'active', label: 'Active only' },
  { value: 'inactive', label: 'Inactive only' },
]

const columns: DataTableColumn<Client>[] = [
  { key: 'name', label: 'Name', value: (row) => row.fullName },
  { key: 'email', label: 'Email', value: (row) => row.email },
  { key: 'city', label: 'City', value: (row) => `${row.city}, ${row.state}` },
  { key: 'phone', label: 'Phone', value: (row) => row.phoneNumber ?? '—' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions', align: 'right' },
]
</script>

<template>
  <div>
    <PageHeader title="Clients" description="Every client profile in CareConnect.">
      <template #actions>
        <AppButton @click="router.push('/admin/clients/new')">New Client</AppButton>
      </template>
    </PageHeader>

    <div class="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center">
      <div class="max-w-xs flex-1">
        <SearchInput v-model="filters.search" placeholder="Search by name, email, or address…" />
      </div>
      <div class="w-full sm:w-48">
        <AppSelect v-model="filters.status" :options="statusFilterOptions" />
      </div>
    </div>

    <ErrorState v-if="error" class="card-base" :description="error" @retry="load" />

    <template v-else>
      <DataTable
        :columns="columns"
        :rows="rows"
        :row-key="(row) => row.id"
        :loading="loading"
        empty-title="No clients found"
        :empty-description="filters.search ? 'Try a different search.' : 'Create the first client profile to get started.'"
      >
        <template #cell-status="{ row }">
          <StatusBadge :label="row.isActive ? 'Active' : 'Inactive'" :tone="activeStatusTone(row.isActive)" />
        </template>

        <template #cell-actions="{ row }">
          <div class="flex justify-end gap-2">
            <AppButton size="sm" variant="ghost" @click="router.push(`/admin/clients/${row.id}`)">View</AppButton>
            <AppButton size="sm" variant="ghost" @click="router.push(`/admin/clients/${row.id}/edit`)">Edit</AppButton>
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
