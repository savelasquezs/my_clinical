import { ref } from 'vue'

export function useFetch(fetcher) {
  const data = ref(null)
  const loading = ref(false)
  const error = ref(null)
  
  const execute = async (...args) => {
    loading.value = true
    error.value = null
    try {
      const result = await fetcher(...args)
      data.value = result
      return result
    } catch (err) {
      error.value = err
      throw err
    } finally {
      loading.value = false
    }
  }
  
  return {
    data,
    loading,
    error,
    execute
  }
}

