<script setup lang="ts">
import { computed, ref } from 'vue'

import AppAlert from '@/components/common/AppAlert.vue'
import AppButton from '@/components/common/AppButton.vue'
import AppCard from '@/components/common/AppCard.vue'
import AppTextarea from '@/components/common/AppTextarea.vue'
import { useApiError } from '@/composables/useApiError'
import { useToast } from '@/composables/useToast'
import { checkInVisit, checkOutVisit, completeVisit } from '@/services/visitService'
import { VisitStatus } from '@/types/enums'
import type { Visit } from '@/types/visit'

const props = defineProps<{ visit: Visit }>()

/**
 * Every gate here only decides which button makes sense to *show* — never whether the click
 * itself succeeds. The backend re-checks status, timing (the 15-minute early-check-in window),
 * ownership, and task completion independently on every call; a stale or bypassed client never
 * gets a transition the API wouldn't have allowed anyway.
 */
const canStart = computed(() => props.visit.status === VisitStatus.Scheduled)
const canCheckOut = computed(() => props.visit.status === VisitStatus.InProgress && !props.visit.actualEndUtc)
const canComplete = computed(() => props.visit.status === VisitStatus.InProgress && !!props.visit.actualEndUtc)

const emit = defineEmits<{ updated: [] }>()

const toast = useToast()
const { getMessage: friendlyMessage } = useApiError()

type Action = 'start' | 'checkOut' | 'complete' | null
const pending = ref<Action>(null)
const error = ref<string | null>(null)
const incompleteTasksReason = ref('')

async function handleStart() {
  pending.value = 'start'
  error.value = null
  try {
    await checkInVisit(props.visit.id)
    toast.success('Visit started.')
    emit('updated')
  } catch (err) {
    error.value = friendlyMessage(err)
  } finally {
    pending.value = null
  }
}

async function handleCheckOut() {
  pending.value = 'checkOut'
  error.value = null
  try {
    await checkOutVisit(props.visit.id)
    toast.success('Checked out.')
    emit('updated')
  } catch (err) {
    error.value = friendlyMessage(err)
  } finally {
    pending.value = null
  }
}

async function handleComplete() {
  pending.value = 'complete'
  error.value = null
  try {
    await completeVisit(props.visit.id, incompleteTasksReason.value)
    toast.success('Visit completed.')
    incompleteTasksReason.value = ''
    emit('updated')
  } catch (err) {
    error.value = friendlyMessage(err)
  } finally {
    pending.value = null
  }
}
</script>

<template>
  <AppCard v-if="canStart || canCheckOut || canComplete" title="Visit Actions">
    <AppAlert v-if="error" tone="danger" class="mb-4">{{ error }}</AppAlert>

    <div v-if="canStart">
      <p class="mb-3 text-sm text-ink-muted">This visit hasn't started yet.</p>
      <AppButton :loading="pending === 'start'" @click="handleStart">Start Visit</AppButton>
    </div>

    <div v-else-if="canCheckOut">
      <p class="mb-3 text-sm text-ink-muted">Visit in progress since {{ new Date(visit.actualStartUtc!).toLocaleTimeString() }}.</p>
      <AppButton :loading="pending === 'checkOut'" @click="handleCheckOut">Check Out</AppButton>
    </div>

    <div v-else-if="canComplete" class="space-y-4">
      <p class="text-sm text-ink-muted">Checked out. Mark this visit complete once everything is confirmed.</p>

      <AppTextarea
        v-model="incompleteTasksReason"
        label="Reason for incomplete tasks"
        hint="Only required if some tasks weren't finished — leave blank if every task on this visit is complete."
        :rows="2"
      />

      <AppButton :loading="pending === 'complete'" @click="handleComplete">Complete Visit</AppButton>
    </div>
  </AppCard>
</template>
