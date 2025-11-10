import { useToast as useVueToast } from 'vue-toastification'

export function useToast() {
  const toast = useVueToast()
  
  const success = (message) => {
    toast.success(message, {
      timeout: 3000,
      position: 'top-right'
    })
  }
  
  const error = (message) => {
    toast.error(message, {
      timeout: 4000,
      position: 'top-right'
    })
  }
  
  const warning = (message) => {
    toast.warning(message, {
      timeout: 3000,
      position: 'top-right'
    })
  }
  
  const info = (message) => {
    toast.info(message, {
      timeout: 3000,
      position: 'top-right'
    })
  }
  
  return {
    success,
    error,
    warning,
    info
  }
}

