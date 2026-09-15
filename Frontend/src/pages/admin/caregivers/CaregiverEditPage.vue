<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'

import CaregiverForm from '@/components/caregivers/CaregiverForm.vue'
import AppCard from '@/components/common/AppCard.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ErrorState from '@/components/common/ErrorState.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import { useApiError } from '@/composables/useApiError'
import { useAsyncData } from '@/composables/useAsyncData'
import { useToast } from '@/composables/useToast'
import { getCaregiverById, updateCaregiver } from '@/services/caregiverService'
import type { CreateCaregiverPayload, UpdateCaregiverPayload } from '@/types/caregiver'

const route = useRoute()
const router = useRouter()
const toast = useToast()
const { getMessage } = useApiError()

const id = computed(() => Number(route.params.id))
const { data: caregiver, loading, error, load } = useAsyncData(() => getCaregiverById(id.value))

onMounted(load)

const submitting = ref(false)
const apiError = ref<string | null>(null)

async function handleSubmit(payload: CreateCaregiverPayload | UpdateCaregiverPayload) {
  submitting.value = true
  apiError.value = null
  try {
    await updateCaregiver(id.value, payload as UpdateCaregiverPayload)
    toast.success('Caregiver updated.')
    router.push(`/admin/caregivers/${id.value}`)
  } catch (err) {
    apiError.value = getMessage(err)
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div>
    <PageHeader :title="caregiver ? `Edit ${caregiver.fullName}` : 'Edit Caregiver'" />

    <AppCard v-if="loading" :padded="false"><LoadingState class="py-16" /></AppCard>
    <AppCard v-else-if="error" :padded="false"><ErrorState :description="error" @retry="load" /></AppCard>
    <AppCard v-else-if="!caregiver" :padded="false">
      <EmptyState title="Caregiver not found" description="This caregiver may have been removed." />
    </AppCard>

    <AppCard v-else class="max-w-2xl">
      <CaregiverForm
        mode="edit"
        :initial="caregiver"
        :submitting="submitting"
        :api-error="apiError"
        @submit="handleSubmit"
        @cancel="router.push(`/admin/caregivers/${id}`)"
      />
    </AppCard>
  </div>
</template>
