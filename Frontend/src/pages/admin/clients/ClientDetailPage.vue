<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'

import AppButton from '@/components/common/AppButton.vue'
import AppCard from '@/components/common/AppCard.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import { useAsyncData } from '@/composables/useAsyncData'
import { getClientById } from '@/services/clientService'
import { activeStatusTone } from '@/utils/status'

const route = useRoute()
const router = useRouter()

const id = computed(() => Number(route.params.id))
const { data: client, loading, error, load } = useAsyncData(() => getClientById(id.value))

onMounted(load)
</script>

<template>
  <div>
    <PageHeader :title="client?.fullName ?? 'Client'">
      <template v-if="client" #actions>
        <AppButton @click="router.push(`/admin/clients/${client.id}/edit`)">Edit</AppButton>
      </template>
    </PageHeader>

    <AppCard v-if="loading" :padded="false"><LoadingState class="py-16" /></AppCard>
    <AppCard v-else-if="error" :padded="false"><ErrorState :description="error" @retry="load" /></AppCard>
    <AppCard v-else-if="!client" :padded="false">
      <EmptyState title="Client not found" description="This client may have been removed." />
    </AppCard>

    <AppCard v-else class="max-w-2xl">
      <dl class="grid gap-x-6 gap-y-5 sm:grid-cols-2">
        <div>
          <dt class="text-xs font-medium text-ink-muted">Status</dt>
          <dd class="mt-1"><StatusBadge :label="client.isActive ? 'Active' : 'Inactive'" :tone="activeStatusTone(client.isActive)" /></dd>
        </div>
        <div>
          <dt class="text-xs font-medium text-ink-muted">Email</dt>
          <dd class="mt-1 text-sm text-ink">{{ client.email }}</dd>
        </div>
        <div>
          <dt class="text-xs font-medium text-ink-muted">Phone</dt>
          <dd class="mt-1 text-sm text-ink">{{ client.phoneNumber ?? '—' }}</dd>
        </div>
        <div class="sm:col-span-2">
          <dt class="text-xs font-medium text-ink-muted">Address</dt>
          <dd class="mt-1 text-sm text-ink">
            {{ client.addressLine1 }}<span v-if="client.addressLine2">, {{ client.addressLine2 }}</span
            >, {{ client.city }}, {{ client.state }} {{ client.postalCode }}
          </dd>
        </div>
        <div>
          <dt class="text-xs font-medium text-ink-muted">Emergency Contact</dt>
          <dd class="mt-1 text-sm text-ink">{{ client.emergencyContactName ?? '—' }}</dd>
        </div>
        <div>
          <dt class="text-xs font-medium text-ink-muted">Emergency Phone</dt>
          <dd class="mt-1 text-sm text-ink">{{ client.emergencyContactPhone ?? '—' }}</dd>
        </div>
      </dl>
    </AppCard>
  </div>
</template>
