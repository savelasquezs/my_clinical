import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useNurseStore = defineStore('nurse', () => {
  const visits = ref([])
  const loading = ref(false)
  
  const setVisits = (data) => {
    visits.value = data
  }
  
  const addVisit = (visit) => {
    visits.value.push(visit)
  }
  
  const setLoading = (value) => {
    loading.value = value
  }
  
  return {
    visits,
    loading,
    setVisits,
    addVisit,
    setLoading
  }
})

