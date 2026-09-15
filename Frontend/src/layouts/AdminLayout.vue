<script setup lang="ts">
import AppHeader from '@/components/layout/AppHeader.vue'
import AppSidebar from '@/components/layout/AppSidebar.vue'
import { ADMIN_NAV } from '@/router/nav'
</script>

<template>
  <div class="flex min-h-screen bg-surface">
    <AppSidebar :items="ADMIN_NAV" />

    <div class="flex min-w-0 flex-1 flex-col">
      <AppHeader />

      <main class="flex-1 p-4 sm:p-6 lg:p-8">
        <RouterView />
      </main>
    </div>
  </div>
</template>
