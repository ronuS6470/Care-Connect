import { defineStore } from 'pinia'
import { ref } from 'vue'

/** Layout chrome state shared by every role layout: mobile drawer + desktop collapse. */
export const useUiStore = defineStore('ui', () => {
  const mobileSidebarOpen = ref(false)
  const sidebarCollapsed = ref(false)

  function openMobileSidebar() {
    mobileSidebarOpen.value = true
  }

  function closeMobileSidebar() {
    mobileSidebarOpen.value = false
  }

  function toggleSidebarCollapsed() {
    sidebarCollapsed.value = !sidebarCollapsed.value
  }

  return {
    mobileSidebarOpen,
    sidebarCollapsed,
    openMobileSidebar,
    closeMobileSidebar,
    toggleSidebarCollapsed,
  }
})
