import { ref } from 'vue'

export function useConfirm() {
  const isOpen = ref(false)
  const title = ref('')
  const message = ref('')
  const confirmText = ref('Confirmar')
  const cancelText = ref('Cancelar')
  let resolvePromise = null
  
  const confirm = (options = {}) => {
    title.value = options.title || 'Confirmar acción'
    message.value = options.message || '¿Estás seguro?'
    confirmText.value = options.confirmText || 'Confirmar'
    cancelText.value = options.cancelText || 'Cancelar'
    isOpen.value = true
    
    return new Promise((resolve) => {
      resolvePromise = resolve
    })
  }
  
  const handleConfirm = () => {
    isOpen.value = false
    if (resolvePromise) {
      resolvePromise(true)
      resolvePromise = null
    }
  }
  
  const handleCancel = () => {
    isOpen.value = false
    if (resolvePromise) {
      resolvePromise(false)
      resolvePromise = null
    }
  }
  
  return {
    isOpen,
    title,
    message,
    confirmText,
    cancelText,
    confirm,
    handleConfirm,
    handleCancel
  }
}

