<script setup lang="ts">
import type { BreadcrumbItem } from '@/types/common'

defineProps<{ items: BreadcrumbItem[] }>()
</script>

<template>
  <nav aria-label="Breadcrumb">
    <ol class="flex flex-wrap items-center gap-1.5 text-sm text-ink-muted">
      <li v-for="(item, index) in items" :key="index" class="flex items-center gap-1.5">
        <RouterLink v-if="item.to && index < items.length - 1" :to="item.to" class="hover:text-ink hover:underline">
          {{ item.label }}
        </RouterLink>
        <span v-else :class="index === items.length - 1 && 'font-medium text-ink'">{{ item.label }}</span>

        <svg v-if="index < items.length - 1" class="h-3.5 w-3.5 text-ink-muted/60" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
          <path fill-rule="evenodd" d="M7.21 14.77a.75.75 0 010-1.06L10.94 10 7.21 6.29a.75.75 0 111.06-1.06l4.25 4.24a.75.75 0 010 1.06l-4.25 4.24a.75.75 0 01-1.06 0z" clip-rule="evenodd" />
        </svg>
      </li>
    </ol>
  </nav>
</template>
