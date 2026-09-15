<script setup lang="ts">
import { reactive, ref, watch } from 'vue'

import AppAlert from '@/components/common/AppAlert.vue'
import AppButton from '@/components/common/AppButton.vue'
import AppModal from '@/components/common/AppModal.vue'
import AppTimeInput from '@/components/common/AppTimeInput.vue'
import type { CaregiverAvailability } from '@/types/caregiver'
import { dayOfWeekLabel } from '@/utils/date'
import { required, runRules } from '@/validation/rules'

/** The parent page owns caregiverId/dayOfWeek (it knows which day's modal is open) and merges them in. */
export interface AvailabilityFormValues {
  startTime: string
  endTime: string
  isActive: boolean
}

const props = withDefaults(
  defineProps<{
    modelValue: boolean
    mode: 'create' | 'edit'
    /** Which day this window belongs to — fixed for both create (picked from that day's card) and edit. */
    dayOfWeek: number
    initial?: CaregiverAvailability | null
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
  'update:modelValue': [boolean]
  submit: [AvailabilityFormValues]
}>()

const form = reactive({ startTime: '', endTime: '', isActive: true })
const errors = reactive<{ startTime: string | null; endTime: string | null }>({ startTime: null, endTime: null })
const touched = ref(false)

watch(
  () => props.modelValue,
  (open) => {
    if (!open) return
    form.startTime = props.initial?.startTime.slice(0, 5) ?? ''
    form.endTime = props.initial?.endTime.slice(0, 5) ?? ''
    form.isActive = props.initial?.isActive ?? true
    errors.startTime = null
    errors.endTime = null
    touched.value = false
  },
)

function validate(): boolean {
  errors.startTime = runRules(form.startTime, [required('Select a start time.')])
  errors.endTime = runRules(form.endTime, [required('Select an end time.')])

  if (!errors.startTime && !errors.endTime && form.endTime <= form.startTime) {
    errors.endTime = 'End time must be after the start time.'
  }

  return errors.startTime === null && errors.endTime === null
}

function handleSubmit() {
  touched.value = true
  if (!validate()) return

  emit('submit', {
    startTime: `${form.startTime}:00`,
    endTime: `${form.endTime}:00`,
    isActive: form.isActive,
  })
}
</script>

<template>
  <AppModal
    :model-value="modelValue"
    :title="`${mode === 'create' ? 'Add' : 'Edit'} availability — ${dayOfWeekLabel(dayOfWeek)}`"
    size="sm"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <form id="availability-form" class="space-y-5" novalidate @submit.prevent="handleSubmit">
      <AppAlert v-if="apiError" tone="danger">{{ apiError }}</AppAlert>

      <div class="grid gap-5 sm:grid-cols-2">
        <AppTimeInput v-model="form.startTime" label="Start Time" required :error="touched ? errors.startTime : null" />
        <AppTimeInput v-model="form.endTime" label="End Time" required :error="touched ? errors.endTime : null" />
      </div>

      <label v-if="mode === 'edit'" class="flex items-center gap-2 text-sm text-ink">
        <input v-model="form.isActive" type="checkbox" class="h-4 w-4 rounded border-border text-brand-600 focus:ring-brand-500" />
        Active
      </label>
    </form>

    <template #footer>
      <AppButton variant="outline" type="button" @click="emit('update:modelValue', false)">Cancel</AppButton>
      <AppButton type="submit" form="availability-form" :loading="submitting">
        {{ mode === 'create' ? 'Add' : 'Save Changes' }}
      </AppButton>
    </template>
  </AppModal>
</template>
