<script setup lang="ts">
import { reactive, ref } from 'vue'

import AppAlert from '@/components/common/AppAlert.vue'
import AppButton from '@/components/common/AppButton.vue'
import AppInput from '@/components/common/AppInput.vue'
import type { Client, CreateClientPayload, UpdateClientPayload } from '@/types/client'
import { exactLength, maxLength, numberRange, required, runRules, usPhoneNumber, usZipCode } from '@/validation/rules'

const props = withDefaults(
  defineProps<{
    mode: 'create' | 'edit'
    initial?: Client | null
    submitting?: boolean
    apiError?: string | null
    /** The client's own self-service profile page hides this — deactivation is an admin action, not something to self-serve. */
    hideActiveToggle?: boolean
  }>(),
  {
    initial: null,
    submitting: false,
    apiError: null,
    hideActiveToggle: false,
  },
)

const emit = defineEmits<{
  submit: [CreateClientPayload | UpdateClientPayload]
  cancel: []
}>()

const form = reactive({
  userId: props.initial?.userId ?? (null as number | null),
  addressLine1: props.initial?.addressLine1 ?? '',
  addressLine2: props.initial?.addressLine2 ?? '',
  city: props.initial?.city ?? '',
  state: props.initial?.state ?? '',
  postalCode: props.initial?.postalCode ?? '',
  emergencyContactName: props.initial?.emergencyContactName ?? '',
  emergencyContactPhone: props.initial?.emergencyContactPhone ?? '',
  isActive: props.initial?.isActive ?? true,
})

const errors = reactive<Record<string, string | null>>({
  userId: null,
  addressLine1: null,
  city: null,
  state: null,
  postalCode: null,
  emergencyContactPhone: null,
})

const touched = ref(false)

function validate(): boolean {
  if (props.mode === 'create') {
    errors.userId = runRules(form.userId, [required('Enter the linked user ID.'), numberRange(1, Number.MAX_SAFE_INTEGER, 'Must be a positive ID.')])
  }

  errors.addressLine1 = runRules(form.addressLine1, [required('Enter an address.'), maxLength(200)])
  errors.city = runRules(form.city, [required('Enter a city.'), maxLength(100)])
  errors.state = runRules(form.state.toUpperCase(), [required('Enter a state.'), exactLength(2, 'Use a 2-letter state code.')])
  errors.postalCode = runRules(form.postalCode, [required('Enter a ZIP code.'), usZipCode()])
  errors.emergencyContactPhone = form.emergencyContactPhone ? runRules(form.emergencyContactPhone, [usPhoneNumber()]) : null

  return Object.values(errors).every((error) => error === null)
}

function handleSubmit() {
  touched.value = true
  if (!validate()) return

  const shared = {
    addressLine1: form.addressLine1,
    addressLine2: form.addressLine2 || null,
    city: form.city,
    state: form.state.toUpperCase(),
    postalCode: form.postalCode,
    emergencyContactName: form.emergencyContactName || null,
    emergencyContactPhone: form.emergencyContactPhone || null,
  }

  if (props.mode === 'create') {
    emit('submit', { userId: form.userId!, ...shared } satisfies CreateClientPayload)
  } else {
    emit('submit', { ...shared, isActive: form.isActive } satisfies UpdateClientPayload)
  }
}
</script>

<template>
  <form class="space-y-5" novalidate @submit.prevent="handleSubmit">
    <AppAlert v-if="apiError" tone="danger">{{ apiError }}</AppAlert>

    <AppInput
      v-if="mode === 'create'"
      v-model="form.userId"
      type="number"
      label="Linked User ID"
      hint="No user directory is exposed by the API — enter the ID of the existing user account this client profile belongs to."
      required
      :error="touched ? errors.userId : null"
    />

    <AppInput v-model="form.addressLine1" label="Address Line 1" required :error="touched ? errors.addressLine1 : null" />
    <AppInput v-model="form.addressLine2" label="Address Line 2" placeholder="Optional" />

    <div class="grid gap-5 sm:grid-cols-3">
      <AppInput v-model="form.city" label="City" required :error="touched ? errors.city : null" />
      <AppInput v-model="form.state" label="State" placeholder="CA" required :error="touched ? errors.state : null" />
      <AppInput v-model="form.postalCode" label="ZIP Code" required :error="touched ? errors.postalCode : null" />
    </div>

    <div class="grid gap-5 sm:grid-cols-2">
      <AppInput v-model="form.emergencyContactName" label="Emergency Contact Name" placeholder="Optional" />
      <AppInput
        v-model="form.emergencyContactPhone"
        label="Emergency Contact Phone"
        placeholder="Optional"
        :error="touched ? errors.emergencyContactPhone : null"
      />
    </div>

    <label v-if="mode === 'edit' && !hideActiveToggle" class="flex items-center gap-2 text-sm text-ink">
      <input v-model="form.isActive" type="checkbox" class="h-4 w-4 rounded border-border text-brand-600 focus:ring-brand-500" />
      Active
    </label>

    <div class="flex items-center justify-end gap-2 border-t border-border pt-5">
      <AppButton variant="outline" type="button" @click="emit('cancel')">Cancel</AppButton>
      <AppButton type="submit" :loading="submitting">{{ mode === 'create' ? 'Create Client' : 'Save Changes' }}</AppButton>
    </div>
  </form>
</template>
