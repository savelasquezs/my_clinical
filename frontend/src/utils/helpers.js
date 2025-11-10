export const formatDni = (dni) => {
  if (!dni) return ''
  return dni.toString().replace(/\B(?=(\d{3})+(?!\d))/g, '.')
}

export const formatPhone = (phone) => {
  if (!phone) return ''
  return phone.toString().replace(/(\d{3})(\d{3})(\d{4})/, '($1) $2-$3')
}

export const debounce = (func, wait) => {
  let timeout
  return function executedFunction(...args) {
    const later = () => {
      clearTimeout(timeout)
      func(...args)
    }
    clearTimeout(timeout)
    timeout = setTimeout(later, wait)
  }
}

export const capitalize = (str) => {
  if (!str) return ''
  return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase()
}

