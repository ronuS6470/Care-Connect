<script setup lang="ts">
import { ref } from 'vue'

import AppAlert from '@/components/common/AppAlert.vue'
import AppButton from '@/components/common/AppButton.vue'
import AppTextarea from '@/components/common/AppTextarea.vue'
import { useApiError } from '@/composables/useApiError'
import { addVisitNote } from '@/services/visitService'
import { maxLength, required, runRules } from '@/validation/rules'

const props = defineProps<{ visitId: number }>()
const emit = defineEmits<{ added: [] }>()

const { getMessage } = useApiError()

const content = ref('')
const error = ref<string | null>(null)
const touched = ref(false)
const submitting = ref(false)
const apiError = ref<string | null>(null)

function validate(): boolean {
  error.value = runRules(content.value, [required('Enter a note.'), maxLength(2000)])
  return error.value === null
}

async function handleSubmit() {
  touched.value = true
  apiError.value = null
  if (!validate()) return

  submitting.value = true
  try {
    await addVisitNote(props.visitId, content.value)
    content.value = ''
    touched.value = false
    emit('added')
  } catch (err) {
    apiError.value = getMessage(err)
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <form class="space-y-3" novalidate @submit.prevent="handleSubmit">
    <AppAlert v-if="apiError" tone="danger">{{ apiError }}</AppAlert>

    <AppTextarea
      v-model="content"
      label="Add a note"
      placeholder="Write a note about this visit…"
      :rows="3"
      required
      :error="touched ? error : null"
    />

    <div class="flex justify-end">
      <AppButton type="submit" size="sm" :loading="submitting">Add Note</AppButton>
    </div>
  </form>
</template>
