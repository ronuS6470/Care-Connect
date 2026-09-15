<script setup lang="ts">
import { useId } from 'vue'

import { cn } from '@/utils/cn'

withDefaults(
  defineProps<{
    /** yyyy-MM-dd, matching a native date input and CareConnect's DateOnly fields. */
    modelValue: string | null
    label?: string
    hint?: string
    error?: string | null
    disabled?: boolean
    required?: boolean
    min?: string
    max?: string
  }>(),
  {
    disabled: false,
    required: false,
  },
)

const emit = defineEmits<{ 'update:modelValue': [string] }>()

const inputId = useId()
</script>

<template>
  <div>
    <label v-if="label" :for="inputId" class="field-label">
      {{ label }}
      <span v-if="required" class="text-danger-600">*</span>
    </label>

    <input
      :id="inputId"
      type="date"
      :value="modelValue ?? ''"
      :disabled="disabled"
      :required="required"
      :min="min"
      :max="max"
      :class="cn('input-base', error && 'input-error')"
      :aria-invalid="!!error"
      @input="emit('update:modelValue', ($event.target as HTMLInputElement).value)"
    />

    <p v-if="error" class="field-error">{{ error }}</p>
    <p v-else-if="hint" class="field-hint">{{ hint }}</p>
  </div>
</template>
