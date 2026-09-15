<script setup lang="ts">
import { onMounted, ref } from 'vue'

import AppCard from '@/components/common/AppCard.vue'
import ClientForm from '@/components/clients/ClientForm.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import { useApiError } from '@/composables/useApiError'
import { useAsyncData } from '@/composables/useAsyncData'
import { useCurrentClientId } from '@/composables/useCurrentClientId'
import { useToast } from '@/composables/useToast'
import { getClientById, updateClient } from '@/services/clientService'
import type { CreateClientPayload, UpdateClientPayload } from '@/types/client'

const { clientId, loading: idLoading, error: idError, load: loadId } = useCurrentClientId()
const toast = useToast()
const { getMessage } = useApiError()

const { data: client, loading: clientLoading, error: clientError, load: loadClient } = useAsyncData(() =>
  clientId.value ? getClientById(clientId.value) : Promise.resolve(null),
)

onMounted(async () => {
  await loadId()
  await loadClient()
})

const submitting = ref(false)
const apiError = ref<string | null>(null)

// ClientForm only reads `initial` once at setup, matching the Admin New/Edit pages where the
// route always remounts it fresh. This page has no separate "away" route to cancel back to, so
// bumping this key forces a remount — discarding any unsaved edits and re-reading the last saved
// `client` value — instead of Cancel silently doing nothing.
const formKey = ref(0)

function handleCancel() {
  formKey.value += 1
  apiError.value = null
}

async function handleSubmit(payload: CreateClientPayload | UpdateClientPayload) {
  if (!clientId.value) return

  submitting.value = true
  apiError.value = null
  try {
    // Create/Update/Delete on this resource are [Authorize(Roles="Admin")] on the backend — this
    // call will 403 for an actual Client session today, surfaced below like any other backend
    // response, matching how availability writes are handled for Caregivers.
    await updateClient(clientId.value, payload as UpdateClientPayload)
    toast.success('Profile updated.')
    await loadClient()
  } catch (err) {
    apiError.value = getMessage(err)
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div>
    <PageHeader title="My Profile" description="Your contact and address information." />

    <AppCard v-if="idLoading || clientLoading" :padded="false"><LoadingState class="py-16" /></AppCard>
    <AppCard v-else-if="idError" :padded="false"><ErrorState :description="idError" @retry="loadId" /></AppCard>
    <AppCard v-else-if="clientError" :padded="false"><ErrorState :description="clientError" @retry="loadClient" /></AppCard>
    <AppCard v-else-if="!clientId || !client" :padded="false">
      <EmptyState title="Profile not available" description="We couldn't find a client profile linked to your account yet." />
    </AppCard>

    <AppCard v-else class="max-w-2xl">
      <ClientForm
        :key="formKey"
        mode="edit"
        :initial="client"
        :submitting="submitting"
        :api-error="apiError"
        hide-active-toggle
        @submit="handleSubmit"
        @cancel="handleCancel"
      />
    </AppCard>
  </div>
</template>
