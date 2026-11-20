<script setup lang="ts">
import type { PersonListItem } from '../types/person'
import { formatThaiDate } from '../utils/buddhistDate'

defineProps<{
  persons: PersonListItem[]
  loading: boolean
}>()

const emit = defineEmits<{
  view: [id: number, order: number]
}>()
</script>

<template>
  <section class="table-card">
    <table>
      <thead>
        <tr>
          <th class="col-id">ลำดับ</th>
          <th>ชื่อ-สกุล</th>
          <th>ที่อยู่</th>
          <th>วันเกิด</th>
          <th class="col-age">อายุ</th>
          <th class="col-actions">การจัดการ</th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="loading">
          <td colspan="6" class="state-cell">
            <div class="spinner" role="status" aria-label="กำลังโหลดข้อมูล"></div>
          </td>
        </tr>
        <tr v-else-if="persons.length === 0">
          <td colspan="6" class="state-cell empty">
            ยังไม่มีข้อมูล — กดปุ่ม "เพิ่มรายการ" เพื่อบันทึกข้อมูลบุคคลคนแรก
          </td>
        </tr>
        <tr v-for="(person, index) in persons" v-else :key="person.id">
          <td class="col-id">{{ index + 1 }}</td>
          <td class="name-cell">{{ person.fullName }}</td>
          <td class="address-cell">{{ person.address }}</td>
          <td>{{ formatThaiDate(person.birthDate) }}</td>
          <td class="col-age">{{ person.age }} ปี</td>
          <td class="col-actions">
            <button type="button" class="btn btn-view" @click="emit('view', person.id, index + 1)">
              View
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </section>
</template>

<style scoped>
.table-card {
  background: #fff;
  border: 1px solid var(--gray-200);
  border-radius: 14px;
  overflow: hidden;
}

table {
  width: 100%;
  border-collapse: collapse;
}

thead {
  background: var(--teal-50);
}

th {
  text-align: left;
  padding: 0.8rem 1rem;
  font-size: 0.875rem;
  color: var(--teal-700);
  font-weight: 700;
  white-space: nowrap;
}

td {
  padding: 0.75rem 1rem;
  border-top: 1px solid var(--gray-200);
  vertical-align: top;
}

tbody tr:hover {
  background: var(--gray-50);
}

.col-id {
  width: 70px;
  color: var(--gray-500);
}

.col-age {
  width: 80px;
  white-space: nowrap;
}

.col-actions {
  width: 110px;
  text-align: center;
}

td.col-actions {
  text-align: center;
}

.name-cell {
  font-weight: 600;
  min-width: 150px;
}

.address-cell {
  color: var(--gray-600);
  max-width: 320px;
}

.state-cell {
  text-align: center;
  padding: 2.5rem 1rem;
  color: var(--gray-500);
}

.state-cell.empty {
  font-size: 0.95rem;
}

@media (max-width: 720px) {
  .address-cell {
    max-width: none;
  }

  table {
    font-size: 0.9rem;
  }
}
</style>
