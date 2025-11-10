import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useDoctorStore = defineStore('doctor', () => {
  const orders = ref([])
  const medicalRecords = ref([])
  const loading = ref(false)
  
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
  
  const setLoading = (value) => {
    loading.value = value
  }
  
  return {
    orders,
    medicalRecords,
    loading,
    setOrders,
    addOrder,
    updateOrderInList,
    setMedicalRecords,
    addMedicalRecord,
    setLoading
  }
})

