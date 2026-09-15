import { customRef } from 'vue'

/** A ref whose reads are immediate but whose writes are debounced — ideal for SearchInput. */
export function useDebouncedRef<T>(initialValue: T, delay = 300) {
  let timeout: ReturnType<typeof setTimeout> | undefined

  return customRef<T>((track, trigger) => {
    let value = initialValue

    return {
      get() {
        track()
        return value
      },
      set(newValue: T) {
        clearTimeout(timeout)
        timeout = setTimeout(() => {
          value = newValue
          trigger()
        }, delay)
      },
    }
  })
}
