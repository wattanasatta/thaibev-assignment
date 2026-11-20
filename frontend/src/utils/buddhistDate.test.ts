import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { calcAge, formatThaiDate, toBE, toCE, toIso } from './buddhistDate'

describe('toBE / toCE', () => {
  it('converts CE to BE by adding 543', () => {
    expect(toBE(1995)).toBe(2538)
    expect(toBE(2026)).toBe(2569)
  })

  it('converts BE to CE by subtracting 543', () => {
    expect(toCE(2538)).toBe(1995)
    expect(toCE(2569)).toBe(2026)
  })

  it('round-trips a year', () => {
    expect(toCE(toBE(2000))).toBe(2000)
  })
})

describe('formatThaiDate', () => {
  it('formats ISO CE date as dd/mm/yyyy in BE', () => {
    expect(formatThaiDate('1995-03-21')).toBe('21/03/2538')
  })

  it('pads single-digit day and month and handles leap day', () => {
    expect(formatThaiDate('2000-02-29')).toBe('29/02/2543')
    expect(formatThaiDate('2001-01-05')).toBe('05/01/2544')
  })

  it('returns an empty string for empty or malformed input', () => {
    expect(formatThaiDate('')).toBe('')
    expect(formatThaiDate('not-a-date')).toBe('')
  })
})

describe('calcAge (year difference rule)', () => {
  beforeEach(() => {
    vi.useFakeTimers()
    vi.setSystemTime(new Date(2026, 10, 20)) // 20 Nov 2026 (local)
  })

  afterEach(() => {
    vi.useRealTimers()
  })

  it('subtracts birth year from current year', () => {
    expect(calcAge('1995-03-21')).toBe(31)
  })

  it('counts a leap-day birth the same way regardless of today', () => {
    expect(calcAge('2000-02-29')).toBe(26)
  })

  it('returns 0 for empty input', () => {
    expect(calcAge('')).toBe(0)
  })
})

describe('toIso', () => {
  it('formats a local Date as ISO yyyy-MM-dd without UTC shifting', () => {
    expect(toIso(new Date(2026, 0, 5))).toBe('2026-01-05')
    expect(toIso(new Date(2000, 1, 29))).toBe('2000-02-29')
  })
})
