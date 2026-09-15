<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'

import AvailabilityFormModal, { type AvailabilityFormValues } from '@/components/caregivers/AvailabilityFormModal.vue'
import AppCard from '@/components/common/AppCard.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import { useApiError } from '@/composables/useApiError'
import { useAsyncData } from '@/composables/useAsyncData'
import { useConfirm } from '@/composables/useConfirm'
import { useCurrentCaregiver } from '@/composables/useCurrentCaregiver'
import { useToast } from '@/composables/useToast'
import {
  createCaregiverAvailability,
  deleteCaregiverAvailability,
  getCaregiverAvailability,
  updateCaregiverAvailability,
} from '@/services/caregiverAvailabilityService'
import type { CaregiverAvailability } from '@/types/caregiver'
import { DAY_OF_WEEK_OPTIONS, dayOfWeekLabel, formatTimeOnly } from '@/utils/date'

const { current: caregiver, loading: caregiverLoading, error: caregiverError, load: loadCaregiver } = useCurrentCaregiver()

const { data: windows, loading, error, load } = useAsyncData(() =>
  caregiver.value ? getCaregiverAvailability(caregiver.value.id) : Promise.resolve([]),
)

onMounted(async () => {
  await loadCaregiver()
  await load()
})

const windowsByDay = computed(() => {
  const map = new Map<number, CaregiverAvailability[]>()
  for (const day of DAY_OF_WEEK_OPTIONS) map.set(day.value, [])
  for (const w of windows.value ?? []) {
    map.get(w.dayOfWeek)?.push(w)
  }
  for (const list of map.values()) list.sort((a, b) => a.startTime.localeCompare(b.startTime))
  return map
})

const toast = useToast()
const confirm = useConfirm()
const { getMessage } = useApiError()

const modalOpen = ref(false)
const modalMode = ref<'create' | 'edit'>('create')
const modalDay = ref(0)
const editingWindow = ref<CaregiverAvailability | null>(null)
const submitting = ref(false)
const apiError = ref<string | null>(null)
const deletingId = ref<number | null>(null)

function openCreate(day: number) {
  modalMode.value = 'create'
  modalDay.value = day
  editingWindow.value = null
  apiError.value = null
  modalOpen.value = true
}

function openEdit(window: CaregiverAvailability) {
  modalMode.value = 'edit'
  modalDay.value = window.dayOfWeek
  editingWindow.value = window
  apiError.value = null
  modalOpen.value = true
}

async function handleSubmit(values: AvailabilityFormValues) {
  if (!caregiver.value) return

  submitting.value = true
  apiError.value = null
  try {
    if (modalMode.value === 'create') {
      await createCaregiverAvailability({
        caregiverId: caregiver.value.id,
        dayOfWeek: modalDay.value,
        startTime: values.startTime,
        endTime: values.endTime,
      })
      toast.success('Availability added.')
    } else if (editingWindow.value) {
      await updateCaregiverAvailability(editingWindow.value.id, {
        dayOfWeek: modalDay.value,
        startTime: values.startTime,
        endTime: values.endTime,
        isActive: values.isActive,
      })
      toast.success('Availability updated.')
    }
    modalOpen.value = false
    await load()
  } catch (err) {
    apiError.value = getMessage(err)
  } finally {
    submitting.value = false
  }
}

async function handleDelete(window: CaregiverAvailability) {
  const confirmed = await confirm.ask({
    title: 'Remove this availability window?',
    message: `${dayOfWeekLabel(window.dayOfWeek)}, ${formatTimeOnly(window.startTime)} – ${formatTimeOnly(window.endTime)} will be removed.`,
    confirmLabel: 'Remove',
    tone: 'danger',
  })
  if (!confirmed) return

  deletingId.value = window.id
  try {
    await deleteCaregiverAvailability(window.id)
    toast.success('Availability removed.')
    await load()
  } catch (err) {
    toast.error(getMessage(err))
  } finally {
    deletingId.value = null
  }
}
</script>

<template>
  <div>
    <PageHeader title="My Availability" description="The weekly windows you're available to be scheduled for visits." />

    <AppCard v-if="caregiverLoading || loading" :padded="false"><LoadingState class="py-16" /></AppCard>
    <AppCard v-else-if="caregiverError" :padded="false"><ErrorState :description="caregiverError" @retry="loadCaregiver" /></AppCard>
    <AppCard v-else-if="error" :padded="false"><ErrorState :description="error" @retry="load" /></AppCard>
    <AppCard v-else-if="!caregiver" :padded="false">
      <EmptyState title="No caregiver profile found" description="This account isn't linked to a caregiver profile." />
    </AppCard>

    <div v-else class="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
      <AppCard v-for="day in DAY_OF_WEEK_OPTIONS" :key="day.value" :title="day.label">
        <ul v-if="windowsByDay.get(day.value)?.length" class="-mx-5 -mt-5 mb-4 divide-y divide-border">
          <li
            v-for="window in windowsByDay.get(day.value)"
            :key="window.id"
            class="flex items-center justify-between gap-2 px-5 py-3"
          >
            <div class="min-w-0">
              <p class="text-sm text-ink">{{ formatTimeOnly(window.startTime) }} – {{ formatTimeOnly(window.endTime) }}</p>
              <p v-if="!window.isActive" class="text-xs text-ink-muted">Inactive</p>
            </div>
            <div class="flex shrink-0 gap-1">
              <button type="button" class="rounded-md p-1.5 text-ink-muted hover:bg-surface-sunken hover:text-ink" aria-label="Edit" @click="openEdit(window)">
                <svg class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                  <path d="M13.586 3.586a2 2 0 112.828 2.828l-.793.793-2.828-2.828.793-.793zM11.379 5.793L3 14.172V17h2.828l8.38-8.379-2.83-2.828z" />
                </svg>
              </button>
              <button
                type="button"
                class="rounded-md p-1.5 text-ink-muted hover:bg-danger-50 hover:text-danger-600 disabled:opacity-50 dark:hover:bg-danger-500/10"
                aria-label="Delete"
                :disabled="deletingId === window.id"
                @click="handleDelete(window)"
              >
                <svg class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                  <path fill-rule="evenodd" d="M8.75 1A2.75 2.75 0 006 3.75v.443c-.795.077-1.584.176-2.365.298a.75.75 0 10.23 1.482l.149-.022.841 10.518A2.75 2.75 0 007.596 19h4.807a2.75 2.75 0 002.742-2.53l.841-10.52.149.023a.75.75 0 00.23-1.482A41.03 41.03 0 0014 4.193V3.75A2.75 2.75 0 0011.25 1h-2.5zM10 4c.84 0 1.673.025 2.5.075V3.75c0-.69-.56-1.25-1.25-1.25h-2.5c-.69 0-1.25.56-1.25 1.25v.325C8.327 4.025 9.16 4 10 4zM8.58 7.72a.75.75 0 00-1.5.06l.3 7.5a.75.75 0 101.5-.06l-.3-7.5zm4.34.06a.75.75 0 10-1.5-.06l-.3 7.5a.75.75 0 101.5.06l.3-7.5z" clip-rule="evenodd" />
                </svg>
              </button>
            </div>
          </li>
        </ul>
        <EmptyState v-else class="-mx-5 -mt-5 mb-4" title="No availability set" />

        <button type="button" class="text-sm font-medium text-brand-600 hover:text-brand-700" @click="openCreate(day.value)">
          + Add
        </button>
      </AppCard>
    </div>

    <AvailabilityFormModal
      v-model="modalOpen"
      :mode="modalMode"
      :day-of-week="modalDay"
      :initial="editingWindow"
      :submitting="submitting"
      :api-error="apiError"
      @submit="handleSubmit"
    />
  </div>
</template>
