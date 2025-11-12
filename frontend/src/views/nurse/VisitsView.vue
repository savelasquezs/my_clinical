<template>
  <div>
    <PageHeader title="Órdenes Disponibles para Visitas de Enfermería">
    </PageHeader>

    <div class="card">
      <LoadingSpinner v-if="nurseStore.loading" />
      <EmptyState 
        v-else-if="nurseStore.availableOrders.length === 0"
        message="No hay órdenes disponibles con visitas de enfermería"
      />
      <CommonTable
        v-else
        :columns="columns"
        :data="nurseStore.availableOrders"
        :has-actions="true"
        @row-click="handleOrderClick"
      >
        <template #actions="{ row }">
          <button 
            @click.stop="handleOrderClick(row)"
            class="btn btn-sm btn-primary"
          >
            Ver Detalles
          </button>
        </template>
      </CommonTable>
    </div>

    <!-- Modal para detalles de orden y crear visita -->
    <CommonModal
      :is-open="isOrderDetailModalOpen"
      title="Detalles de Orden"
      size="xl"
      @close="closeOrderDetailModal"
    >
      <OrderDetailsForNurse
        v-if="selectedOrderNumber"
        :order-number="selectedOrderNumber"
        @create-visit="handleCreateVisit"
      />
    </CommonModal>

    <!-- Modal para crear visita -->
    <CommonModal
      :is-open="isCreateVisitModalOpen"
      title="Crear Visita de Enfermería"
      size="xl"
      @close="closeCreateVisitModal"
    >
      <NurseVisitForm
        v-if="selectedOrderNumber && selectedNurseVisitItemNumber"
        :order-number="selectedOrderNumber"
        :nurse-visit-item-number="selectedNurseVisitItemNumber"
        :patient-dni="selectedPatientDni"
        :order-details="orderDetails"
        @submit="handleSubmit"
        @cancel="closeCreateVisitModal"
      />
    </CommonModal>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useNurseStore } from '@/stores/nurse'
import { nurseService } from '@/services/nurseService'
import { useToast } from '@/composables/useToast'
import { useDate } from '@/composables/useDate'
import PageHeader from '@/components/shared/PageHeader.vue'
import CommonTable from '@/components/shared/CommonTable.vue'
import CommonModal from '@/components/shared/CommonModal.vue'
import LoadingSpinner from '@/components/shared/LoadingSpinner.vue'
import EmptyState from '@/components/shared/EmptyState.vue'
import OrderDetailsForNurse from '@/components/nurse/OrderDetailsForNurse.vue'
import NurseVisitForm from '@/components/forms/NurseVisitForm.vue'

const nurseStore = useNurseStore()
const toast = useToast()
const { formatDate } = useDate()

const isOrderDetailModalOpen = ref(false)
const isCreateVisitModalOpen = ref(false)
const selectedOrderNumber = ref(null)
const selectedNurseVisitItemNumber = ref(null)
const selectedPatientDni = ref(null)
const orderDetails = ref(null)

const columns = [
  { key: 'orderNumber', label: 'Número de Orden' },
  { 
    key: 'creationDate', 
    label: 'Fecha de Creación',
    formatter: (value) => formatDate(value)
  },
  { key: 'itemsCount', label: 'Items' }
]

const handleOrderClick = async (order) => {
  selectedOrderNumber.value = order.orderNumber
  selectedPatientDni.value = order.patientDni
  try {
    orderDetails.value = await nurseStore.loadOrderDetails(order.orderNumber)
    isOrderDetailModalOpen.value = true
  } catch (error) {
    // Error handled by interceptor
  }
}

const handleCreateVisit = (nurseVisitItemNumber) => {
  selectedNurseVisitItemNumber.value = nurseVisitItemNumber
  isOrderDetailModalOpen.value = false
  isCreateVisitModalOpen.value = true
}

const handleSubmit = async (data) => {
  try {
    await nurseService.createNurseVisit(data)
    toast.success('Visita creada exitosamente')
    closeCreateVisitModal()
    // Recargar órdenes disponibles
    await nurseStore.loadAvailableOrders()
  } catch (error) {
    // Error handled by interceptor
  }
}

const closeOrderDetailModal = () => {
  isOrderDetailModalOpen.value = false
  selectedOrderNumber.value = null
  orderDetails.value = null
}

const closeCreateVisitModal = () => {
  isCreateVisitModalOpen.value = false
  selectedNurseVisitItemNumber.value = null
}

onMounted(async () => {
  await nurseStore.loadAvailableOrders()
})
</script>

