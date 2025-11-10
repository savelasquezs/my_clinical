import { defineStore } from 'pinia'
import { ref } from 'vue'
import { adminService } from '@/services/adminService'

export const useAdminStore = defineStore('admin', () => {
  const patients = ref([])
  const appointments = ref([])
  const invoices = ref([])
  const loading = ref(false)
  const filters = ref({
    patientDni: '',
    dateFrom: null,
    dateTo: null
  })
  
  const setPatients = (data) => {
    patients.value = data
  }
  
  const addPatient = (patient) => {
    patients.value.push(patient)
  }
  
  const updatePatientInList = (dni, updatedPatient) => {
    const index = patients.value.findIndex(p => p.dni === dni)
    if (index !== -1) {
      patients.value[index] = updatedPatient
    }
  }
  
  const loadPatientsIfNeeded = async () => {
    // Si ya hay pacientes en el store, no hacer nada
    if (patients.value.length > 0) {
      return
    }
    
    // Si no hay pacientes, cargarlos
    try {
      loading.value = true
      const patientsData = await adminService.getAllPatients()
      patients.value = patientsData
    } catch (error) {
      console.error('Error loading patients:', error)
      throw error
    } finally {
      loading.value = false
    }
  }
  
  const setAppointments = (data) => {
    appointments.value = data
  }
  
  const addAppointment = (appointment) => {
    appointments.value.push(appointment)
  }
  
  const setInvoices = (data) => {
    invoices.value = data
  }
  
  const addInvoice = (invoice) => {
    invoices.value.push(invoice)
  }
  
  const setLoading = (value) => {
    loading.value = value
  }
  
  const setFilters = (newFilters) => {
    filters.value = { ...filters.value, ...newFilters }
  }
  
  return {
    patients,
    appointments,
    invoices,
    loading,
    filters,
    setPatients,
    addPatient,
    updatePatientInList,
    loadPatientsIfNeeded,
    setAppointments,
    addAppointment,
    setInvoices,
    addInvoice,
    setLoading,
    setFilters
  }
})

