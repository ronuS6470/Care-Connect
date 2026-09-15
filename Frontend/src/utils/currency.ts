const currencyFormatter = new Intl.NumberFormat(undefined, {
  style: 'currency',
  currency: 'USD',
  minimumFractionDigits: 2,
  maximumFractionDigits: 2,
})

export function formatCurrency(value: number | null | undefined): string {
  if (value === null || value === undefined) return '—'
  return currencyFormatter.format(value)
}
