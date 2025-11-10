<template>
  <div>
    <PageHeader title="Citas">
      <template #actions>
        <button @click="openCreateModal" class="btn btn-primary">
          Crear Cita
        </button>
      </template>
    </PageHeader>

    <div class="card">
      <CommonTable
        :columns="columns"
        :data="adminStore.appointments"
        :loading="adminStore.loading"
      />
    </div>

    <CommonModal
      :is-open="isModalOpen"
      title="Crear Cita"
      @close="closeModal"
    >
      <AppointmentForm
        @submit="handleSubmit"
        @cancel="closeModal"
      />
    </CommonModal>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useAdminStore } from '@/stores/admin'
import { adminService } from '@/services/adminService'
import { useToast } from '@/composables/useToast'
import { useDate } from '@/composables/useDate'
import PageHeader from '@/components/shared/PageHeader.vue'
import CommonTable from '@/components/shared/CommonTable.vue'
import CommonModal from '@/components/shared/CommonModal.vue'
import AppointmentForm from '@/components/forms/AppointmentForm.vue'

const adminStore = useAdminStore()
const toast = useToast()
const { formatDateTime } = useDate()

const isModalOpen = ref(false)

const columns = [
  { key: 'id1', label: 'ID' },
  { key: 'patientDni', label: 'DNI Paciente' },
  { 
    key: 'date1', 
    label: 'Fecha',
    formatter: (value) => formatDateTime(value)
  }
]

const openCreateModal = () => {
  isModalOpen.value = true
}

const handleSubmit = async (data) => {
  try {
    await adminService.createAppointment(data)
    toast.success('Cita creada exitosamente')
    await loadAppointments()
    closeModal()
  } catch (error) {
    // Error handled by interceptor
  }
}

const closeModal = () => {
  isModalOpen.value = false
}

const loadAppointments = async () => {
  adminStore.setLoading(true)
  try {
    // Load appointments logic here
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

