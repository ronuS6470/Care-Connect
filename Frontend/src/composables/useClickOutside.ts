import { onBeforeUnmount, onMounted, type Ref } from 'vue'

/** Invokes `handler` on any pointer event outside `target` — used by UserMenu/dropdown popovers. */
export function useClickOutside(target: Ref<HTMLElement | null>, handler: () => void) {
  function onPointerDown(event: PointerEvent) {
    const el = target.value
    if (!el || el.contains(event.target as Node)) return
    handler()
  }

  onMounted(() => document.addEventListener('pointerdown', onPointerDown))
  onBeforeUnmount(() => document.removeEventListener('pointerdown', onPointerDown))
}
