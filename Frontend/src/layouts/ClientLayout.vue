<script setup lang="ts">
import SidebarNav from '@/components/layout/SidebarNav.vue'
import TopBar from '@/components/layout/TopBar.vue'
import { CLIENT_NAV } from '@/router/nav'
</script>

<template>
  <div class="flex min-h-screen bg-surface">
    <SidebarNav :items="CLIENT_NAV" />

    <div class="flex min-w-0 flex-1 flex-col">
      <TopBar>
        <p class="truncate text-sm font-medium text-ink-muted">Client workspace</p>
      </TopBar>

      <main class="flex-1 p-4 sm:p-6 lg:p-8">
        <RouterView />
      </main>
    </div>
  </div>
</template>
