import { computed, ref } from 'vue'

export interface UsePaginationOptions {
  initialPage?: number
  pageSize?: number
}

/** Local pagination state for a DataTable + Pagination pair; pair with a server response's totalRecords. */
export function usePagination(options: UsePaginationOptions = {}) {
  const page = ref(options.initialPage ?? 1)
  const pageSize = ref(options.pageSize ?? 20)
  const totalRecords = ref(0)

  const totalPages = computed(() => (pageSize.value <= 0 ? 0 : Math.ceil(totalRecords.value / pageSize.value)))

  function setTotal(total: number) {
    totalRecords.value = total
  }

  function goTo(next: number) {
    page.value = Math.min(Math.max(1, next), Math.max(1, totalPages.value))
  }

  function reset() {
    page.value = 1
  }

  return { page, pageSize, totalRecords, totalPages, setTotal, goTo, reset }
}
