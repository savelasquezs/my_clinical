<template>
  <div>
    <PageHeader title="Citas">
      <template #actions>
        <button v-if="activeTab === 'pending'" @click="openCreateModal" class="btn btn-primary">
          Crear Cita
        </button>
      </template>
    </PageHeader>

    <!-- Tabs -->
    <div class="card mb-6">
      <div class="border-b border-gray-200">
        <nav class="-mb-px flex space-x-8">
          <button
            @click="activeTab = 'pending'"
            :class="[
              'py-4 px-1 border-b-2 font-medium text-sm',
              activeTab === 'pending'
                ? 'border-blue-500 text-blue-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            ]"
          >
            Pendientes
          </button>
          <button
            @click="activeTab = 'accepted'"
            :class="[
              'py-4 px-1 border-b-2 font-medium text-sm',
              activeTab === 'accepted'
                ? 'border-blue-500 text-blue-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            ]"
          >
            Aceptadas
          </button>
        </nav>
      </div>
    </div>

    <div class="card">
      <CommonTable
        :columns="columns"
        :data="filteredAppointments"
        :loading="adminStore.loading"
        @edit="handleEdit"
        @delete="handleDelete"
      />
    </div>

    <CommonModal
      :is-open="isModalOpen"
      :title="modalTitle"
      @close="closeModal"
    >
      <AppointmentForm
        :mode="formMode"
        :initial-data="selectedAppointment"
        @submit="handleSubmit"
        @cancel="closeModal"
      />
    </CommonModal>

    <DeleteModal
      :is-open="isDeleteModalOpen"
      title="Cancelar Cita"
      :message="`¿Está seguro de que desea cancelar la cita #${appointmentToDelete?.id1}?`"
      @confirm="confirmDelete"
      @cancel="cancelDelete"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAdminStore } from '@/stores/admin'
import { adminService } from '@/services/adminService'
import { useToast } from '@/composables/useToast'
import { useDate } from '@/composables/useDate'
import PageHeader from '@/components/shared/PageHeader.vue'
import CommonTable from '@/components/shared/CommonTable.vue'
import CommonModal from '@/components/shared/CommonModal.vue'
import DeleteModal from '@/components/shared/DeleteModal.vue'
import AppointmentForm from '@/components/forms/AppointmentForm.vue'

const adminStore = useAdminStore()
const toast = useToast()
const { formatDateTime } = useDate()

const activeTab = ref('pending')
const isModalOpen = ref(false)
const isDeleteModalOpen = ref(false)
const formMode = ref('create')
const selectedAppointment = ref(null)
const appointmentToDelete = ref(null)

const modalTitle = computed(() => 
  formMode.value === 'create' ? 'Crear Cita' : 'Editar Cita'
)

const filteredAppointments = computed(() => {
  if (activeTab.value === 'pending') {
    return adminStore.appointments.filter(a => !a.isAccepted1)
  } else {
    return adminStore.appointments.filter(a => a.isAccepted1)
  }
})

const columns = [
  { key: 'id1', label: 'ID' },
  { key: 'patientDni', label: 'DNI Paciente' },
  { key: 'patientName', label: 'Nombre Paciente' },
  { 
    key: 'date1', 
    label: 'Fecha',
    formatter: (value) => formatDateTime(value)
  },
  {
    key: 'isAccepted1',
    label: 'Estado',
    formatter: (value) => value ? 'Aceptada' : 'Pendiente'
  }
]

const openCreateModal = () => {
  formMode.value = 'create'
  selectedAppointment.value = null
  isModalOpen.value = true
}

const handleEdit = (appointment) => {
  formMode.value = 'edit'
  selectedAppointment.value = appointment
  isModalOpen.value = true
}

const handleDelete = (appointment) => {
  appointmentToDelete.value = appointment
  isDeleteModalOpen.value = true
}

const handleSubmit = async (data) => {
  try {
    if (formMode.value === 'create') {
      await adminService.createAppointment(data)
      toast.success('Cita creada exitosamente')
    } else {
      await adminService.updateAppointment(selectedAppointment.value.id1, data)
      toast.success('Cita actualizada exitosamente')
    }
    await loadAppointments()
    closeModal()
  } catch (error) {
    // Error handled by interceptor
  }
}

const confirmDelete = async () => {
  try {
    await adminService.cancelAppointment(appointmentToDelete.value.id1)
    toast.success('Cita cancelada exitosamente')
    await loadAppointments()
    cancelDelete()
  } catch (error) {
    // Error handled by interceptor
  }
}

const cancelDelete = () => {
  isDeleteModalOpen.value = false
  appointmentToDelete.value = null
}

const closeModal = () => {
  isModalOpen.value = false
  selectedAppointment.value = null
  formMode.value = 'create'
}

const loadAppointments = async () => {
  adminStore.setLoading(true)
  try {
    const appointments = await adminService.getAllAppointments()
    adminStore.setAppointments(appointments)
  } catch (error) {
    // Error handled by interceptor
  } finally {
    adminStore.setLoading(false)
  }
}

onMounted(() => {
  loadAppointments()
})
</script>

