export interface PersonListItem {
  id: number
  fullName: string
  address: string
  /** ค.ศ. ISO yyyy-MM-dd — มาจาก API แล้ว UI จะแสดงเป็น พ.ศ. */
  birthDate: string
  age: number
}

export interface PersonDetail {
  id: number
  firstName: string
  lastName: string
  fullName: string
  birthDate: string
  age: number
  address: string
}

export interface CreatePersonPayload {
  firstName: string
  lastName: string
  /** ค.ศ. ISO yyyy-MM-dd */
  birthDate: string
  address: string
}

export interface FieldErrors {
  [key: string]: string[] | undefined
}
