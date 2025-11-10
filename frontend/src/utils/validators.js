export const validators = {
  required: (value) => {
    if (!value || (typeof value === 'string' && !value.trim())) {
      return 'Este campo es requerido'
    }
    return true
  },
  
  email: (value) => {
    if (!value) return true
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    if (!emailRegex.test(value)) {
      return 'Email inválido'
    }
    return true
  },
  
  minLength: (min) => (value) => {
    if (!value) return true
    if (value.length < min) {
      return `Debe tener al menos ${min} caracteres`
    }
    return true
  },
  
  maxLength: (max) => (value) => {
    if (!value) return true
    if (value.length > max) {
      return `Debe tener máximo ${max} caracteres`
    }
    return true
  },
  
  numeric: (value) => {
    if (!value) return true
    if (isNaN(value)) {
      return 'Debe ser un número'
    }
    return true
  },
  
  positive: (value) => {
    if (!value) return true
    if (parseFloat(value) <= 0) {
      return 'Debe ser un número positivo'
    }
    return true
  },
  
  dateNotInFuture: (value) => {
    if (!value) return true
    const date = new Date(value)
    if (date > new Date()) {
      return 'La fecha no puede ser en el futuro'
    }
    return true
  },
  
  dateNotInPast: (value) => {
    if (!value) return true
    const date = new Date(value)
    if (date < new Date()) {
      return 'La fecha no puede ser en el pasado'
    }
    return true
  }
}

