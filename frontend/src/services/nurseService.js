import api from '@/api/axios'
import { ENDPOINTS } from '@/api/endpoints'

export const nurseService = {
  async createNurseVisit(data) {
    const response = await api.post(ENDPOINTS.NURSE.VISITS, data)
    return response.data
  },
  
  async getPatientInfo(patientDni) {
    const response = await api.get(ENDPOINTS.NURSE.PATIENT_INFO(patientDni))
    return response.data
  },

  async getAvailableOrders() {
    const response = await api.get(ENDPOINTS.NURSE.AVAILABLE_ORDERS)
    return response.data
  },

  async getOrderDetails(orderNumber) {
    const response = await api.get(ENDPOINTS.NURSE.ORDER_DETAILS(orderNumber))
    return response.data
  }
}

