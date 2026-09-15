<script setup lang="ts">
import { computed } from 'vue'

import { cn } from '@/utils/cn'

const props = defineProps<{
  page: number
  totalPages: number
  totalRecords?: number
  pageSize?: number
}>()

const emit = defineEmits<{ 'update:page': [number] }>()

/** Condensed page list with ellipsis gaps, e.g. 1 … 4 5 [6] 7 8 … 20. */
const pageItems = computed<(number | 'gap')[]>(() => {
  const total = props.totalPages
  const current = props.page
  if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1)

  const items = new Set<number>([1, total, current, current - 1, current + 1])
  const sorted = [...items].filter((n) => n >= 1 && n <= total).sort((a, b) => a - b)

  const result: (number | 'gap')[] = []
  let previous = 0
  for (const n of sorted) {
    if (previous && n - previous > 1) result.push('gap')
    result.push(n)
    previous = n
  }
  return result
})

const rangeLabel = computed(() => {
  if (!props.totalRecords || !props.pageSize) return null
  const start = (props.page - 1) * props.pageSize + 1
  const end = Math.min(props.page * props.pageSize, props.totalRecords)
  return `${start}–${end} of ${props.totalRecords}`
})

function go(page: number) {
  if (page < 1 || page > props.totalPages || page === props.page) return
  emit('update:page', page)
}
</script>

<template>
  <div class="flex flex-col items-center justify-between gap-3 sm:flex-row">
    <p v-if="rangeLabel" class="text-sm text-ink-muted">{{ rangeLabel }}</p>

    <nav class="flex items-center gap-1" aria-label="Pagination">
      <button
        type="button"
        class="btn-base h-8 w-8 border border-border bg-surface-elevated text-ink-muted hover:bg-surface-sunken disabled:opacity-40"
        :disabled="page <= 1"
        aria-label="Previous page"
        @click="go(page - 1)"
      >
        <svg class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
          <path fill-rule="evenodd" d="M12.79 5.23a.75.75 0 010 1.06L9.06 10l3.73 3.71a.75.75 0 11-1.06 1.06l-4.25-4.24a.75.75 0 010-1.06l4.25-4.24a.75.75 0 011.06 0z" clip-rule="evenodd" />
        </svg>
      </button>

      <template v-for="(item, index) in pageItems" :key="index">
        <span v-if="item === 'gap'" class="px-2 text-sm text-ink-muted">…</span>
        <button
          v-else
          type="button"
          :class="
            cn(
              'btn-base h-8 w-8 text-xs font-medium',
              item === page ? 'bg-brand-700 text-white' : 'bg-surface-elevated text-ink hover:bg-surface-sunken',
            )
          "
          :aria-current="item === page ? 'page' : undefined"
          @click="go(item)"
        >
          {{ item }}
        </button>
      </template>

      <button
        type="button"
        class="btn-base h-8 w-8 border border-border bg-surface-elevated text-ink-muted hover:bg-surface-sunken disabled:opacity-40"
        :disabled="page >= totalPages"
        aria-label="Next page"
        @click="go(page + 1)"
      >
        <svg class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
          <path fill-rule="evenodd" d="M7.21 14.77a.75.75 0 010-1.06L10.94 10 7.21 6.29a.75.75 0 111.06-1.06l4.25 4.24a.75.75 0 010 1.06l-4.25 4.24a.75.75 0 01-1.06 0z" clip-rule="evenodd" />
        </svg>
      </button>
    </nav>
  </div>
</template>
