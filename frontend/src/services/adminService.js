import api from '@/api/axios'
import { ENDPOINTS } from '@/api/endpoints'

export const adminService = {
  // Patients
  async getAllPatients() {
    const response = await api.get(ENDPOINTS.ADMIN.PATIENTS)
    return response.data
  },
  
  async getPatientByDni(dni) {
    const response = await api.get(ENDPOINTS.ADMIN.PATIENT_BY_DNI(dni))
    return response.data
  },
  
  async createPatient(data) {
    const response = await api.post(ENDPOINTS.ADMIN.PATIENTS, data)
    return response.data
  },
  
  async updatePatient(dni, data) {
    const response = await api.put(ENDPOINTS.ADMIN.PATIENT_BY_DNI(dni), data)
    return response.data
  },
  
  // Appointments
  async createAppointment(data) {
    const response = await api.post(ENDPOINTS.ADMIN.APPOINTMENTS, data)
    return response.data
  },
  
  async getPatientAppointments(patientDni) {
    const response = await api.get(ENDPOINTS.ADMIN.PATIENT_APPOINTMENTS(patientDni))
    return response.data
  },
  
  // Invoices
  async createInvoice(data) {
    const response = await api.post(ENDPOINTS.ADMIN.INVOICES, data)
    return response.data
  }
}

