<script setup lang="ts">
import { computed, ref, watch } from 'vue'

import AppAlert from '@/components/common/AppAlert.vue'
import AppButton from '@/components/common/AppButton.vue'
import AppModal from '@/components/common/AppModal.vue'
import AppSelect from '@/components/common/AppSelect.vue'
import type { SelectOption } from '@/types/common'
import { UserRole, USER_ROLE_LABELS } from '@/types/enums'
import type { User } from '@/types/user'

const props = withDefaults(
  defineProps<{
    modelValue: boolean
    user: User | null
    submitting?: boolean
    apiError?: string | null
  }>(),
  {
    submitting: false,
    apiError: null,
  },
)

const emit = defineEmits<{
  'update:modelValue': [boolean]
  submit: [UserRole]
}>()

const role = ref<UserRole>(UserRole.Client)

watch(
  () => props.modelValue,
  (open) => {
    if (open && props.user) role.value = props.user.role
  },
)

const roleOptions: SelectOption<UserRole>[] = [
  { value: UserRole.Admin, label: USER_ROLE_LABELS[UserRole.Admin] },
  { value: UserRole.Caregiver, label: USER_ROLE_LABELS[UserRole.Caregiver] },
  { value: UserRole.Client, label: USER_ROLE_LABELS[UserRole.Client] },
]

const unchanged = computed(() => props.user !== null && role.value === props.user.role)

/**
 * The backend refuses to move a user off a role whose profile still exists, because that profile
 * (and everything joined to it) would be stranded under an account that can no longer reach it.
 * Warning up front beats submitting into a 409.
 */
const profileWarning = computed(() => {
  if (!props.user || unchanged.value) return null
  if (props.user.caregiverId !== null) return 'This user has a caregiver profile, so their role cannot be changed until it is removed or reassigned.'
  if (props.user.clientId !== null) return 'This user has a client profile, so their role cannot be changed until it is removed or reassigned.'
  return null
})
</script>

<template>
  <AppModal
    :model-value="modelValue"
    :title="user ? `Change role — ${user.fullName}` : 'Change role'"
    size="sm"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <form id="user-role-form" class="space-y-4" novalidate @submit.prevent="emit('submit', role)">
      <AppAlert v-if="apiError" tone="danger">{{ apiError }}</AppAlert>
      <AppAlert v-else-if="profileWarning" tone="warning">{{ profileWarning }}</AppAlert>

      <AppSelect v-model="role" label="Role" :options="roleOptions" required />

      <p class="text-xs text-ink-muted">
        A role change takes effect the next time this user signs in — their current session keeps the
        old role until it expires.
      </p>
    </form>

    <template #footer>
      <AppButton variant="outline" type="button" @click="emit('update:modelValue', false)">Cancel</AppButton>
      <AppButton type="submit" form="user-role-form" :loading="submitting" :disabled="unchanged">Save Role</AppButton>
    </template>
  </AppModal>
</template>
