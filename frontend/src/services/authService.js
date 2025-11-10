import api from '@/api/axios'
import { ENDPOINTS } from '@/api/endpoints'
import { useAuthStore } from '@/stores/auth'
import { ROUTE_BY_ROLE } from '@/utils/constants'

export const authService = {
  async login(username, password) {
    const response = await api.post(ENDPOINTS.AUTH.LOGIN, {
      username,
      password
    })
    
    const user = response.data
    const authStore = useAuthStore()
    authStore.login(user)
    
    return user
  },
  
  logout() {
    const authStore = useAuthStore()
    authStore.logout()
  },
  
  getDefaultRoute(role) {
    return ROUTE_BY_ROLE[role] || '/login'
  }
}

