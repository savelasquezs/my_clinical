import api from '@/api/axios'
import { ENDPOINTS } from '@/api/endpoints'

export const doctorService = {
  // Orders
  async createOrder(data) {
    const response = await api.post(ENDPOINTS.DOCTOR.ORDERS, data)
    return response.data
  },
  
  async getPatientOrders(patientDni) {
    const response = await api.get(ENDPOINTS.DOCTOR.PATIENT_ORDERS(patientDni))
    return response.data
  },
  
  async addMedicationToOrder(orderNumber, data) {
    const response = await api.post(ENDPOINTS.DOCTOR.ADD_MEDICATION_TO_ORDER(orderNumber), data)
    return response.data
  },
  
  async addProcedureToOrder(orderNumber, data) {
    const response = await api.post(ENDPOINTS.DOCTOR.ADD_PROCEDURE_TO_ORDER(orderNumber), data)
    return response.data
  },
  
  async addDiagnosticAidToOrder(orderNumber, data) {
    const response = await api.post(ENDPOINTS.DOCTOR.ADD_DIAGNOSTIC_AID_TO_ORDER(orderNumber), data)
    return response.data
  },
  
  // Medical Records
  async createMedicalRecord(data) {
    const response = await api.post(ENDPOINTS.DOCTOR.MEDICAL_RECORDS, data)
    return response.data
  },
  
  async getMedicalHistory(patientDni) {
    const response = await api.get(ENDPOINTS.DOCTOR.MEDICAL_HISTORY(patientDni))
    return response.data
  }
}

