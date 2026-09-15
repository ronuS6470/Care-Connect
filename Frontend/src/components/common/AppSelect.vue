<script setup lang="ts" generic="T extends string | number">
import { useId } from 'vue'

import { cn } from '@/utils/cn'
import type { SelectOption } from '@/types/common'

const props = withDefaults(
  defineProps<{
    modelValue: T | null
    options: SelectOption<T>[]
    label?: string
    placeholder?: string
    hint?: string
    error?: string | null
    disabled?: boolean
    required?: boolean
  }>(),
  {
    disabled: false,
    required: false,
  },
)

const emit = defineEmits<{ 'update:modelValue': [T | null] }>()

const selectId = useId()

function onChange(event: Event) {
  const raw = (event.target as HTMLSelectElement).value
  if (raw === '') {
    emit('update:modelValue', null)
    return
  }
  const match = props.options.find((option) => String(option.value) === raw)
  emit('update:modelValue', match ? match.value : (raw as T))
}
</script>

<template>
  <div>
    <label v-if="label" :for="selectId" class="field-label">
      {{ label }}
      <span v-if="required" class="text-danger-600">*</span>
    </label>

    <select
      :id="selectId"
      :value="modelValue ?? ''"
      :disabled="disabled"
      :required="required"
      :class="cn('input-base appearance-none bg-no-repeat pr-9', error && 'input-error')"
      style="
        background-image: url('data:image/svg+xml;utf8,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 20 20%22 fill=%22%236b7089%22><path fill-rule=%22evenodd%22 d=%22M5.23 7.21a.75.75 0 011.06.02L10 10.94l3.71-3.71a.75.75 0 111.06 1.06l-4.24 4.25a.75.75 0 01-1.06 0L5.21 8.29a.75.75 0 01.02-1.08z%22 clip-rule=%22evenodd%22/></svg>');
        background-position: right 0.6rem center;
        background-size: 1.1em;
      "
      :aria-invalid="!!error"
      @change="onChange"
    >
      <option v-if="placeholder" value="" disabled>{{ placeholder }}</option>
      <option v-for="option in options" :key="String(option.value)" :value="option.value" :disabled="option.disabled">
        {{ option.label }}
      </option>
    </select>

    <p v-if="error" class="field-error">{{ error }}</p>
    <p v-else-if="hint" class="field-hint">{{ hint }}</p>
  </div>
</template>
