import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useDoctorStore = defineStore('doctor', () => {
  const orders = ref([])
  const medicalRecords = ref([])
  const allPatients = ref([])
  const loading = ref(false)
  const selectedPatient = ref(null)
  const selectedMedicalRecord = ref(null)
  const selectedOrder = ref(null)
  
  const setOrders = (data) => {
    orders.value = data
  }
  
  const addOrder = (order) => {
    orders.value.push(order)
  }
  
  const updateOrderInList = (orderNumber, updatedOrder) => {
    const index = orders.value.findIndex(o => o.orderNumber === orderNumber)
    if (index !== -1) {
      orders.value[index] = updatedOrder
    }
  }
  
  const setMedicalRecords = (data) => {
    medicalRecords.value = data
  }
  
  const addMedicalRecord = (record) => {
    medicalRecords.value.push(record)
  }
  
  const setAllPatients = (data) => {
    allPatients.value = data
  }
  
  // Pacientes que tienen al menos un registro médico
  const patientsWithRecords = computed(() => {
    if (!allPatients.value.length || !medicalRecords.value.length) {
      return []
    }
    
    const patientDnisWithRecords = new Set(medicalRecords.value.map(mr => mr.patientDni))
    return allPatients.value.filter(p => patientDnisWithRecords.has(p.dni))
  })
  
  const setSelectedPatient = (patient) => {
    selectedPatient.value = patient
  }
  
  const setSelectedMedicalRecord = (record) => {
    selectedMedicalRecord.value = record
  }
  
  const setSelectedOrder = (order) => {
    selectedOrder.value = order
  }
  
  const clearSelection = () => {
    selectedPatient.value = null
    selectedMedicalRecord.value = null
    selectedOrder.value = null
  }
  
  const setLoading = (value) => {
    loading.value = value
  }
  
  return {
    orders,
    medicalRecords,
    allPatients,
    patientsWithRecords,
    loading,
    selectedPatient,
    selectedMedicalRecord,
    selectedOrder,
    setOrders,
    addOrder,
    updateOrderInList,
    setMedicalRecords,
    addMedicalRecord,
    setAllPatients,
    setSelectedPatient,
    setSelectedMedicalRecord,
    setSelectedOrder,
    clearSelection,
    setLoading
  }
})

