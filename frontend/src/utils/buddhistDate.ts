/**
 * เครื่องมือจัดการปี พ.ศ.
 * หลักการ: DB/API เก็บและส่งเป็น ค.ศ. (ISO yyyy-MM-dd) เท่านั้น
 * แปลงเป็น พ.ศ. เฉพาะตอนแสดงผล (ปี พ.ศ. = ปี ค.ศ. + 543)
 */

const BE_OFFSET = 543

export const THAI_MONTHS = [
  'มกราคม',
  'กุมภาพันธ์',
  'มีนาคม',
  'เมษายน',
  'พฤษภาคม',
  'มิถุนายน',
  'กรกฎาคม',
  'สิงหาคม',
  'กันยายน',
  'ตุลาคม',
  'พฤศจิกายน',
  'ธันวาคม',
]

export const toBE = (ceYear: number): number => ceYear + BE_OFFSET
export const toCE = (beYear: number): number => beYear - BE_OFFSET

const pad2 = (n: number): string => String(n).padStart(2, '0')

/** 'yyyy-MM-dd' (ค.ศ.) → 'dd/mm/yyyy' พ.ศ. — ใช้ในตารางและ modal ตาม mockup */
export function formatThaiDate(iso: string): string {
  if (!iso) return ''
  const [y, m, d] = iso.split('-').map(Number)
  if (!y || !m || !d) return ''
  return `${pad2(d)}/${pad2(m)}/${toBE(y)}`
}

/**
 * อายุตามโจทย์: ปีปัจจุบัน - ปีเกิด (ค.ศ. ลบ ค.ศ. = พ.ศ. ลบ พ.ศ.)
 * @param birthIso 'yyyy-MM-dd' ค.ศ.
 */
export function calcAge(birthIso: string): number {
  const birthYear = Number(birthIso.slice(0, 4))
  if (!birthYear) return 0
  return new Date().getFullYear() - birthYear
}

/** Date (local) → 'yyyy-MM-dd' ค.ศ. แบบใช้ date part ของ local (กัน UTC shift) */
export function toIso(date: Date): string {
  return `${date.getFullYear()}-${pad2(date.getMonth() + 1)}-${pad2(date.getDate())}`
}

/** แสดงวันที่ใน date picker input: dd/mm/yyyy พ.ศ. ด้วย Intl (Thai Buddhist calendar) */
const buddhistFormatter = new Intl.DateTimeFormat('th-TH-u-ca-buddhist', {
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
})

export function formatBuddhistInput(date: Date): string {
  return buddhistFormatter.format(date)
}
