<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { personApi } from '../api/personApi'
import type { PersonDetail } from '../types/person'
import { formatThaiDate } from '../utils/buddhistDate'
import BaseModal from './BaseModal.vue'

const props = defineProps<{
  personId: number
}>()

const emit = defineEmits<{
  close: []
}>()

const detail = ref<PersonDetail | null>(null)
const loading = ref(false)
const errorMessage = ref('')

onMounted(async () => {
  loading.value = true
  errorMessage.value = ''
  try {
    detail.value = await personApi.getById(props.personId)
  } catch (error) {
    errorMessage.value =
      error instanceof Error ? error.message : 'โหลดข้อมูลไม่สำเร็จ กรุณาลองใหม่อีกครั้ง'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <!-- IT 01-3: แสดงข้อมูลแบบ read-only มีเฉพาะปุ่ม ปิด -->
  <BaseModal title="ดูรายละเอียดข้อมูลบุคคล" @close="emit('close')">
    <div class="modal-body">
      <div v-if="loading" class="state-loading">
        <div class="spinner" role="status" aria-label="กำลังโหลดข้อมูล"></div>
      </div>

      <div v-else-if="errorMessage" class="banner banner-error">{{ errorMessage }}</div>

      <template v-else-if="detail">

        <div class="field">
          <label>ชื่อ</label>
          <input type="text" :value="detail.firstName" readonly disabled />
        </div>

        <div class="field">
          <label>นามสกุล</label>
          <input type="text" :value="detail.lastName" readonly disabled />
        </div>

        <div class="field">
          <label>วันเกิด</label>
          <input type="text" :value="formatThaiDate(detail.birthDate)" readonly disabled />
        </div>

        <div class="field">
          <label>อายุ</label>
          <input type="text" :value="`${detail.age} ปี`" readonly disabled />
        </div>

        <div class="field">
          <label>ที่อยู่</label>
          <textarea :value="detail.address" readonly disabled rows="3"></textarea>
        </div>
      </template>
    </div>

    <div class="modal-footer">
      <button type="button" class="btn btn-primary" @click="emit('close')">ปิด</button>
    </div>
  </BaseModal>
</template>

<style scoped>
.state-loading {
  padding: 2.5rem 0;
}
</style>
