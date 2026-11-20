import { ref } from 'vue'
import { personApi } from '../api/personApi'
import type { CreatePersonPayload, PersonDetail, PersonListItem } from '../types/person'

/** state ของหน้ารายการ — แชร์กับ modals ผ่าน PersonListView */
export function usePersons() {
  const persons = ref<PersonListItem[]>([])
  const loading = ref(false)
  const errorMessage = ref('')

  async function fetchAll(): Promise<void> {
    loading.value = true
    errorMessage.value = ''
    try {
      persons.value = await personApi.getAll()
    } catch (error) {
      errorMessage.value = error instanceof Error ? error.message : 'โหลดข้อมูลไม่สำเร็จ'
    } finally {
      loading.value = false
    }
  }

  /** บันทึกสำเร็จ → เพิ่มแถวใหม่ที่ท้ายตาราง (API เรียง Id ASC ข้อมูลใหม่อยู่ท้ายอยู่แล้ว) */
  function appendPerson(detail: PersonDetail): void {
    persons.value.push({
      id: detail.id,
      fullName: detail.fullName,
      address: detail.address,
      birthDate: detail.birthDate,
      age: detail.age,
    })
  }

  async function createPerson(payload: CreatePersonPayload): Promise<PersonDetail> {
    return personApi.create(payload)
  }

  return { persons, loading, errorMessage, fetchAll, appendPerson, createPerson }
}
