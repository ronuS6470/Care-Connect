<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'

import AppCard from '@/components/common/AppCard.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import VisitNoteForm from '@/components/visits/VisitNoteForm.vue'
import VisitTasksCard from '@/components/visits/VisitTasksCard.vue'
import { useAsyncData } from '@/composables/useAsyncData'
import { getVisitById, getVisitNotes } from '@/services/visitService'
import { formatDate, formatDateTime, formatTime } from '@/utils/date'
import { visitStatusLabel, visitStatusTone } from '@/utils/status'

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
    <PageHeader :title="visit ? `${visit.clientFullName} · ${formatDate(visit.scheduledStartUtc)}` : 'Visit'" />

    <AppCard v-if="loading" :padded="false"><LoadingState class="py-16" /></AppCard>
    <AppCard v-else-if="error" :padded="false"><ErrorState :description="error" @retry="load" /></AppCard>
    <AppCard v-else-if="!visit" :padded="false">
      <EmptyState title="Visit not found" description="This visit may have been removed." />
    </AppCard>

    <div v-else class="max-w-3xl space-y-6">
      <AppCard title="Overview">
        <dl class="grid gap-x-6 gap-y-5 sm:grid-cols-2">
          <div>
            <dt class="text-xs font-medium text-ink-muted">Status</dt>
            <dd class="mt-1"><StatusBadge :label="visitStatusLabel(visit.status)" :tone="visitStatusTone(visit.status)" /></dd>
          </div>
          <div>
            <dt class="text-xs font-medium text-ink-muted">Client</dt>
            <dd class="mt-1 text-sm text-ink">{{ visit.clientFullName }}</dd>
          </div>
          <div>
            <dt class="text-xs font-medium text-ink-muted">Caregiver</dt>
            <dd class="mt-1 text-sm text-ink">{{ visit.caregiverFullName }}</dd>
          </div>
          <div>
            <dt class="text-xs font-medium text-ink-muted">Scheduled Date</dt>
            <dd class="mt-1 text-sm text-ink">{{ formatDate(visit.scheduledStartUtc) }}</dd>
          </div>
          <div>
            <dt class="text-xs font-medium text-ink-muted">Scheduled Start</dt>
            <dd class="mt-1 text-sm text-ink">{{ formatTime(visit.scheduledStartUtc) }}</dd>
          </div>
          <div>
            <dt class="text-xs font-medium text-ink-muted">Scheduled End</dt>
            <dd class="mt-1 text-sm text-ink">{{ formatTime(visit.scheduledEndUtc) }}</dd>
          </div>
          <div>
            <dt class="text-xs font-medium text-ink-muted">Check-In</dt>
            <dd class="mt-1 text-sm text-ink">{{ visit.actualStartUtc ? formatDateTime(visit.actualStartUtc) : 'Not checked in' }}</dd>
          </div>
          <div>
            <dt class="text-xs font-medium text-ink-muted">Check-Out</dt>
            <dd class="mt-1 text-sm text-ink">{{ visit.actualEndUtc ? formatDateTime(visit.actualEndUtc) : 'Not checked out' }}</dd>
          </div>
          <div v-if="visit.cancellationReason" class="sm:col-span-2">
            <dt class="text-xs font-medium text-ink-muted">Cancellation Reason</dt>
            <dd class="mt-1 text-sm text-ink">{{ visit.cancellationReason }}</dd>
          </div>
        </dl>
      </AppCard>

      <VisitTasksCard :visit-id="visit.id" :visit-status="visit.status" :tasks="visit.visitTasks" @updated="load" />

      <AppCard title="Notes">
        <LoadingState v-if="notesLoading" class="py-8" label="Loading notes…" />
        <template v-else>
          <ul v-if="notes && notes.length" class="-mx-5 -mt-5 mb-4 divide-y divide-border">
            <li v-for="note in notes" :key="note.id" class="px-5 py-3">
              <p class="text-sm text-ink">{{ note.content }}</p>
              <p class="mt-1 text-xs text-ink-muted">{{ note.authorFullName }} · {{ formatDateTime(note.createdAtUtc) }}</p>
            </li>
          </ul>
          <EmptyState v-else class="-mx-5 -mt-5 mb-4" title="No notes yet" />

          <VisitNoteForm :visit-id="visit.id" @added="loadNotes" />
        </template>
      </AppCard>
    </div>
  </div>
</template>
