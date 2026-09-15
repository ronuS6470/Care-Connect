import { ref, type Ref } from 'vue'

import { useApiError } from './useApiError'

/** Standard load/error/data trio for a single fetch-on-mount call, shared by dashboard pages. */
export function useAsyncData<T>(fetcher: () => Promise<T>) {
  const data: Ref<T | null> = ref(null)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const { getMessage } = useApiError()

  async function load() {
    loading.value = true
    error.value = null
    try {
      data.value = await fetcher()
    } catch (err) {
      error.value = getMessage(err)
    } finally {
      loading.value = false
    }
  }

  return { data, loading, error, load }
}
