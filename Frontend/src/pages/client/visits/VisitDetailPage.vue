<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'

import AppCard from '@/components/common/AppCard.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useAsyncData } from '@/composables/useAsyncData'
import { getVisitById, getVisitNotes } from '@/services/visitService'
import { formatDate, formatDateTime, formatTime } from '@/utils/date'
import { visitStatusLabel, visitStatusTone } from '@/utils/status'

// Both calls are already row-scoped server-side (VisitAccessResolver.EnsureCanViewVisitAsync) —
// this client only ever gets its own visit and the notes attached to it.
const route = useRoute()

const id = computed(() => Number(route.params.id))
const { data: visit, loading, error, load } = useAsyncData(() => getVisitById(id.value))
const { data: notes, loading: notesLoading, load: loadNotes } = useAsyncData(() => getVisitNotes(id.value))

onMounted(() => {
  load()
  loadNotes()
})
</script>

<template>
  <div>
    <PageHeader :title="visit ? `Visit · ${formatDate(visit.scheduledStartUtc)}` : 'Visit'" />

    <AppCard v-if="loading" :padded="false"><LoadingState class="py-16" /></AppCard>
    <AppCard v-else-if="error" :padded="false"><ErrorState :description="error" @retry="load" /></AppCard>
    <AppCard v-else-if="!visit" :padded="false">
      <EmptyState title="Visit not found" description="This visit may not exist, or may not belong to you." />
    </AppCard>

    <div v-else class="max-w-3xl space-y-6">
      <AppCard title="Overview">
        <dl class="grid gap-x-6 gap-y-5 sm:grid-cols-2">
          <div>
            <dt class="text-xs font-medium text-ink-muted">Status</dt>
            <dd class="mt-1"><StatusBadge :label="visitStatusLabel(visit.status)" :tone="visitStatusTone(visit.status)" /></dd>
          </div>
          <div>
            <dt class="text-xs font-medium text-ink-muted">Caregiver</dt>
            <dd class="mt-1 text-sm text-ink">{{ visit.caregiverFullName }}</dd>
          </div>
          <div>
            <dt class="text-xs font-medium text-ink-muted">Date</dt>
            <dd class="mt-1 text-sm text-ink">{{ formatDate(visit.scheduledStartUtc) }}</dd>
          </div>
          <div>
            <dt class="text-xs font-medium text-ink-muted">Scheduled Time</dt>
            <dd class="mt-1 text-sm text-ink">{{ formatTime(visit.scheduledStartUtc) }} – {{ formatTime(visit.scheduledEndUtc) }}</dd>
          </div>
          <div v-if="visit.cancellationReason" class="sm:col-span-2">
            <dt class="text-xs font-medium text-ink-muted">Cancellation Reason</dt>
            <dd class="mt-1 text-sm text-ink">{{ visit.cancellationReason }}</dd>
          </div>
        </dl>
      </AppCard>

      <AppCard title="Tasks" :description="`${visit.visitTasks.filter((t) => t.isCompleted).length} of ${visit.visitTasks.length} completed`">
        <ul v-if="visit.visitTasks.length" class="-mx-5 -my-5 divide-y divide-border">
          <li v-for="task in visit.visitTasks" :key="task.id" class="flex items-start gap-3 px-5 py-3">
            <svg
              class="mt-0.5 h-5 w-5 shrink-0"
              :class="task.isCompleted ? 'text-success-600' : 'text-ink-muted'"
              viewBox="0 0 20 20"
              fill="currentColor"
              aria-hidden="true"
            >
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
            <p :class="['text-sm', task.isCompleted ? 'text-ink' : 'text-ink-muted']">{{ task.careTaskName }}</p>
          </li>
        </ul>
        <EmptyState v-else title="No tasks on this visit" />
      </AppCard>

      <AppCard title="Notes">
        <LoadingState v-if="notesLoading" class="py-8" label="Loading notes…" />
        <ul v-else-if="notes && notes.length" class="-mx-5 -my-5 divide-y divide-border">
          <li v-for="note in notes" :key="note.id" class="px-5 py-3">
            <p class="text-sm text-ink">{{ note.content }}</p>
            <p class="mt-1 text-xs text-ink-muted">{{ note.authorFullName }} · {{ formatDateTime(note.createdAtUtc) }}</p>
          </li>
        </ul>
        <EmptyState v-else title="No notes yet" />
      </AppCard>
    </div>
  </div>
</template>
