import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'
import type { PersonListItem } from '../types/person'
import PersonTable from './PersonTable.vue'

const persons: PersonListItem[] = [
  {
    id: 7,
    fullName: 'สมชาย ใจดี',
    address: '123 ถนนนิมมานเหมินท์ เชียงใหม่',
    birthDate: '1995-03-21',
    age: 31,
  },
  {
    id: 12,
    fullName: 'สมหญิง รักไทย',
    address: '45 ถนนสุขุมวิท กรุงเทพฯ',
    birthDate: '2000-02-29',
    age: 26,
  },
]

describe('PersonTable', () => {
  it('shows a running order in the first column instead of the id', () => {
    const wrapper = mount(PersonTable, { props: { persons, loading: false } })

    const rows = wrapper.findAll('tbody tr')
    expect(rows).toHaveLength(2)
    expect(rows[0].findAll('td')[0].text()).toBe('1')
    expect(rows[1].findAll('td')[0].text()).toBe('2')
  })

  it('formats birth date in Buddhist Era dd/mm/yyyy', () => {
    const wrapper = mount(PersonTable, { props: { persons, loading: false } })

    expect(wrapper.text()).toContain('21/03/2538')
    expect(wrapper.text()).toContain('29/02/2543')
  })

  it('emits view with the person id', async () => {
    const wrapper = mount(PersonTable, { props: { persons, loading: false } })

    await wrapper.findAll('tbody tr')[1].find('button').trigger('click')

    expect(wrapper.emitted('view')?.[0]).toEqual([12])
  })

  it('renders a loading indicator while loading', () => {
    const wrapper = mount(PersonTable, { props: { persons: [], loading: true } })

    expect(wrapper.find('[role="status"]').exists()).toBe(true)
  })

  it('renders an empty state when there are no persons', () => {
    const wrapper = mount(PersonTable, { props: { persons: [], loading: false } })

    expect(wrapper.text()).toContain('ยังไม่มีข้อมูล')
  })
})
