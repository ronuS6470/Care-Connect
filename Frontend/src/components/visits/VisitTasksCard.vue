<script setup lang="ts">
import { computed, ref } from 'vue'

import AppAlert from '@/components/common/AppAlert.vue'
import AppButton from '@/components/common/AppButton.vue'
import AppCard from '@/components/common/AppCard.vue'
import AppTextarea from '@/components/common/AppTextarea.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import { useApiError } from '@/composables/useApiError'
import { useToast } from '@/composables/useToast'
import { completeVisitTask, uncompleteVisitTask, updateVisitTask } from '@/services/visitService'
import { VisitStatus } from '@/types/enums'
import type { VisitTask } from '@/types/visit'
import { formatDateTime } from '@/utils/date'
import { maxLength, runRules } from '@/validation/rules'

const props = defineProps<{ visitId: number; visitStatus: VisitStatus; tasks: VisitTask[] }>()
const emit = defineEmits<{ updated: [] }>()

const toast = useToast()
const { getMessage } = useApiError()

// Only decides which controls to *show* — VisitTaskLookup.EnsureVisitIsEditable re-checks this
// server-side on every write, so a stale client never gets a mutation the API wouldn't allow.
const isEditable = computed(
  () => ![VisitStatus.Completed, VisitStatus.Cancelled, VisitStatus.NoShow].includes(props.visitStatus),
)

const actionError = ref<string | null>(null)
const togglingId = ref<number | null>(null)

const editingId = ref<number | null>(null)
const noteDraft = ref('')
const noteError = ref<string | null>(null)
const savingNoteId = ref<number | null>(null)

async function toggleComplete(task: VisitTask) {
  togglingId.value = task.id
  actionError.value = null
  try {
    if (task.isCompleted) {
      await uncompleteVisitTask(props.visitId, task.id)
    } else {
      await completeVisitTask(props.visitId, task.id)
    }
    emit('updated')
  } catch (err) {
    actionError.value = getMessage(err)
  } finally {
    togglingId.value = null
  }
}

function startEditNotes(task: VisitTask) {
  editingId.value = task.id
  noteDraft.value = task.notes ?? ''
  noteError.value = null
}

function cancelEditNotes() {
  editingId.value = null
  noteError.value = null
}

async function saveNotes(task: VisitTask) {
  noteError.value = runRules(noteDraft.value, [maxLength(300)])
  if (noteError.value) return

  savingNoteId.value = task.id
  try {
    await updateVisitTask(props.visitId, task.id, noteDraft.value || null)
    toast.success('Task notes updated.')
    editingId.value = null
    emit('updated')
  } catch (err) {
    noteError.value = getMessage(err)
  } finally {
    savingNoteId.value = null
  }
}
</script>

<template>
  <AppCard title="Tasks" :description="`${tasks.filter((t) => t.isCompleted).length} of ${tasks.length} completed`">
    <AppAlert v-if="actionError" tone="danger" class="mb-4">{{ actionError }}</AppAlert>

    <ul v-if="tasks.length" class="-mx-5 -my-5 divide-y divide-border">
      <li v-for="task in tasks" :key="task.id" class="px-5 py-3">
        <div class="flex items-start gap-3">
          <component
            :is="isEditable ? 'button' : 'span'"
            :type="isEditable ? 'button' : undefined"
            class="mt-0.5 h-5 w-5 shrink-0 rounded-full disabled:opacity-50"
            :class="task.isCompleted ? 'text-success-600' : isEditable ? 'text-ink-muted hover:text-ink' : 'text-ink-muted'"
            :aria-label="isEditable ? (task.isCompleted ? 'Mark incomplete' : 'Mark complete') : undefined"
            :disabled="isEditable ? togglingId === task.id : undefined"
            @click="isEditable && toggleComplete(task)"
          >
            <svg viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
              <path
                v-if="task.isCompleted"
                fill-rule="evenodd"
                d="M16.704 4.153a.75.75 0 01.143 1.052l-8 10.5a.75.75 0 01-1.127.075l-4.5-4.5a.75.75 0 011.06-1.06l3.894 3.893 7.48-9.817a.75.75 0 011.05-.143z"
                clip-rule="evenodd"
              />
              <path
                v-else
                fill-rule="evenodd"
                d="M10 18a8 8 0 100-16 8 8 0 000 16zm0-2a6 6 0 100-12 6 6 0 000 12z"
                clip-rule="evenodd"
              />
            </svg>
          </component>

          <div class="min-w-0 flex-1">
            <p :class="['text-sm', task.isCompleted ? 'text-ink' : 'text-ink-muted']">{{ task.careTaskName }}</p>
            <p v-if="task.completedAtUtc" class="mt-0.5 text-xs text-ink-muted">Completed {{ formatDateTime(task.completedAtUtc) }}</p>

            <template v-if="editingId === task.id">
              <AppTextarea v-model="noteDraft" class="mt-2" :rows="2" placeholder="Optional notes" :error="noteError" />
              <div class="mt-2 flex gap-2">
                <AppButton size="sm" :loading="savingNoteId === task.id" @click="saveNotes(task)">Save</AppButton>
                <AppButton size="sm" variant="outline" type="button" @click="cancelEditNotes">Cancel</AppButton>
              </div>
            </template>
            <template v-else>
              <p v-if="task.notes" class="mt-0.5 text-xs text-ink-muted">{{ task.notes }}</p>
              <button
                v-if="isEditable"
                type="button"
                class="mt-1 text-xs font-medium text-brand-600 hover:text-brand-700"
                @click="startEditNotes(task)"
              >
                {{ task.notes ? 'Edit notes' : '+ Add notes' }}
              </button>
            </template>
          </div>
        </div>
      </li>
    </ul>
    <EmptyState v-else title="No tasks on this visit" />
  </AppCard>
</template>
