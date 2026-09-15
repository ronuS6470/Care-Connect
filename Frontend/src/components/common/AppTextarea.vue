<script setup lang="ts">
import { useId } from 'vue'

import { cn } from '@/utils/cn'

withDefaults(
  defineProps<{
    modelValue: string | null
    label?: string
    placeholder?: string
    hint?: string
    error?: string | null
    disabled?: boolean
    required?: boolean
    rows?: number
  }>(),
  {
    disabled: false,
    required: false,
    rows: 4,
  },
)

const emit = defineEmits<{ 'update:modelValue': [string] }>()

const textareaId = useId()
</script>

<template>
  <div>
    <label v-if="label" :for="textareaId" class="field-label">
      {{ label }}
      <span v-if="required" class="text-danger-600">*</span>
    </label>

    <textarea
      :id="textareaId"
      :value="modelValue ?? ''"
      :placeholder="placeholder"
      :disabled="disabled"
      :required="required"
      :rows="rows"
      :class="cn('input-base resize-y', error && 'input-error')"
      :aria-invalid="!!error"
      @input="emit('update:modelValue', ($event.target as HTMLTextAreaElement).value)"
    />

    <p v-if="error" class="field-error">{{ error }}</p>
    <p v-else-if="hint" class="field-hint">{{ hint }}</p>
  </div>
</template>
