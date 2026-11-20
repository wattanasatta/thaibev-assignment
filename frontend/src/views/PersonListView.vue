<script setup lang="ts">
import { onMounted, ref } from 'vue'
import PersonFormModal from '../components/PersonFormModal.vue'
import PersonTable from '../components/PersonTable.vue'
import PersonViewModal from '../components/PersonViewModal.vue'
import { usePersons } from '../composables/usePersons'
import type { PersonDetail } from '../types/person'

const { persons, loading, errorMessage, fetchAll, appendPerson, createPerson } = usePersons()

const showAddModal = ref(false)
const viewingPerson = ref<{ id: number; order: number } | null>(null)

onMounted(fetchAll)

function onSaved(detail: PersonDetail): void {
  appendPerson(detail) // ข้อมูลใหม่ไปต่อท้ายตาราง
  showAddModal.value = false
}

function onView(id: number, order: number): void {
  viewingPerson.value = { id, order }
}
</script>

<template>
  <div class="page">
    <header class="page-header">
      <div>
        <h1>ระบบจัดการข้อมูลบุคคล</h1>
        <p class="subtitle">Person Management — ThaiBev IT Assignment</p>
      </div>
      <!-- IT 01-1: ปุ่ม Add -->
      <button type="button" class="btn btn-primary" @click="showAddModal = true">
        ＋ เพิ่มรายการ
      </button>
    </header>

    <div v-if="errorMessage" class="banner banner-error list-error">
      <span>{{ errorMessage }}</span>
      <button type="button" class="btn btn-ghost" @click="fetchAll">ลองอีกครั้ง</button>
    </div>

    <PersonTable :persons="persons" :loading="loading" @view="onView" />

    <!-- IT 01-2: Add modal -->
    <PersonFormModal
      v-if="showAddModal"
      :create-person="createPerson"
      @close="showAddModal = false"
      @saved="onSaved"
    />

    <!-- IT 01-3: View modal -->
    <PersonViewModal
      v-if="viewingPerson !== null"
      :person-id="viewingPerson.id"
      :order="viewingPerson.order"
      @close="viewingPerson = null"
    />
  </div>
</template>

<style scoped>
.page {
  max-width: 980px;
  margin: 0 auto;
  padding: 2rem 1.25rem 3rem;
}

.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 1.25rem;
  flex-wrap: wrap;
}

.page-header h1 {
  font-size: 1.5rem;
  color: var(--teal-700);
}

.subtitle {
  margin: 0.2rem 0 0;
  color: var(--gray-500);
  font-size: 0.9rem;
}

.list-error {
  margin-bottom: 1rem;
}
</style>
