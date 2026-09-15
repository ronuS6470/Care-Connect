<script setup lang="ts">
import { reactive, ref } from 'vue'

import AppAlert from '@/components/common/AppAlert.vue'
import AppButton from '@/components/common/AppButton.vue'
import AppDateInput from '@/components/common/AppDateInput.vue'
import AppInput from '@/components/common/AppInput.vue'
import type { Caregiver, CreateCaregiverPayload, UpdateCaregiverPayload } from '@/types/caregiver'
import { toDateInputValue } from '@/utils/date'
import { maxLength, numberRange, required, runRules } from '@/validation/rules'

const props = withDefaults(
  defineProps<{
    mode: 'create' | 'edit'
    initial?: Caregiver | null
    submitting?: boolean
    apiError?: string | null
  }>(),
  {
    initial: null,
    submitting: false,
    apiError: null,
  },
)

const emit = defineEmits<{
  submit: [CreateCaregiverPayload | UpdateCaregiverPayload]
  cancel: []
}>()

const today = toDateInputValue(new Date())
const eighteenYearsAgo = toDateInputValue(new Date(new Date().setFullYear(new Date().getFullYear() - 18)))

const form = reactive({
  userId: props.initial?.userId ?? null as number | null,
  licenseNumber: props.initial?.licenseNumber ?? '',
  hourlyRate: props.initial?.hourlyRate ?? (null as number | null),
  hireDate: toDateInputValue(props.initial?.hireDate) || today,
  dateOfBirth: toDateInputValue(props.initial?.dateOfBirth),
  yearsOfExperience: props.initial?.yearsOfExperience ?? (0 as number | null),
  isActive: props.initial?.isActive ?? true,
})

const errors = reactive<Record<string, string | null>>({
  userId: null,
  licenseNumber: null,
  hourlyRate: null,
  hireDate: null,
  dateOfBirth: null,
  yearsOfExperience: null,
})

const touched = ref(false)

function validate(): boolean {
  if (props.mode === 'create') {
    errors.userId = runRules(form.userId, [required('Enter the linked user ID.'), numberRange(1, Number.MAX_SAFE_INTEGER, 'Must be a positive ID.')])
    errors.dateOfBirth = runRules(form.dateOfBirth, [required('Enter a date of birth.')])
    if (!errors.dateOfBirth && form.dateOfBirth > eighteenYearsAgo) {
      errors.dateOfBirth = 'Caregiver must be at least 18 years old.'
    }
  }

  errors.licenseNumber = runRules(form.licenseNumber, [maxLength(50)])
  errors.hourlyRate = runRules(form.hourlyRate, [required('Enter an hourly rate.'), numberRange(0.01, Number.MAX_SAFE_INTEGER, 'Must be greater than 0.')])
  errors.hireDate = runRules(form.hireDate, [required('Enter a hire date.')])
  errors.yearsOfExperience = runRules(form.yearsOfExperience, [required('Enter years of experience.'), numberRange(0, 80)])

  return Object.values(errors).every((error) => error === null)
}

function handleSubmit() {
  touched.value = true
  if (!validate()) return

  if (props.mode === 'create') {
    emit('submit', {
      userId: form.userId!,
      licenseNumber: form.licenseNumber || null,
      hourlyRate: form.hourlyRate!,
      hireDate: form.hireDate,
      dateOfBirth: form.dateOfBirth,
      yearsOfExperience: form.yearsOfExperience!,
    } satisfies CreateCaregiverPayload)
  } else {
    emit('submit', {
      licenseNumber: form.licenseNumber || null,
      hourlyRate: form.hourlyRate!,
      hireDate: form.hireDate,
      yearsOfExperience: form.yearsOfExperience!,
      isActive: form.isActive,
    } satisfies UpdateCaregiverPayload)
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
      hint="No user directory is exposed by the API — enter the ID of the existing user account this caregiver profile belongs to."
      required
      :error="touched ? errors.userId : null"
    />

    <div class="grid gap-5 sm:grid-cols-2">
      <AppInput v-model="form.licenseNumber" label="License Number" placeholder="Optional" :error="touched ? errors.licenseNumber : null" />
      <AppInput
        v-model="form.hourlyRate"
        type="number"
        label="Hourly Rate ($)"
        required
        :error="touched ? errors.hourlyRate : null"
      />
    </div>

    <div class="grid gap-5 sm:grid-cols-2">
      <AppDateInput v-model="form.hireDate" label="Hire Date" :max="today" required :error="touched ? errors.hireDate : null" />
      <AppDateInput
        v-if="mode === 'create'"
        v-model="form.dateOfBirth"
        label="Date of Birth"
        :max="eighteenYearsAgo"
        required
        :error="touched ? errors.dateOfBirth : null"
      />
      <AppInput
        v-model="form.yearsOfExperience"
        type="number"
        label="Years of Experience"
        required
        :error="touched ? errors.yearsOfExperience : null"
      />
    </div>

    <label v-if="mode === 'edit'" class="flex items-center gap-2 text-sm text-ink">
      <input v-model="form.isActive" type="checkbox" class="h-4 w-4 rounded border-border text-brand-600 focus:ring-brand-500" />
      Active
    </label>

    <div class="flex items-center justify-end gap-2 border-t border-border pt-5">
      <AppButton variant="outline" type="button" @click="emit('cancel')">Cancel</AppButton>
      <AppButton type="submit" :loading="submitting">{{ mode === 'create' ? 'Create Caregiver' : 'Save Changes' }}</AppButton>
    </div>
  </form>
</template>
