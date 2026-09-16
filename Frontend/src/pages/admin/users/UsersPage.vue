<script setup lang="ts">
import { onMounted, ref } from 'vue'

import AppButton from '@/components/common/AppButton.vue'
import AppCard from '@/components/common/AppCard.vue'
import AppSelect from '@/components/common/AppSelect.vue'
import DataTable, { type DataTableColumn } from '@/components/common/DataTable.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import Pagination from '@/components/common/Pagination.vue'
import SearchInput from '@/components/common/SearchInput.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import ChangeUserRoleModal from '@/components/users/ChangeUserRoleModal.vue'
import ResetUserPasswordModal from '@/components/users/ResetUserPasswordModal.vue'
import { useApiError } from '@/composables/useApiError'
import { useConfirm } from '@/composables/useConfirm'
import { useToast } from '@/composables/useToast'
import { useUsers, type UserRoleFilter, type UserStatusFilter } from '@/composables/useUsers'
import { resetUserPassword, updateUserRole } from '@/services/userService'
import { useAuthStore } from '@/stores/auth'
import type { SelectOption } from '@/types/common'
import { UserRole, USER_ROLE_LABELS } from '@/types/enums'
import type { User } from '@/types/user'
import { activeStatusTone } from '@/utils/status'

const auth = useAuthStore()
const toast = useToast()
const confirm = useConfirm()
const { getMessage } = useApiError()

const { rows, loading, error, load, filters, pagination, toggleActive, togglingId } = useUsers({ pageSize: 15 })

onMounted(load)

const roleOptions: SelectOption<UserRoleFilter>[] = [
  { value: 'all', label: 'All roles' },
  { value: UserRole.Admin, label: USER_ROLE_LABELS[UserRole.Admin] },
  { value: UserRole.Caregiver, label: USER_ROLE_LABELS[UserRole.Caregiver] },
  { value: UserRole.Client, label: USER_ROLE_LABELS[UserRole.Client] },
]

const statusOptions: SelectOption<UserStatusFilter>[] = [
  { value: 'all', label: 'All statuses' },
  { value: 'active', label: 'Active' },
  { value: 'inactive', label: 'Deactivated' },
]

const columns: DataTableColumn<User>[] = [
  { key: 'user', label: 'User' },
  { key: 'role', label: 'Role' },
  { key: 'profile', label: 'Profile' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions', align: 'right' },
]

/** The API refuses self role changes and self-deactivation; hide those actions rather than let them 409. */
function isSelf(user: User) {
  return user.id === auth.userId
}

function profileLabel(user: User) {
  if (user.role === UserRole.Caregiver) return user.caregiverId === null ? 'No caregiver profile' : `Caregiver #${user.caregiverId}`
  if (user.role === UserRole.Client) return user.clientId === null ? 'No client profile' : `Client #${user.clientId}`
  return '—'
}

function profileMissing(user: User) {
  return (user.role === UserRole.Caregiver && user.caregiverId === null)
    || (user.role === UserRole.Client && user.clientId === null)
}

const roleModalOpen = ref(false)
const passwordModalOpen = ref(false)
const selectedUser = ref<User | null>(null)
const submitting = ref(false)
const modalError = ref<string | null>(null)

function openRoleModal(user: User) {
  selectedUser.value = user
  modalError.value = null
  roleModalOpen.value = true
}

function openPasswordModal(user: User) {
  selectedUser.value = user
  modalError.value = null
  passwordModalOpen.value = true
}

async function handleRoleSubmit(role: UserRole) {
  if (!selectedUser.value) return

  submitting.value = true
  modalError.value = null
  try {
    await updateUserRole(selectedUser.value.id, { role })
    toast.success(`${selectedUser.value.fullName} is now ${USER_ROLE_LABELS[role]}. It applies at their next sign-in.`)
    roleModalOpen.value = false
    await load()
  } catch (err) {
    modalError.value = getMessage(err)
  } finally {
    submitting.value = false
  }
}

async function handlePasswordSubmit(newPassword: string) {
  if (!selectedUser.value) return

  submitting.value = true
  modalError.value = null
  try {
    await resetUserPassword(selectedUser.value.id, { newPassword })
    toast.success(`Password reset for ${selectedUser.value.fullName}.`)
    passwordModalOpen.value = false
    await load()
  } catch (err) {
    modalError.value = getMessage(err)
  } finally {
    submitting.value = false
  }
}

async function handleToggleActive(user: User) {
  if (user.isActive) {
    const confirmed = await confirm.ask({
      title: `Deactivate ${user.fullName}?`,
      message: 'They will not be able to sign in. Any session they already have stays active until it expires.',
      confirmLabel: 'Deactivate',
      tone: 'danger',
    })
    if (!confirmed) return
  }

  if (await toggleActive(user)) await load()
}
</script>

<template>
  <div>
    <PageHeader title="Users" description="Accounts, roles, and sign-in access." />

    <AppCard class="mb-4" :padded="true">
      <div class="grid gap-4 sm:grid-cols-3">
        <SearchInput v-model="filters.search" placeholder="Search name or email" />
        <AppSelect v-model="filters.role" :options="roleOptions" />
        <AppSelect v-model="filters.status" :options="statusOptions" />
      </div>
    </AppCard>

    <ErrorState v-if="error" class="card-base" :description="error" @retry="load" />

    <template v-else>
      <DataTable
        :columns="columns"
        :rows="rows"
        :row-key="(row) => row.id"
        :loading="loading"
        empty-title="No users found"
        empty-description="No accounts match these filters."
      >
        <template #cell-user="{ row }">
          <div class="min-w-0">
            <p class="truncate text-ink">{{ row.fullName }}<span v-if="isSelf(row)" class="text-ink-muted"> (you)</span></p>
            <p class="truncate text-xs text-ink-muted">{{ row.email }}</p>
          </div>
        </template>

        <template #cell-role="{ row }">
          <StatusBadge :label="USER_ROLE_LABELS[row.role]" tone="brand" />
        </template>

        <template #cell-profile="{ row }">
          <span :class="profileMissing(row) ? 'text-warning-600' : 'text-ink-muted'" class="text-xs">
            {{ profileLabel(row) }}
          </span>
        </template>

        <template #cell-status="{ row }">
          <div class="flex flex-col items-start gap-1">
            <StatusBadge :label="row.isActive ? 'Active' : 'Deactivated'" :tone="activeStatusTone(row.isActive)" />
            <span v-if="!row.hasPassword" class="text-xs text-warning-600">No password set</span>
          </div>
        </template>

        <template #cell-actions="{ row }">
          <div class="flex justify-end gap-1">
            <AppButton size="sm" variant="ghost" :disabled="isSelf(row)" @click="openRoleModal(row)">Role</AppButton>
            <AppButton size="sm" variant="ghost" @click="openPasswordModal(row)">Reset password</AppButton>
            <AppButton
              size="sm"
              variant="ghost"
              :disabled="isSelf(row) || togglingId === row.id"
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

    <ChangeUserRoleModal
      v-model="roleModalOpen"
      :user="selectedUser"
      :submitting="submitting"
      :api-error="modalError"
      @submit="handleRoleSubmit"
    />

    <ResetUserPasswordModal
      v-model="passwordModalOpen"
      :user="selectedUser"
      :submitting="submitting"
      :api-error="modalError"
      @submit="handlePasswordSubmit"
    />
  </div>
</template>
