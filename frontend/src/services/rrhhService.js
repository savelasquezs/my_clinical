import api from '@/api/axios'
import { ENDPOINTS } from '@/api/endpoints'

export const rrhhService = {
  async getAllUsers() {
    const response = await api.get(ENDPOINTS.RRHH.USERS)
    return response.data
  },
  
  async getUserByUsername(username) {
    const response = await api.get(ENDPOINTS.RRHH.USER_BY_USERNAME(username))
    return response.data
  },
  
  async createUser(data) {
    const response = await api.post(ENDPOINTS.RRHH.USERS, data)
    return response.data
  },
  
  async updateUser(dni, data) {
    const response = await api.put(ENDPOINTS.RRHH.USER_BY_DNI(dni), data)
    return response.data
  },
  
  async deleteUser(dni) {
    const response = await api.delete(ENDPOINTS.RRHH.USER_BY_DNI(dni))
    return response.data
  }
}

