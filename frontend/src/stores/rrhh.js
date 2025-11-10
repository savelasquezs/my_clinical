import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useRRHHStore = defineStore('rrhh', () => {
  const users = ref([])
  const loading = ref(false)
  
  const setUsers = (data) => {
    users.value = data
  }
  
  const addUser = (user) => {
    users.value.push(user)
  }
  
  const updateUserInList = (dni, updatedUser) => {
    const index = users.value.findIndex(u => u.dni === dni)
    if (index !== -1) {
      users.value[index] = updatedUser
    }
  }
  
  const removeUser = (dni) => {
    users.value = users.value.filter(u => u.dni !== dni)
  }
  
  const setLoading = (value) => {
    loading.value = value
  }
  
  return {
    users,
    loading,
    setUsers,
    addUser,
    updateUserInList,
    removeUser,
    setLoading
  }
})

