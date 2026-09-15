import { defineStore } from 'pinia'
import { ref, watch } from 'vue'

const COLLAPSED_STORAGE_KEY = 'careconnect.sidebar-collapsed'

/** Layout chrome state shared by every role layout: mobile drawer + desktop collapse. */
export const useUiStore = defineStore('ui', () => {
  const mobileSidebarOpen = ref(false)
  const sidebarCollapsed = ref(localStorage.getItem(COLLAPSED_STORAGE_KEY) === '1')

  watch(sidebarCollapsed, (collapsed) => {
    localStorage.setItem(COLLAPSED_STORAGE_KEY, collapsed ? '1' : '0')
  })

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
