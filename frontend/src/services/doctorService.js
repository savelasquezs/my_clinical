import api from '@/api/axios'
import { ENDPOINTS } from '@/api/endpoints'

export const doctorService = {
  // Orders
  async createOrder(data) {
    const response = await api.post(ENDPOINTS.DOCTOR.ORDERS, data)
    return response.data
  },
  
  async getOrderByNumber(orderNumber) {
    const response = await api.get(ENDPOINTS.DOCTOR.ORDER_BY_NUMBER(orderNumber))
    return response.data
  },
  
  async getPatientOrders(patientDni) {
    const response = await api.get(ENDPOINTS.DOCTOR.PATIENT_ORDERS(patientDni))
    return response.data
  },
  
  async addOrderItem(orderNumber, data) {
    const response = await api.post(ENDPOINTS.DOCTOR.ADD_ORDER_ITEM(orderNumber), data)
    return response.data
  },
  
  async updateOrderItem(orderNumber, itemNumber, data) {
    const response = await api.put(ENDPOINTS.DOCTOR.UPDATE_ORDER_ITEM(orderNumber, itemNumber), data)
    return response.data
  },
  
  async deleteOrderItem(orderNumber, itemNumber) {
    const response = await api.delete(ENDPOINTS.DOCTOR.DELETE_ORDER_ITEM(orderNumber, itemNumber))
    return response.data
  },
  
  async getNurseVisitsByOrder(orderNumber) {
    const response = await api.get(ENDPOINTS.DOCTOR.NURSE_VISITS_BY_ORDER(orderNumber))
    return response.data
  },
  
  // Medical Records
  async createMedicalRecord(data) {
    const response = await api.post(ENDPOINTS.DOCTOR.MEDICAL_RECORDS, data)
    return response.data
  },
  
  async getAllMedicalRecords() {
    const response = await api.get(ENDPOINTS.DOCTOR.ALL_MEDICAL_RECORDS)
    return response.data
  },
  
  async getMedicalHistory(patientDni) {
    const response = await api.get(ENDPOINTS.DOCTOR.MEDICAL_HISTORY(patientDni))
    return response.data
  },

  async updateMedicalRecord(id, data) {
    const response = await api.put(ENDPOINTS.DOCTOR.UPDATE_MEDICAL_RECORD(id), data)
    return response.data
  }
}

