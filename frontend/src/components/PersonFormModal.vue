<script setup lang="ts">
import { computed, ref } from 'vue'
import { ApiError } from '../api/personApi'
import type { CreatePersonPayload, PersonDetail } from '../types/person'
import { calcAge, formatBuddhistInput, THAI_MONTHS, toBE, toIso } from '../utils/buddhistDate'
import BaseModal from './BaseModal.vue'

const props = defineProps<{
  /** ผ่านจาก PersonListView (มาจาก usePersons) เพื่อให้ modal ไม่ยุ่งกับ axios โดยตรง */
  createPerson: (payload: CreatePersonPayload) => Promise<PersonDetail>
}>()

const emit = defineEmits<{
  close: []
  saved: [detail: PersonDetail]
}>()

// ปี ค.ศ. ปัจจุบัน (picker ทำงานบน ค.ศ. ภายใน แล้วแสดงผลเป็น พ.ศ.)
const currentCeYear = new Date().getFullYear()
const minDate = new Date(1900, 0, 1)
const maxDate = new Date()

const firstName = ref('')
const lastName = ref('')
const birthDate = ref<Date | null>(null)
const address = ref('')

const fieldErrors = ref<Record<string, string>>({})
const submitError = ref('')
const submitting = ref(false)

/** อายุคำนวณสดทันทีเมื่อเลือกวันเกิด (พ.ศ. ปัจจุบัน - พ.ศ. เกิด) */
const ageLabel = computed(() =>
  birthDate.value ? `${calcAge(toIso(birthDate.value))} ปี` : '-- ปี',
)

function validate(): boolean {
  const errors: Record<string, string> = {}
  if (!firstName.value.trim()) errors.firstName = 'กรุณากรอกชื่อ'
  else if (firstName.value.trim().length > 100) errors.firstName = 'ชื่อต้องไม่เกิน 100 ตัวอักษร'

  if (!lastName.value.trim()) errors.lastName = 'กรุณากรอกนามสกุล'
  else if (lastName.value.trim().length > 100) errors.lastName = 'นามสกุลต้องไม่เกิน 100 ตัวอักษร'

  if (!birthDate.value) errors.birthDate = 'กรุณาเลือกวันเกิด'

  if (!address.value.trim()) errors.address = 'กรุณากรอกที่อยู่'
  else if (address.value.trim().length > 500) errors.address = 'ที่อยู่ต้องไม่เกิน 500 ตัวอักษร'

  fieldErrors.value = errors
  return Object.keys(errors).length === 0
}

async function submit(): Promise<void> {
  submitError.value = ''
  if (!validate()) return

  submitting.value = true // disable ปุ่มระหว่างส่ง กันกดซ้ำเป็น 2 แถว
  try {
    const detail = await props.createPerson({
      firstName: firstName.value.trim(),
      lastName: lastName.value.trim(),
      birthDate: toIso(birthDate.value as Date),
      address: address.value.trim(),
    })
    emit('saved', detail)
  } catch (error) {
    if (error instanceof ApiError) {
      for (const [key, messages] of Object.entries(error.fieldErrors)) {
        if (messages?.length) fieldErrors.value[key] = messages[0]
      }
      submitError.value = Object.keys(error.fieldErrors).length ? '' : error.message
    } else {
      submitError.value = 'บันทึกไม่สำเร็จ กรุณาลองใหม่อีกครั้ง'
    }
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <BaseModal title="เพิ่มข้อมูลบุคคล" @close="emit('close')">
    <form class="modal-body" novalidate @submit.prevent="submit">
      <div class="field" :class="{ invalid: fieldErrors.firstName }">
        <label for="first-name">ชื่อ <span class="required">*</span></label>
        <input
          id="first-name"
          v-model="firstName"
          type="text"
          maxlength="100"
          placeholder="เช่น สมชาย"
          autocomplete="off"
        />
        <span v-if="fieldErrors.firstName" class="field-error">{{ fieldErrors.firstName }}</span>
      </div>

      <div class="field" :class="{ invalid: fieldErrors.lastName }">
        <label for="last-name">นามสกุล <span class="required">*</span></label>
        <input
          id="last-name"
          v-model="lastName"
          type="text"
          maxlength="100"
          placeholder="เช่น ใจดี"
          autocomplete="off"
        />
        <span v-if="fieldErrors.lastName" class="field-error">{{ fieldErrors.lastName }}</span>
      </div>

      <div class="field" :class="{ invalid: fieldErrors.birthDate }">
        <label for="birth-date">วันเกิด <span class="required">*</span></label>
        <VueDatePicker
          id="birth-date"
          v-model="birthDate"
          auto-apply
          :enable-time-picker="false"
          :year-range="[1900, currentCeYear]"
          :min-date="minDate"
          :max-date="maxDate"
          prevent-min-max-navigation
          :text-input="false"
          :formats="{ input: formatBuddhistInput }"
          placeholder="dd/mm/yyyy (พ.ศ.)"
        >
          <!-- header ของปฏิทิน: เดือนไทย + ปี พ.ศ. เลือกผ่าน dropdown 2 ตัว -->
          <template #month-year="{ month, year, years, updateMonthYear }">
            <div class="picker-header">
              <select
                aria-label="เลือกเดือน"
                :value="month"
                @change="updateMonthYear(Number(($event.target as HTMLSelectElement).value), year)"
              >
                <option v-for="(name, index) in THAI_MONTHS" :key="name" :value="index">
                  {{ name }}
                </option>
              </select>
              <select
                aria-label="เลือกปี (พ.ศ.)"
                :value="year"
                @change="updateMonthYear(month, Number(($event.target as HTMLSelectElement).value))"
              >
                <option v-for="y in years" :key="y.value" :value="y.value">{{ toBE(y.value) }}</option>
              </select>
            </div>
          </template>
        </VueDatePicker>
        <span v-if="fieldErrors.birthDate" class="field-error">{{ fieldErrors.birthDate }}</span>
      </div>

      <div class="field">
        <label>อายุ (คำนวณอัตโนมัติ)</label>
        <div class="age-display">{{ ageLabel }}</div>
      </div>

      <div class="field" :class="{ invalid: fieldErrors.address }">
        <label for="address">ที่อยู่ <span class="required">*</span></label>
        <textarea
          id="address"
          v-model="address"
          maxlength="500"
          placeholder="บ้านเลขที่ ถนน ตำบล อำเภอ จังหวัด"
        ></textarea>
        <span v-if="fieldErrors.address" class="field-error">{{ fieldErrors.address }}</span>
      </div>

      <div v-if="submitError" class="banner banner-error submit-error">{{ submitError }}</div>
    </form>

    <div class="modal-footer">
      <!-- ยกเลิก = ปิด modal โดยไม่เรียก API และไม่บันทึกอะไรลงฐานข้อมูล -->
      <button type="button" class="btn btn-ghost" :disabled="submitting" @click="emit('close')">
        ยกเลิก
      </button>
      <button type="button" class="btn btn-primary" :disabled="submitting" @click="submit">
        {{ submitting ? 'กำลังบันทึก...' : 'บันทึก' }}
      </button>
    </div>
  </BaseModal>
</template>

<style scoped>
.submit-error {
  margin-bottom: 0.75rem;
}

.picker-header {
  display: flex;
  gap: 0.5rem;
  justify-content: center;
  padding: 0.5rem 0.5rem 0.25rem;
}

.picker-header select {
  font-family: inherit;
  font-size: 0.9rem;
  font-weight: 600;
  color: var(--gray-900);
  border: 1px solid var(--gray-200);
  border-radius: 8px;
  padding: 0.35rem 0.5rem;
  background: #fff;
  cursor: pointer;
}

.picker-header select:focus {
  outline: 2px solid var(--teal-600);
  outline-offset: -1px;
}
</style>
