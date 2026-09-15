<script setup lang="ts" generic="T extends Record<string, unknown>">
import LoadingState from './LoadingState.vue'
import EmptyState from './EmptyState.vue'

export interface DataTableColumn<T> {
  key: string
  label: string
  align?: 'left' | 'right' | 'center'
  /** Falls back to row[key] when omitted — set this for computed/derived cell values. */
  value?: (row: T) => unknown
  headerClass?: string
  cellClass?: string
}

withDefaults(
  defineProps<{
    columns: DataTableColumn<T>[]
    rows: T[]
    rowKey: (row: T) => string | number
    loading?: boolean
    emptyTitle?: string
    emptyDescription?: string
  }>(),
  {
    loading: false,
    emptyTitle: 'Nothing to show yet',
  },
)

function cellValue(column: DataTableColumn<T>, row: T): unknown {
  return column.value ? column.value(row) : row[column.key]
}

const alignClass: Record<NonNullable<DataTableColumn<T>['align']>, string> = {
  left: 'text-left',
  right: 'text-right',
  center: 'text-center',
}

defineSlots<{
  [key: `cell-${string}`]: (props: { row: T; value: unknown }) => unknown
  default?: () => unknown
}>()
</script>

<template>
  <div class="card-base overflow-hidden">
    <LoadingState v-if="loading" class="py-16" label="Loading…" />

    <EmptyState v-else-if="rows.length === 0" class="py-16" :title="emptyTitle" :description="emptyDescription" />

    <template v-else>
      <!-- Desktop / tablet: standard table -->
      <div class="hidden overflow-x-auto md:block">
        <table class="w-full text-sm">
          <thead>
            <tr class="border-b border-border bg-surface-sunken/60">
              <th
                v-for="column in columns"
                :key="column.key"
                scope="col"
                :class="['px-4 py-3 font-medium text-ink-muted', alignClass[column.align ?? 'left'], column.headerClass]"
              >
                {{ column.label }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="row in rows" :key="rowKey(row)" class="border-b border-border last:border-0 hover:bg-surface-sunken/40">
              <td
                v-for="column in columns"
                :key="column.key"
                :class="['px-4 py-3 text-ink', alignClass[column.align ?? 'left'], column.cellClass]"
              >
                <slot :name="`cell-${column.key}`" :row="row" :value="cellValue(column, row)">
                  {{ cellValue(column, row) }}
                </slot>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Mobile: stacked cards, same slots so custom cell rendering carries over -->
      <ul class="divide-y divide-border md:hidden">
        <li v-for="row in rows" :key="rowKey(row)" class="space-y-2 px-4 py-4">
          <div v-for="column in columns" :key="column.key" class="flex items-start justify-between gap-4 text-sm">
            <span class="shrink-0 font-medium text-ink-muted">{{ column.label }}</span>
            <span class="text-right text-ink">
              <slot :name="`cell-${column.key}`" :row="row" :value="cellValue(column, row)">
                {{ cellValue(column, row) }}
              </slot>
            </span>
          </div>
        </li>
      </ul>
    </template>
  </div>
</template>
