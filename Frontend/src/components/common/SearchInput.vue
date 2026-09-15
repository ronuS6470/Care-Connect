<script setup lang="ts">
import { ref, watch } from 'vue'

const props = withDefaults(
  defineProps<{
    modelValue: string
    placeholder?: string
    debounce?: number
  }>(),
  {
    placeholder: 'Search…',
    debounce: 300,
  },
)

const emit = defineEmits<{ 'update:modelValue': [string] }>()

const draft = ref(props.modelValue)
let timeout: ReturnType<typeof setTimeout> | undefined

watch(
  () => props.modelValue,
  (value) => {
    if (value !== draft.value) draft.value = value
  },
)

watch(draft, (value) => {
  clearTimeout(timeout)
  timeout = setTimeout(() => emit('update:modelValue', value), props.debounce)
})

function clear() {
  draft.value = ''
}
</script>

<template>
  <div class="relative">
    <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3 text-ink-muted">
      <svg class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
        <path
          fill-rule="evenodd"
          d="M9 3.5a5.5 5.5 0 100 11 5.5 5.5 0 000-11zM2 9a7 7 0 1112.452 4.391l3.328 3.329a.75.75 0 11-1.06 1.06l-3.329-3.328A7 7 0 012 9z"
          clip-rule="evenodd"
        />
      </svg>
    </div>

    <input
      v-model="draft"
      type="search"
      :placeholder="placeholder"
      class="input-base pl-9"
      :class="draft && 'pr-9'"
    />

    <button
      v-if="draft"
      type="button"
      class="absolute inset-y-0 right-0 flex items-center pr-3 text-ink-muted hover:text-ink"
      aria-label="Clear search"
      @click="clear"
    >
      <svg class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
        <path
          d="M6.28 5.22a.75.75 0 00-1.06 1.06L8.94 10l-3.72 3.72a.75.75 0 101.06 1.06L10 11.06l3.72 3.72a.75.75 0 101.06-1.06L11.06 10l3.72-3.72a.75.75 0 00-1.06-1.06L10 8.94 6.28 5.22z"
        />
      </svg>
    </button>
  </div>
</template>
