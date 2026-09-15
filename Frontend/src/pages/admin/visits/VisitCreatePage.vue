<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'

import AppAlert from '@/components/common/AppAlert.vue'
import AppButton from '@/components/common/AppButton.vue'
import AppCard from '@/components/common/AppCard.vue'
import AppDateInput from '@/components/common/AppDateInput.vue'
import AppSelect from '@/components/common/AppSelect.vue'
import AppTimeInput from '@/components/common/AppTimeInput.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import { useApiError } from '@/composables/useApiError'
import { useToast } from '@/composables/useToast'
import { getAssignments } from '@/services/assignmentService'
import { getCareTasks } from '@/services/careTaskService'
import { ApiError } from '@/services/api'
import { createVisit } from '@/services/visitService'
import type { SelectOption } from '@/types/common'
import { AssignmentStatus } from '@/types/enums'
import type { CareTask } from '@/types/careTask'
import { combineDateAndTimeToIso, toDateInputValue } from '@/utils/date'
import { required, runRules } from '@/validation/rules'

const router = useRouter()
const toast = useToast()
const { getMessage } = useApiError()

// Both dropdowns are populated from the API, never hardcoded — CreateVisitDto's business rules
// (is the assignment active, is the caregiver free, is the visit window valid) are enforced
// server-side; narrowing to Active assignments / active tasks here is only a UX convenience so
// the admin isn't offered choices the backend would reject outright.
const assignmentOptions = ref<SelectOption<number>[]>([])
const taskOptions = ref<CareTask[]>([])
const optionsLoading = ref(true)
const optionsError = ref<string | null>(null)

async function loadOptions() {
  optionsLoading.value = true
  optionsError.value = null
  try {
    const [assignments, tasks] = await Promise.all([getAssignments(), getCareTasks()])
    assignmentOptions.value = assignments.data
      .filter((a) => a.status === AssignmentStatus.Active)
      .map((a) => ({ value: a.id, label: `${a.caregiverFullName} → ${a.clientFullName}` }))
    taskOptions.value = tasks.data.filter((t) => t.isActive)
  } catch (err) {
    optionsError.value = getMessage(err, 'Could not load assignments/tasks. Please try again.')
  } finally {
    optionsLoading.value = false
  }
}

onMounted(loadOptions)

const form = reactive({
  assignmentId: null as number | null,
  visitDate: toDateInputValue(new Date()),
  startTime: '',
  endTime: '',
  taskIds: [] as number[],
})

const errors = reactive<Record<string, string | null>>({
  assignmentId: null,
  visitDate: null,
  startTime: null,
  endTime: null,
  taskIds: null,
})

const touched = ref(false)
const submitting = ref(false)
const apiError = ref<string | null>(null)
const apiErrorTitle = ref<string | null>(null)

function validate(): boolean {
  errors.assignmentId = runRules(form.assignmentId, [required('Select an assignment.')])
  errors.visitDate = runRules(form.visitDate, [required('Select a visit date.')])
  errors.startTime = runRules(form.startTime, [required('Select a start time.')])
  errors.endTime = runRules(form.endTime, [required('Select an end time.')])
  errors.taskIds = form.taskIds.length === 0 ? 'Select at least one task.' : null

  if (!errors.startTime && !errors.endTime && form.endTime <= form.startTime) {
    errors.endTime = 'End time must be after the start time.'
  }

  return Object.values(errors).every((e) => e === null)
}

async function handleSubmit() {
  touched.value = true
  if (!validate()) return

  const scheduledStartUtc = combineDateAndTimeToIso(form.visitDate, form.startTime)
  const scheduledEndUtc = combineDateAndTimeToIso(form.visitDate, form.endTime)
  if (!scheduledStartUtc || !scheduledEndUtc) return

  submitting.value = true
  apiError.value = null
  apiErrorTitle.value = null
  try {
    const id = await createVisit({
      caregiverAssignmentId: form.assignmentId!,
      scheduledStartUtc,
      scheduledEndUtc,
      careTaskIds: form.taskIds,
    })
    toast.success('Visit scheduled.')
    router.push(`/admin/visits/${id}`)
  } catch (err) {
    // The backend is the sole authority on scheduling conflicts (availability, double-booking,
    // assignment validity) — this only relabels its 409 response for clarity, it never
    // second-guesses or re-derives the decision itself.
    if (err instanceof ApiError && err.status === 409) {
      apiErrorTitle.value = 'Scheduling Conflict'
      apiError.value = err.message
    } else {
      apiErrorTitle.value = null
      apiError.value = getMessage(err)
    }
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div>
    <PageHeader title="New Visit" />

    <AppCard v-if="optionsLoading" :padded="false"><LoadingState class="py-16" label="Loading assignments and tasks…" /></AppCard>
    <AppCard v-else-if="optionsError" :padded="false">
      <AppAlert tone="danger" class="m-5">{{ optionsError }}</AppAlert>
    </AppCard>

    <AppCard v-else class="max-w-2xl">
      <form class="space-y-5" novalidate @submit.prevent="handleSubmit">
        <AppAlert v-if="apiError" tone="danger" :title="apiErrorTitle ?? undefined">{{ apiError }}</AppAlert>

        <AppSelect
          v-model="form.assignmentId"
          label="Assignment"
          placeholder="Select an active assignment"
          :options="assignmentOptions"
          required
          :error="touched ? errors.assignmentId : null"
        />

        <div class="grid gap-5 sm:grid-cols-3">
          <AppDateInput v-model="form.visitDate" label="Visit Date" :min="toDateInputValue(new Date())" required :error="touched ? errors.visitDate : null" />
          <AppTimeInput v-model="form.startTime" label="Start Time" required :error="touched ? errors.startTime : null" />
          <AppTimeInput v-model="form.endTime" label="End Time" required :error="touched ? errors.endTime : null" />
        </div>

        <div>
          <span class="field-label">Tasks <span class="text-danger-600">*</span></span>
          <div class="grid gap-2 rounded-lg border border-border p-3 sm:grid-cols-2">
            <label v-for="task in taskOptions" :key="task.id" class="flex items-center gap-2 text-sm text-ink">
              <input v-model="form.taskIds" type="checkbox" :value="task.id" class="h-4 w-4 rounded border-border text-brand-600 focus:ring-brand-500" />
              {{ task.name }}
            </label>
            <p v-if="taskOptions.length === 0" class="text-sm text-ink-muted">No active care tasks available.</p>
          </div>
          <p v-if="touched && errors.taskIds" class="field-error">{{ errors.taskIds }}</p>
        </div>

        <div class="flex items-center justify-end gap-2 border-t border-border pt-5">
          <AppButton variant="outline" type="button" @click="router.push('/admin/visits')">Cancel</AppButton>
          <AppButton type="submit" :loading="submitting">Schedule Visit</AppButton>
        </div>
      </form>
    </AppCard>
  </div>
</template>
