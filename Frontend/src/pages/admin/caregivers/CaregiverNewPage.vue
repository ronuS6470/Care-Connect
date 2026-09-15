<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'

import CaregiverForm from '@/components/caregivers/CaregiverForm.vue'
import AppCard from '@/components/common/AppCard.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import { useApiError } from '@/composables/useApiError'
import { useToast } from '@/composables/useToast'
import { createCaregiver } from '@/services/caregiverService'
import type { CreateCaregiverPayload, UpdateCaregiverPayload } from '@/types/caregiver'

const router = useRouter()
const toast = useToast()
const { getMessage } = useApiError()

const submitting = ref(false)
const apiError = ref<string | null>(null)

async function handleSubmit(payload: CreateCaregiverPayload | UpdateCaregiverPayload) {
  submitting.value = true
  apiError.value = null
  try {
    const id = await createCaregiver(payload as CreateCaregiverPayload)
    toast.success('Caregiver created.')
    router.push(`/admin/caregivers/${id}`)
  } catch (err) {
    apiError.value = getMessage(err)
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div>
    <PageHeader title="New Caregiver" />

    <AppCard class="max-w-2xl">
      <CaregiverForm
        mode="create"
        :submitting="submitting"
        :api-error="apiError"
        @submit="handleSubmit"
        @cancel="router.push('/admin/caregivers')"
      />
    </AppCard>
  </div>
</template>
