import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useAuthStore = defineStore('auth', () => {
  const user = ref(null)
  
  const isAuthenticated = computed(() => user.value !== null)
  const userRole = computed(() => user.value?.role || null)
  const userDni = computed(() => user.value?.dni || null)
  const userFullname = computed(() => user.value?.fullname || null)
  const username = computed(() => user.value?.username || null)
  
  const login = (userData) => {
    user.value = userData
    localStorage.setItem('user', JSON.stringify(userData))
  }
  
  const logout = () => {
    user.value = null
    localStorage.removeItem('user')
  }
  
  const checkAuth = () => {
    const storedUser = localStorage.getItem('user')
    if (storedUser) {
      try {
        user.value = JSON.parse(storedUser)
      } catch (e) {
        localStorage.removeItem('user')
      }
    }
  }
  
  return {
    user,
    isAuthenticated,
    userRole,
    userDni,
    userFullname,
    username,
    login,
    logout,
    checkAuth
  }
})

