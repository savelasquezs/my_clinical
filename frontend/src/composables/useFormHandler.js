import { ref, computed } from 'vue'

export function useFormHandler(initialData = {}, validators = {}) {
  const formData = ref({ ...initialData })
  const errors = ref({})
  const touched = ref({})
  
  const isDirty = computed(() => {
    return Object.keys(formData.value).some(key => {
      return formData.value[key] !== initialData[key]
    })
  })
  
  const isValid = computed(() => {
    return Object.keys(errors.value).length === 0
  })
  
  const validate = () => {
    errors.value = {}
    touched.value = {}
    
    Object.keys(validators).forEach(field => {
      const fieldValidators = Array.isArray(validators[field]) 
        ? validators[field] 
        : [validators[field]]
      
      for (const validator of fieldValidators) {
        const result = validator(formData.value[field])
        if (result !== true) {
          errors.value[field] = result
          touched.value[field] = true
          break
        }
      }
    })
    
    return isValid.value
  }
  
  const validateField = (field) => {
    if (!validators[field]) return true
    
    const fieldValidators = Array.isArray(validators[field])
      ? validators[field]
      : [validators[field]]
    
    for (const validator of fieldValidators) {
      const result = validator(formData.value[field])
      if (result !== true) {
        errors.value[field] = result
        touched.value[field] = true
        return false
      }
    }
    
    delete errors.value[field]
    return true
  }
  
  const reset = () => {
    formData.value = { ...initialData }
    errors.value = {}
    touched.value = {}
  }
  
  const setFieldValue = (field, value) => {
    formData.value[field] = value
    if (touched.value[field]) {
      validateField(field)
    }
  }
  
  const setFieldTouched = (field) => {
    touched.value[field] = true
    validateField(field)
  }
  
  return {
    formData,
    errors,
    touched,
    isDirty,
    isValid,
    validate,
    validateField,
    reset,
    setFieldValue,
    setFieldTouched
  }
}

