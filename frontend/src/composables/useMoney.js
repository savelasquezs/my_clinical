export function useMoney() {
  const formatMoney = (amount) => {
    if (amount === null || amount === undefined) return '$0.00'
    return new Intl.NumberFormat('es-AR', {
      style: 'currency',
      currency: 'ARS',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(amount)
  }
  
  const parseMoney = (value) => {
    if (!value) return 0
    const cleaned = value.toString().replace(/[^\d.-]/g, '')
    const parsed = parseFloat(cleaned)
    return isNaN(parsed) ? 0 : parsed
  }
  
  return {
    formatMoney,
    parseMoney
  }
}

