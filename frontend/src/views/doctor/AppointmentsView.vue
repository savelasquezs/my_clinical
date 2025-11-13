<template>
  <div>
    <PageHeader title="Citas Disponibles" />

    <div class="card">
      <div v-if="loading" class="flex justify-center p-8">
        <LoadingSpinner />
      </div>
      <div v-else-if="appointments.length === 0" class="p-8">
        <EmptyState 
          message="No hay citas disponibles"
          description="Todas las citas han sido aceptadas o no hay citas programadas" 
        />
      </div>
      <div v-else>
        <CommonTable
          :columns="columns"
          :data="appointments"
          :loading="loading"
          :show-edit="false"
          :show-delete="false"
        >
          <template #actions="{ row }">
            <button @click="handleAcceptAppointment(row)" class="btn btn-sm btn-primary">
              Aceptar
            </button>
          </template>
        </CommonTable>
      </div>
    </div>

    <!-- Modal de Registro Médico -->
    <CommonModal
      :is-open="isMedicalRecordModalOpen"
      title="Crear Registro Médico"
      size="xl"
      @close="closeMedicalRecordModal"
    >
      <MedicalRecordForm
        mode="create"
        :initial-data="selectedAppointmentData"
        @submit="handleCreateMedicalRecord"
        @cancel="closeMedicalRecordModal"
      />
    </CommonModal>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { doctorService } from '@/services/doctorService'
import { useToast } from '@/composables/useToast'
import { useDate } from '@/composables/useDate'
import PageHeader from '@/components/shared/PageHeader.vue'
import CommonTable from '@/components/shared/CommonTable.vue'
import CommonModal from '@/components/shared/CommonModal.vue'
import LoadingSpinner from '@/components/shared/LoadingSpinner.vue'
import EmptyState from '@/components/shared/EmptyState.vue'
import MedicalRecordForm from '@/components/forms/MedicalRecordForm.vue'

const toast = useToast()
const { formatDateTime } = useDate()

const appointments = ref([])
const loading = ref(false)
const isMedicalRecordModalOpen = ref(false)
const selectedAppointmentData = ref(null)

const columns = [
  { key: 'id1', label: 'ID' },
  { key: 'patientDni', label: 'DNI Paciente' },
  { key: 'patientName', label: 'Nombre Paciente' },
  { 
    key: 'date1', 
    label: 'Fecha',
    formatter: (value) => formatDateTime(value)
  }
]

const loadAppointments = async () => {
  loading.value = true
  try {
    const data = await doctorService.getAvailableAppointments()
    appointments.value = data
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

const handleAcceptAppointment = (appointment) => {
  // Preparar datos para prellenar el formulario
  selectedAppointmentData.value = {
    patientDni: appointment.patientDni,
    date: appointment.date1,
    appointmentId: appointment.id1
  }
  
  // Abrir modal de registro médico
  isMedicalRecordModalOpen.value = true
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

    // Crear el registro médico con appointmentId
    await doctorService.createMedicalRecord({
      patientDni: data.patientDni,
      date: data.date,
      consultationReason: data.consultationReason,
      symptoms: data.symptoms,
      diagnosis: data.diagnosis,
      orderNumber: data.orderNumber,
      appointmentId: data.appointmentId
    })
    
    toast.success('Registro médico creado exitosamente. La cita ha sido aceptada.')
    
    // Cerrar modal
    closeMedicalRecordModal()
    
    // Recargar citas disponibles (la cita aceptada ya no aparecerá)
    await loadAppointments()
  } catch (error) {
    // Error handled by interceptor
  }
}

const closeMedicalRecordModal = () => {
  isMedicalRecordModalOpen.value = false
  setTimeout(() => {
    selectedAppointmentData.value = null
  }, 300)
}

onMounted(() => {
  loadAppointments()
})
</script>

