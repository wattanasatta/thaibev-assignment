<script setup lang="ts">
import { onBeforeUnmount, onMounted } from 'vue'

const props = defineProps<{
  title: string
}>()

const emit = defineEmits<{
  close: []
}>()

function onKeydown(event: KeyboardEvent) {
  if (event.key === 'Escape') {
    emit('close')
  }
}

onMounted(() => document.addEventListener('keydown', onKeydown))
onBeforeUnmount(() => document.removeEventListener('keydown', onKeydown))

function onOverlayClick(event: MouseEvent) {
  // คลิกนอกการ์ด (บน overlay) = ปิด
  if (event.target === event.currentTarget) {
    emit('close')
  }
}
</script>

<template>
  <div class="modal-overlay" @mousedown="onOverlayClick">
    <div class="modal-card" role="dialog" aria-modal="true" :aria-label="props.title">
      <div class="modal-header">
        <h2>{{ props.title }}</h2>
        <button type="button" class="modal-close" aria-label="ปิดหน้าต่าง" @click="emit('close')">
          ×
        </button>
      </div>
      <slot />
    </div>
  </div>
</template>
