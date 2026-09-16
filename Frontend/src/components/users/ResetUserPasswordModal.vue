<script setup lang="ts">
import { reactive, ref, watch } from 'vue'

import AppAlert from '@/components/common/AppAlert.vue'
import AppButton from '@/components/common/AppButton.vue'
import AppInput from '@/components/common/AppInput.vue'
import AppModal from '@/components/common/AppModal.vue'
import type { User } from '@/types/user'
import { PASSWORD_MIN_LENGTH } from '@/validation/password'
import { maxLength, minLength, required, runRules } from '@/validation/rules'

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
  submit: [string]
}>()

const form = reactive({ newPassword: '', confirmPassword: '' })
const errors = reactive<{ newPassword: string | null; confirmPassword: string | null }>({
  newPassword: null,
  confirmPassword: null,
})
const touched = ref(false)

watch(
  () => props.modelValue,
  (open) => {
    if (!open) return
    form.newPassword = ''
    form.confirmPassword = ''
    errors.newPassword = null
    errors.confirmPassword = null
    touched.value = false
  },
)

function validate(): boolean {
  errors.newPassword = runRules(form.newPassword, [
    required('Enter a new password.'),
    minLength(PASSWORD_MIN_LENGTH),
    maxLength(128),
  ])
  errors.confirmPassword = form.confirmPassword === form.newPassword ? null : 'Passwords do not match.'

  return errors.newPassword === null && errors.confirmPassword === null
}

function handleSubmit() {
  touched.value = true
  if (!validate()) return

  emit('submit', form.newPassword)
}
</script>

<template>
  <AppModal
    :model-value="modelValue"
    :title="user ? `Reset password — ${user.fullName}` : 'Reset password'"
    size="sm"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <form id="reset-password-form" class="space-y-4" novalidate @submit.prevent="handleSubmit">
      <AppAlert v-if="apiError" tone="danger">{{ apiError }}</AppAlert>

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

      <p class="text-xs text-ink-muted">
        Tell the user their new password over a separate channel, and ask them to change it after
        signing in. Any session they already have stays signed in until it expires.
      </p>
    </form>

    <template #footer>
      <AppButton variant="outline" type="button" @click="emit('update:modelValue', false)">Cancel</AppButton>
      <AppButton type="submit" form="reset-password-form" :loading="submitting">Reset Password</AppButton>
    </template>
  </AppModal>
</template>
