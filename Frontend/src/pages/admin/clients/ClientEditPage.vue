<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'

import AppCard from '@/components/common/AppCard.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import ClientForm from '@/components/clients/ClientForm.vue'
import { useApiError } from '@/composables/useApiError'
import { useAsyncData } from '@/composables/useAsyncData'
import { useToast } from '@/composables/useToast'
import { getClientById, updateClient } from '@/services/clientService'
import type { CreateClientPayload, UpdateClientPayload } from '@/types/client'

const route = useRoute()
const router = useRouter()
const toast = useToast()
const { getMessage } = useApiError()

const id = computed(() => Number(route.params.id))
const { data: client, loading, error, load } = useAsyncData(() => getClientById(id.value))

onMounted(load)

const submitting = ref(false)
const apiError = ref<string | null>(null)

async function handleSubmit(payload: CreateClientPayload | UpdateClientPayload) {
  submitting.value = true
  apiError.value = null
  try {
    await updateClient(id.value, payload as UpdateClientPayload)
    toast.success('Client updated.')
    router.push(`/admin/clients/${id.value}`)
  } catch (err) {
    apiError.value = getMessage(err)
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div>
    <PageHeader :title="client ? `Edit ${client.fullName}` : 'Edit Client'" />

    <AppCard v-if="loading" :padded="false"><LoadingState class="py-16" /></AppCard>
    <AppCard v-else-if="error" :padded="false"><ErrorState :description="error" @retry="load" /></AppCard>
    <AppCard v-else-if="!client" :padded="false">
      <EmptyState title="Client not found" description="This client may have been removed." />
    </AppCard>

    <AppCard v-else class="max-w-2xl">
      <ClientForm
        mode="edit"
        :initial="client"
        :submitting="submitting"
        :api-error="apiError"
        @submit="handleSubmit"
        @cancel="router.push(`/admin/clients/${id}`)"
      />
    </AppCard>
  </div>
</template>
