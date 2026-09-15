<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'

import AppCard from '@/components/common/AppCard.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import ClientForm from '@/components/clients/ClientForm.vue'
import { useApiError } from '@/composables/useApiError'
import { useToast } from '@/composables/useToast'
import { createClient } from '@/services/clientService'
import type { CreateClientPayload, UpdateClientPayload } from '@/types/client'

const router = useRouter()
const toast = useToast()
const { getMessage } = useApiError()

const submitting = ref(false)
const apiError = ref<string | null>(null)

async function handleSubmit(payload: CreateClientPayload | UpdateClientPayload) {
  submitting.value = true
  apiError.value = null
  try {
    const id = await createClient(payload as CreateClientPayload)
    toast.success('Client created.')
    router.push(`/admin/clients/${id}`)
  } catch (err) {
    apiError.value = getMessage(err)
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div>
    <PageHeader title="New Client" />

    <AppCard class="max-w-2xl">
      <ClientForm mode="create" :submitting="submitting" :api-error="apiError" @submit="handleSubmit" @cancel="router.push('/admin/clients')" />
    </AppCard>
  </div>
</template>
