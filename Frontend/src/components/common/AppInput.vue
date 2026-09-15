<script setup lang="ts">
import { computed, useId } from 'vue'

import { cn } from '@/utils/cn'

const props = withDefaults(
  defineProps<{
    modelValue: string | number | null
    label?: string
    type?: 'text' | 'email' | 'password' | 'number' | 'tel' | 'search'
    placeholder?: string
    hint?: string
    error?: string | null
    disabled?: boolean
    required?: boolean
    autocomplete?: string
  }>(),
  {
    type: 'text',
    disabled: false,
    required: false,
  },
)

const emit = defineEmits<{
  'update:modelValue': [string | number | null]
  blur: [FocusEvent]
}>()

const inputId = useId()

const displayValue = computed(() => props.modelValue ?? '')

function onInput(event: Event) {
  const target = event.target as HTMLInputElement
  emit('update:modelValue', props.type === 'number' ? target.valueAsNumber : target.value)
}
</script>

<template>
  <div>
    <label v-if="label" :for="inputId" class="field-label">
      {{ label }}
      <span v-if="required" class="text-danger-600">*</span>
    </label>

    <div class="relative">
      <div v-if="$slots.leading" class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3 text-ink-muted">
        <slot name="leading" />
      </div>

      <input
        :id="inputId"
        :type="type"
        :value="displayValue"
        :placeholder="placeholder"
        :disabled="disabled"
        :required="required"
        :autocomplete="autocomplete"
        :class="cn('input-base', error && 'input-error', $slots.leading && 'pl-9')"
        :aria-invalid="!!error"
        :aria-describedby="error ? `${inputId}-error` : hint ? `${inputId}-hint` : undefined"
        @input="onInput"
        @blur="$emit('blur', $event)"
      />

      <div v-if="$slots.trailing" class="absolute inset-y-0 right-0 flex items-center pr-3 text-ink-muted">
        <slot name="trailing" />
      </div>
    </div>

    <p v-if="error" :id="`${inputId}-error`" class="field-error">{{ error }}</p>
    <p v-else-if="hint" :id="`${inputId}-hint`" class="field-hint">{{ hint }}</p>
  </div>
</template>
