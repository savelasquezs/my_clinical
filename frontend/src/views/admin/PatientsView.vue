<template>
  <div>
    <PageHeader title="Pacientes">
      <template #actions>
        <button @click="openCreateModal" class="btn btn-primary">
          Crear Paciente
        </button>
      </template>
    </PageHeader>

    <div class="card">
      <CommonTable
        :columns="columns"
        :data="adminStore.patients"
        :loading="adminStore.loading"
        @edit="handleEdit"
      />
    </div>

    <CommonModal
      :is-open="isModalOpen"
      :title="modalTitle"
      size="xl"
      @close="closeModal"
    >
      <PatientForm
        :mode="formMode"
        :initial-data="selectedPatient"
        @submit="handleSubmit"
        @cancel="closeModal"
      />
    </CommonModal>
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
import PatientForm from '@/components/forms/PatientForm.vue'

const adminStore = useAdminStore()
const toast = useToast()
const { formatDate } = useDate()

const isModalOpen = ref(false)
const formMode = ref('create')
const selectedPatient = ref(null)

const modalTitle = computed(() => 
  formMode.value === 'create' ? 'Crear Paciente' : 'Editar Paciente'
)

const columns = [
  { key: 'dni', label: 'DNI' },
  { key: 'fullname', label: 'Nombre Completo' },
  { key: 'email', label: 'Email' },
  { key: 'phonenumber', label: 'Teléfono' },
  { 
    key: 'birthdate', 
    label: 'Fecha de Nacimiento',
    formatter: (value) => formatDate(value)
  }
]

const openCreateModal = () => {
  formMode.value = 'create'
  selectedPatient.value = null
  isModalOpen.value = true
}

const handleEdit = (patient) => {
  formMode.value = 'edit'
  selectedPatient.value = patient
  isModalOpen.value = true
}

const handleSubmit = async (data) => {
  try {
    if (formMode.value === 'create') {
      await adminService.createPatient(data)
      toast.success('Paciente creado exitosamente')
      await loadPatients()
    } else {
      await adminService.updatePatient(selectedPatient.value.dni, data)
      toast.success('Paciente actualizado exitosamente')
      await loadPatients()
    }
    closeModal()
  } catch (error) {
    // Error handled by interceptor
  }
}

const closeModal = () => {
  isModalOpen.value = false
  selectedPatient.value = null
}

const loadPatients = async () => {
  adminStore.setLoading(true)
  try {
    const patients = await adminService.getAllPatients()
    adminStore.setPatients(patients)
  } catch (error) {
    // Error handled by interceptor
  } finally {
    adminStore.setLoading(false)
  }
}

onMounted(() => {
  loadPatients()
})
</script>

