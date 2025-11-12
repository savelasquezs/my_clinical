import api from '@/api/axios'
import { ENDPOINTS } from '@/api/endpoints'

export const supportService = {
  // Medications
  async getAllMedications() {
    const response = await api.get(ENDPOINTS.SUPPORT.MEDICATIONS)
    return response.data
  },
  
  async createMedication(data) {
    const response = await api.post(ENDPOINTS.SUPPORT.MEDICATIONS, data)
    return response.data
  },
  
  async updateMedication(data) {
    const response = await api.put(ENDPOINTS.SUPPORT.MEDICATIONS, data)
    return response.data
  },
  
  async deleteMedication(id) {
    const response = await api.delete(ENDPOINTS.SUPPORT.DELETE_MEDICATION(id))
    return response.data
  },
  
  // Procedures
  async getAllProcedures() {
    const response = await api.get(ENDPOINTS.SUPPORT.PROCEDURES)
    return response.data
  },
  
  async createProcedure(data) {
    const response = await api.post(ENDPOINTS.SUPPORT.PROCEDURES, data)
    return response.data
  },
  
  async updateProcedure(data) {
    const response = await api.put(ENDPOINTS.SUPPORT.PROCEDURES, data)
    return response.data
  },
  
  async deleteProcedure(id) {
    const response = await api.delete(ENDPOINTS.SUPPORT.DELETE_PROCEDURE(id))
    return response.data
  },
  
  // Diagnostic Aids
  async getAllDiagnosticAids() {
    const response = await api.get(ENDPOINTS.SUPPORT.DIAGNOSTIC_AIDS)
    return response.data
  },
  
  async createDiagnosticAid(data) {
    const response = await api.post(ENDPOINTS.SUPPORT.DIAGNOSTIC_AIDS, data)
    return response.data
  },
  
  async updateDiagnosticAid(data) {
    const response = await api.put(ENDPOINTS.SUPPORT.DIAGNOSTIC_AIDS, data)
    return response.data
  },
  
  async deleteDiagnosticAid(id) {
    const response = await api.delete(ENDPOINTS.SUPPORT.DELETE_DIAGNOSTIC_AID(id))
    return response.data
  }
}

