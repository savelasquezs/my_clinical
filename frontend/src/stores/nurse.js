import { defineStore } from 'pinia'
import { ref } from 'vue'
import { nurseService } from '@/services/nurseService'

export const useNurseStore = defineStore('nurse', () => {
  const visits = ref([])
  const availableOrders = ref([])
  const selectedOrder = ref(null)
  const loading = ref(false)
  
  const setVisits = (data) => {
    visits.value = data
  }
  
  const addVisit = (visit) => {
    visits.value.push(visit)
  }
  
  const setLoading = (value) => {
    loading.value = value
  }

  const setAvailableOrders = (orders) => {
    availableOrders.value = orders
  }

  const setSelectedOrder = (order) => {
    selectedOrder.value = order
  }

  const loadAvailableOrders = async () => {
    try {
      setLoading(true)
      const orders = await nurseService.getAvailableOrders()
      setAvailableOrders(orders)
    } catch (error) {
      console.error('Error loading available orders:', error)
      throw error
    } finally {
      setLoading(false)
    }
  }

  const loadOrderDetails = async (orderNumber) => {
    try {
      setLoading(true)
      const order = await nurseService.getOrderDetails(orderNumber)
      setSelectedOrder(order)
      return order
    } catch (error) {
      console.error('Error loading order details:', error)
      throw error
    } finally {
      setLoading(false)
    }
  }
  
  return {
    visits,
    availableOrders,
    selectedOrder,
    loading,
    setVisits,
    addVisit,
    setLoading,
    setAvailableOrders,
    setSelectedOrder,
    loadAvailableOrders,
    loadOrderDetails
  }
})

