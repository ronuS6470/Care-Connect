import { ref, type Ref } from 'vue'

import { ApiError } from '@/services/http'

/** Standard load/error/data trio for a single fetch-on-mount call, shared by dashboard pages. */
export function useAsyncData<T>(fetcher: () => Promise<T>) {
  const data: Ref<T | null> = ref(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function load() {
    loading.value = true
    error.value = null
    try {
      data.value = await fetcher()
    } catch (err) {
      error.value = err instanceof ApiError ? err.message : 'Something went wrong. Please try again.'
    } finally {
      loading.value = false
    }
  }

  return { data, loading, error, load }
}
