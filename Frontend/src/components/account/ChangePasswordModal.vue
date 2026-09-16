<script setup lang="ts">
import { reactive, ref, watch } from 'vue'

import AppAlert from '@/components/common/AppAlert.vue'
import AppButton from '@/components/common/AppButton.vue'
import AppInput from '@/components/common/AppInput.vue'
import AppModal from '@/components/common/AppModal.vue'
import { useApiError } from '@/composables/useApiError'
import { useToast } from '@/composables/useToast'
import { changePassword } from '@/services/authService'
import { ApiError } from '@/services/api'
import { PASSWORD_MAX_LENGTH, PASSWORD_MIN_LENGTH } from '@/validation/password'
import { maxLength, minLength, required, runRules } from '@/validation/rules'

const props = defineProps<{ modelValue: boolean }>()
const emit = defineEmits<{ 'update:modelValue': [boolean] }>()

const toast = useToast()
const { getMessage } = useApiError()

const form = reactive({ currentPassword: '', newPassword: '', confirmPassword: '' })
const errors = reactive<Record<'currentPassword' | 'newPassword' | 'confirmPassword', string | null>>({
  currentPassword: null,
  newPassword: null,
  confirmPassword: null,
})

const touched = ref(false)
const submitting = ref(false)
const apiError = ref<string | null>(null)

watch(
  () => props.modelValue,
  (open) => {
    if (!open) return
    form.currentPassword = ''
    form.newPassword = ''
    form.confirmPassword = ''
    errors.currentPassword = errors.newPassword = errors.confirmPassword = null
    touched.value = false
    apiError.value = null
  },
)

function validate(): boolean {
  errors.currentPassword = runRules(form.currentPassword, [required('Enter your current password.')])
  errors.newPassword = runRules(form.newPassword, [
    required('Enter a new password.'),
    minLength(PASSWORD_MIN_LENGTH),
    maxLength(PASSWORD_MAX_LENGTH),
  ])

  if (!errors.newPassword && form.newPassword === form.currentPassword) {
    errors.newPassword = 'New password must be different from your current one.'
  }

  errors.confirmPassword = form.confirmPassword === form.newPassword ? null : 'Passwords do not match.'

  return Object.values(errors).every((error) => error === null)
}

async function handleSubmit() {
  touched.value = true
  apiError.value = null
  if (!validate()) return

  submitting.value = true
  try {
    await changePassword({ currentPassword: form.currentPassword, newPassword: form.newPassword })
    toast.success('Password changed.')
    emit('update:modelValue', false)
  } catch (err) {
    // A wrong current password comes back as a 400 carrying field-level errors — show them against
    // the field rather than as a generic banner. It is never a 401, so this never signs the user out.
    if (err instanceof ApiError && err.status === 400 && err.errors.length > 0) {
      const currentPasswordError = err.errors.find((e) => e.includes('CurrentPassword'))
      const newPasswordError = err.errors.find((e) => e.includes('NewPassword'))

      errors.currentPassword = currentPasswordError ? stripFieldPrefix(currentPasswordError) : null
      errors.newPassword = newPasswordError ? stripFieldPrefix(newPasswordError) : null

      if (!currentPasswordError && !newPasswordError) apiError.value = getMessage(err)
    } else {
      apiError.value = getMessage(err)
    }
  } finally {
    submitting.value = false
  }
}

/** Backend errors read "Password.CurrentPassword: ..." — the field is already obvious from position. */
function stripFieldPrefix(message: string): string {
  const separator = message.indexOf(': ')
  return separator === -1 ? message : message.slice(separator + 2)
}
</script>

<template>
  <AppModal
    :model-value="modelValue"
    title="Change password"
    size="sm"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <form id="change-password-form" class="space-y-4" novalidate @submit.prevent="handleSubmit">
      <AppAlert v-if="apiError" tone="danger">{{ apiError }}</AppAlert>

      <AppInput
        v-model="form.currentPassword"
        type="password"
        label="Current password"
        autocomplete="current-password"
        required
        :error="touched ? errors.currentPassword : null"
      />

      <AppInput
        v-model="form.newPassword"
        type="password"
        label="New password"
        autocomplete="new-password"
        required
        :hint="`At least ${PASSWORD_MIN_LENGTH} characters.`"
        :error="touched ? errors.newPassword : null"
      />

      <AppInput
        v-model="form.confirmPassword"
        type="password"
        label="Confirm new password"
        autocomplete="new-password"
        required
        :error="touched ? errors.confirmPassword : null"
      />

      <p class="text-xs text-ink-muted">You'll stay signed in on this device after changing it.</p>
    </form>

    <template #footer>
      <AppButton variant="outline" type="button" @click="emit('update:modelValue', false)">Cancel</AppButton>
      <AppButton type="submit" form="change-password-form" :loading="submitting">Change Password</AppButton>
    </template>
  </AppModal>
</template>
