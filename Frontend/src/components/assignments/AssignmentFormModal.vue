<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'

import AppAlert from '@/components/common/AppAlert.vue'
import AppButton from '@/components/common/AppButton.vue'
import AppDateInput from '@/components/common/AppDateInput.vue'
import AppModal from '@/components/common/AppModal.vue'
import AppSelect from '@/components/common/AppSelect.vue'
import AppTextarea from '@/components/common/AppTextarea.vue'
import { getCaregivers } from '@/services/caregiverService'
import { getClients } from '@/services/clientService'
import type { Assignment, CreateAssignmentPayload, UpdateAssignmentPayload } from '@/types/assignment'
import type { SelectOption } from '@/types/common'
import { AssignmentStatus, ASSIGNMENT_STATUS_LABELS } from '@/types/enums'
import { required, runRules } from '@/validation/rules'

const props = withDefaults(
  defineProps<{
    modelValue: boolean
    mode: 'create' | 'edit'
    initial?: Assignment | null
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
  submit: [CreateAssignmentPayload | UpdateAssignmentPayload]
}>()

const form = reactive({
  caregiverId: null as number | null,
  clientId: null as number | null,
  status: AssignmentStatus.Active,
  startDate: '',
  endDate: '',
  notes: '',
})

const errors = reactive<Record<string, string | null>>({
  caregiverId: null,
  clientId: null,
  startDate: null,
  endDate: null,
})

const touched = ref(false)

const caregiverOptions = ref<SelectOption<number>[]>([])
const clientOptions = ref<SelectOption<number>[]>([])
const optionsLoading = ref(false)

const statusOptions: SelectOption<AssignmentStatus>[] = [
  { value: AssignmentStatus.Active, label: ASSIGNMENT_STATUS_LABELS[AssignmentStatus.Active] },
  { value: AssignmentStatus.Completed, label: ASSIGNMENT_STATUS_LABELS[AssignmentStatus.Completed] },
  { value: AssignmentStatus.Cancelled, label: ASSIGNMENT_STATUS_LABELS[AssignmentStatus.Cancelled] },
]

async function loadOptions() {
  optionsLoading.value = true
  try {
    const [caregivers, clients] = await Promise.all([getCaregivers(), getClients({ pageSize: 200 })])
    // Only active caregivers/clients are offered — CreateAssignmentCommandHandler rejects an
    // inactive one either way, this just avoids sending a request that's already doomed.
    caregiverOptions.value = caregivers.data.filter((c) => c.isActive).map((c) => ({ value: c.id, label: c.fullName }))
    clientOptions.value = clients.data.filter((c) => c.isActive).map((c) => ({ value: c.id, label: c.fullName }))
  } catch {
    caregiverOptions.value = []
    clientOptions.value = []
  } finally {
    optionsLoading.value = false
  }
}

const isEdit = computed(() => props.mode === 'edit')

watch(
  () => props.modelValue,
  (open) => {
    if (!open) return

    touched.value = false
    errors.caregiverId = errors.clientId = errors.startDate = errors.endDate = null

    if (props.mode === 'create') {
      form.caregiverId = null
      form.clientId = null
      form.startDate = ''
      form.endDate = ''
      form.notes = ''
      void loadOptions()
    } else if (props.initial) {
      form.status = props.initial.status
      form.endDate = props.initial.endDate ?? ''
      form.notes = props.initial.notes ?? ''
    }
  },
)

function validate(): boolean {
  if (props.mode === 'create') {
    errors.caregiverId = runRules(form.caregiverId, [required('Select a caregiver.')])
    errors.clientId = runRules(form.clientId, [required('Select a client.')])
    errors.startDate = runRules(form.startDate, [required('Enter a start date.')])
  }

  if (form.endDate && props.mode === 'create' && form.startDate && form.endDate < form.startDate) {
    errors.endDate = 'End date must be on or after the start date.'
  } else {
    errors.endDate = null
  }

  return Object.values(errors).every((error) => error === null)
}

function handleSubmit() {
  touched.value = true
  if (!validate()) return

  if (props.mode === 'create') {
    emit('submit', {
      caregiverId: form.caregiverId!,
      clientId: form.clientId!,
      startDate: form.startDate,
      endDate: form.endDate || null,
      notes: form.notes || null,
    } satisfies CreateAssignmentPayload)
  } else {
    emit('submit', {
      status: form.status,
      endDate: form.endDate || null,
      notes: form.notes || null,
    } satisfies UpdateAssignmentPayload)
  }
}
</script>

<template>
  <AppModal
    :model-value="modelValue"
    :title="mode === 'create' ? 'New Assignment' : 'Edit Assignment'"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <form id="assignment-form" class="space-y-5" novalidate @submit.prevent="handleSubmit">
      <AppAlert v-if="apiError" tone="danger">{{ apiError }}</AppAlert>

      <template v-if="mode === 'create'">
        <AppSelect
          v-model="form.caregiverId"
          label="Caregiver"
          :options="caregiverOptions"
          :placeholder="optionsLoading ? 'Loading…' : 'Select a caregiver'"
          :disabled="optionsLoading"
          required
          :error="touched ? errors.caregiverId : null"
        />
        <AppSelect
          v-model="form.clientId"
          label="Client"
          :options="clientOptions"
          :placeholder="optionsLoading ? 'Loading…' : 'Select a client'"
          :disabled="optionsLoading"
          required
          :error="touched ? errors.clientId : null"
        />
        <AppDateInput v-model="form.startDate" label="Start Date" required :error="touched ? errors.startDate : null" />
      </template>

      <AppSelect v-else v-model="form.status" label="Status" :options="statusOptions" required />

      <AppDateInput v-model="form.endDate" label="End Date" hint="Optional" :error="touched ? errors.endDate : null" />
      <AppTextarea v-model="form.notes" label="Notes" placeholder="Optional" :rows="3" />
    </form>

    <template #footer>
      <AppButton variant="outline" type="button" @click="emit('update:modelValue', false)">Cancel</AppButton>
      <AppButton type="submit" form="assignment-form" :loading="submitting" :disabled="isEdit ? false : optionsLoading">
        {{ mode === 'create' ? 'Create Assignment' : 'Save Changes' }}
      </AppButton>
    </template>
  </AppModal>
</template>
