import axios from 'axios'
import type { CreatePersonPayload, PersonDetail, PersonListItem } from '../types/person'

const client = axios.create({
  baseURL: '/api',
  timeout: 10_000,
})

export class ApiError extends Error {
  readonly fieldErrors: Record<string, string[]>

  constructor(message: string, fieldErrors: Record<string, string[]>) {
    super(message)
    this.fieldErrors = fieldErrors
  }
}

function toApiError(error: unknown): ApiError {
  if (axios.isAxiosError(error)) {
    // ProblemDetails / ValidationProblemDetails จาก API
    const data = error.response?.data as
      | { title?: string; detail?: string; errors?: Record<string, string[]> }
      | undefined

    // normalize key จาก API: ModelState ของ DataAnnotations ใช้ชื่อ property PascalCase
    // ส่วน AppValidationException ใช้ camelCase — ปรับให้เป็น camelCase ทั้งหมดเพื่อจับคู่กับฟอร์ม
    const fieldErrors: Record<string, string[]> = {}
    for (const [key, messages] of Object.entries(data?.errors ?? {})) {
      const camelKey = key.charAt(0).toLowerCase() + key.slice(1)
      fieldErrors[camelKey] = messages
    }
    const firstFieldMessage = Object.values(fieldErrors).flat()[0]
    const message =
      firstFieldMessage ?? data?.detail ?? data?.title ?? 'เกิดข้อผิดพลาดในการเชื่อมต่อเซิร์ฟเวอร์'
    return new ApiError(message, fieldErrors)
  }
  return new ApiError('เกิดข้อผิดพลาดที่ไม่คาดคิด', {})
}

export const personApi = {
  async getAll(): Promise<PersonListItem[]> {
    try {
      const res = await client.get<PersonListItem[]>('/persons')
      return res.data
    } catch (error) {
      throw toApiError(error)
    }
  },

  async getById(id: number): Promise<PersonDetail> {
    try {
      const res = await client.get<PersonDetail>(`/persons/${id}`)
      return res.data
    } catch (error) {
      throw toApiError(error)
    }
  },

  async create(payload: CreatePersonPayload): Promise<PersonDetail> {
    try {
      const res = await client.post<PersonDetail>('/persons', payload)
      return res.data
    } catch (error) {
      throw toApiError(error)
    }
  },
}
