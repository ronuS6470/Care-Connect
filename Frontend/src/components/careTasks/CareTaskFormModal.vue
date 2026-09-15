<script setup lang="ts">
import { reactive, ref, watch } from 'vue'

import AppAlert from '@/components/common/AppAlert.vue'
import AppButton from '@/components/common/AppButton.vue'
import AppInput from '@/components/common/AppInput.vue'
import AppModal from '@/components/common/AppModal.vue'
import AppTextarea from '@/components/common/AppTextarea.vue'
import type { CareTask, CareTaskPayload } from '@/types/careTask'
import { maxLength, required, runRules } from '@/validation/rules'

const props = withDefaults(
  defineProps<{
    modelValue: boolean
    mode: 'create' | 'edit'
    initial?: CareTask | null
    submitting?: boolean
    apiError?: string | null
  }>(),
  {
    initial: null,
    submitting: false,
    apiError: null,
  },
)

const emit = defineEmits<{
  'update:modelValue': [boolean]
  submit: [CareTaskPayload]
}>()

const form = reactive({ name: '', description: '' })
const errors = reactive<{ name: string | null }>({ name: null })
const touched = ref(false)

watch(
  () => props.modelValue,
  (open) => {
    if (!open) return
    form.name = props.initial?.name ?? ''
    form.description = props.initial?.description ?? ''
    errors.name = null
    touched.value = false
  },
)

function validate(): boolean {
  errors.name = runRules(form.name, [required('Enter a task name.'), maxLength(150)])
  return errors.name === null
}

function handleSubmit() {
  touched.value = true
  if (!validate()) return

  emit('submit', { name: form.name, description: form.description || null })
}
</script>

<template>
  <AppModal :model-value="modelValue" :title="mode === 'create' ? 'New Care Task' : 'Edit Care Task'" @update:model-value="emit('update:modelValue', $event)">
    <form id="care-task-form" class="space-y-5" novalidate @submit.prevent="handleSubmit">
      <AppAlert v-if="apiError" tone="danger">{{ apiError }}</AppAlert>

      <AppInput v-model="form.name" label="Task Name" required :error="touched ? errors.name : null" />
      <AppTextarea v-model="form.description" label="Description" placeholder="Optional" :rows="3" />
    </form>

    <template #footer>
      <AppButton variant="outline" type="button" @click="emit('update:modelValue', false)">Cancel</AppButton>
      <AppButton type="submit" form="care-task-form" :loading="submitting">
        {{ mode === 'create' ? 'Create Task' : 'Save Changes' }}
      </AppButton>
    </template>
  </AppModal>
</template>
