<template>
  <div>
    <PageHeader title="Historia Clínica">
      <template #actions>
        <button v-if="!selectedPatient" @click="openCreateModal" class="btn btn-primary">
          Crear Registro Médico
        </button>
        <button v-else @click="goBack" class="btn btn-secondary">
          Volver
        </button>
      </template>
    </PageHeader>

    <!-- Nivel 1: Lista de pacientes con registros médicos -->
    <div v-if="!selectedPatient" class="card">
      <div v-if="doctorStore.loading" class="flex justify-center p-8">
        <LoadingSpinner />
      </div>
      <div v-else-if="doctorStore.patientsWithRecords.length === 0" class="p-8">
        <EmptyState 
          message="No hay pacientes con registros médicos"
          description="Cree un registro médico para comenzar"
        />
      </div>
      <div v-else>
        <CommonTable
          :columns="patientColumns"
          :data="doctorStore.patientsWithRecords"
          :clickable="true"
          :show-edit="false"
          :show-delete="false"
          @row-click="selectPatient"
        />
      </div>
    </div>

    <!-- Nivel 2: Registros médicos y órdenes del paciente seleccionado -->
    <div v-else class="space-y-6">
      <div class="card">
        <h2 class="text-xl font-semibold mb-4">
          Paciente: {{ selectedPatient.fullname }} ({{ selectedPatient.dni }})
        </h2>
      </div>

      <div class="grid grid-cols-2 gap-6">
        <!-- Columna de Registros Médicos -->
        <div class="card">
          <h3 class="text-lg font-semibold mb-4">Registros Médicos</h3>
          <div v-if="patientMedicalRecords.length === 0" class="text-sm text-gray-500 p-4">
            No hay registros médicos para este paciente
          </div>
          <div v-else class="space-y-2">
            <div
              v-for="record in patientMedicalRecords"
              :key="record.id"
              @click="viewMedicalRecord(record)"
              class="p-4 bg-gray-50 rounded-lg border cursor-pointer hover:bg-gray-100 transition-colors"
            >
              <div class="flex justify-between items-start">
                <div>
                  <p class="font-medium">{{ formatDate(record.date) }}</p>
                  <p class="text-sm text-gray-600 mt-1">{{ record.diagnosis }}</p>
                </div>
                <span v-if="record.orderNumber" class="text-xs text-emerald-600">
                  Orden #{{ record.orderNumber }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Columna de Órdenes -->
        <div class="card">
          <h3 class="text-lg font-semibold mb-4">Órdenes</h3>
          <div v-if="patientOrders.length === 0" class="text-sm text-gray-500 p-4">
            No hay órdenes para este paciente
          </div>
          <div v-else class="space-y-2">
            <div
              v-for="order in patientOrders"
              :key="order.orderNumber"
              @click="viewOrder(order)"
              class="p-4 bg-gray-50 rounded-lg border cursor-pointer hover:bg-gray-100 transition-colors"
            >
              <div class="flex justify-between items-start">
                <div>
                  <p class="font-medium">Orden #{{ order.orderNumber }}</p>
                  <p class="text-sm text-gray-600 mt-1">
                    {{ formatDate(order.creationDate) }}
                  </p>
                  <p class="text-xs text-gray-500 mt-1">
                    {{ order.items?.length || 0 }} item(s)
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal para crear registro médico -->
    <CommonModal
      :is-open="isCreateModalOpen"
      title="Crear Registro Médico"
      size="xl"
      @close="closeCreateModal"
    >
      <MedicalRecordForm
        @submit="handleCreateMedicalRecord"
        @cancel="closeCreateModal"
      />
    </CommonModal>

    <!-- Modal para ver detalle de registro médico -->
    <CommonModal
      v-if="selectedMedicalRecord"
      :is-open="isMedicalRecordDetailOpen && !!selectedMedicalRecord"
      title="Detalle del Registro Médico"
      size="lg"
      @close="closeMedicalRecordDetail"
    >
      <MedicalRecordDetail
        v-if="selectedMedicalRecord"
        :medical-record="selectedMedicalRecord"
        @view-order="handleViewOrderFromRecord"
        @close="closeMedicalRecordDetail"
      />
    </CommonModal>

    <!-- Modal para ver detalle de orden -->
    <CommonModal
      :is-open="isOrderDetailOpen"
      title="Detalle de la Orden"
      size="xl"
      @close="closeOrderDetail"
    >
      <OrderDetail
        v-if="selectedOrder"
        :order="selectedOrder"
        @close="closeOrderDetail"
        @updated="handleOrderUpdated"
      />
    </CommonModal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useDoctorStore } from '@/stores/doctor'
import { useAdminStore } from '@/stores/admin'
import { doctorService } from '@/services/doctorService'
import { adminService } from '@/services/adminService'
import { useToast } from '@/composables/useToast'
import { useDate } from '@/composables/useDate'
import PageHeader from '@/components/shared/PageHeader.vue'
import CommonTable from '@/components/shared/CommonTable.vue'
import CommonModal from '@/components/shared/CommonModal.vue'
import MedicalRecordForm from '@/components/forms/MedicalRecordForm.vue'
import MedicalRecordDetail from '@/components/shared/MedicalRecordDetail.vue'
import OrderDetail from '@/components/shared/OrderDetail.vue'
import LoadingSpinner from '@/components/shared/LoadingSpinner.vue'
import EmptyState from '@/components/shared/EmptyState.vue'

const doctorStore = useDoctorStore()
const adminStore = useAdminStore()
const toast = useToast()
const { formatDate } = useDate()

const isCreateModalOpen = ref(false)
const isMedicalRecordDetailOpen = ref(false)
const isOrderDetailOpen = ref(false)

const selectedPatient = computed(() => doctorStore.selectedPatient)
const selectedMedicalRecord = computed(() => doctorStore.selectedMedicalRecord)
const selectedOrder = computed(() => doctorStore.selectedOrder)

const patientColumns = [
  { key: 'dni', label: 'DNI' },
  { key: 'fullname', label: 'Nombre Completo' },
  { key: 'email', label: 'Email' }
]

const patientMedicalRecords = computed(() => {
  if (!selectedPatient.value) return []
  return doctorStore.medicalRecords.filter(mr => mr.patientDni === selectedPatient.value.dni)
})

const patientOrders = computed(() => {
  if (!selectedPatient.value) return []
  // Las órdenes ya están filtradas por paciente desde loadPatientOrders
  return doctorStore.orders
})

const loadData = async () => {
  doctorStore.setLoading(true)
  try {
    // Cargar todos los pacientes
    const patients = await adminService.getAllPatients()
    doctorStore.setAllPatients(patients)
    adminStore.setPatients(patients)

    // Cargar todos los registros médicos
    const medicalRecords = await doctorService.getAllMedicalRecords()
    doctorStore.setMedicalRecords(medicalRecords)
  } catch (error) {
    console.error('Error loading data:', error)
  } finally {
    doctorStore.setLoading(false)
  }
}

const loadPatientOrders = async (patientDni) => {
  try {
    const orders = await doctorService.getPatientOrders(patientDni)
    doctorStore.setOrders(orders)
  } catch (error) {
    console.error('Error loading patient orders:', error)
  }
}

const selectPatient = async (patient) => {
  doctorStore.setSelectedPatient(patient)
  // Cargar órdenes del paciente
  await loadPatientOrders(patient.dni)
}

const goBack = () => {
  doctorStore.clearSelection()
}

const viewMedicalRecord = (record) => {
  doctorStore.setSelectedMedicalRecord(record)
  isMedicalRecordDetailOpen.value = true
}

const viewOrder = async (order) => {
  try {
    // Cargar la orden completa con items
    const fullOrder = await doctorService.getOrderByNumber(order.orderNumber)
    doctorStore.setSelectedOrder(fullOrder)
    isOrderDetailOpen.value = true
  } catch (error) {
    console.error('Error loading order:', error)
  }
}

const handleViewOrderFromRecord = async (orderNumber) => {
  closeMedicalRecordDetail()
  await viewOrder({ orderNumber })
}

const handleOrderUpdated = async () => {
  // Recargar la orden
  if (selectedPatient.value) {
    await loadPatientOrders(selectedPatient.value.dni)
    if (doctorStore.selectedOrder) {
      const updatedOrder = await doctorService.getOrderByNumber(doctorStore.selectedOrder.orderNumber)
      doctorStore.setSelectedOrder(updatedOrder)
    }
  }
}

const openCreateModal = () => {
  isCreateModalOpen.value = true
}

const closeCreateModal = () => {
  isCreateModalOpen.value = false
}

const closeMedicalRecordDetail = () => {
  isMedicalRecordDetailOpen.value = false
  // Limpiar el registro seleccionado después de que el modal se cierre
  setTimeout(() => {
    doctorStore.setSelectedMedicalRecord(null)
  }, 300) // Esperar a que termine la transición
}

const closeOrderDetail = () => {
  isOrderDetailOpen.value = false
  doctorStore.setSelectedOrder(null)
}

const handleCreateMedicalRecord = async (data) => {
  try {
    // Si viene con orden, crear primero la orden y luego el registro médico
    if (data.order) {
      // Crear la orden con sus items
      const orderData = {
        orderNumber: data.order.orderNumber,
        creationDate: data.order.creationDate,
        items: data.order.items
      }
      await doctorService.createOrder(orderData)
      
      // Asociar la orden al registro médico
      data.orderNumber = data.order.orderNumber
    }

    // Crear el registro médico
    await doctorService.createMedicalRecord({
      patientDni: data.patientDni,
      date: data.date,
      consultationReason: data.consultationReason,
      symptoms: data.symptoms,
      diagnosis: data.diagnosis,
      orderNumber: data.orderNumber
    })

    toast.success('Registro médico creado exitosamente')
    closeCreateModal()
    
    // Recargar datos
    await loadData()
    
    // Si hay un paciente seleccionado, recargar sus órdenes
    if (selectedPatient.value) {
      await loadPatientOrders(selectedPatient.value.dni)
    }
  } catch (error) {
    // Error handled by interceptor
  }
}

onMounted(() => {
  loadData()
})

// Cargar órdenes cuando se selecciona un paciente
watch(selectedPatient, async (patient) => {
  if (patient) {
    await loadPatientOrders(patient.dni)
  }
})
</script>
