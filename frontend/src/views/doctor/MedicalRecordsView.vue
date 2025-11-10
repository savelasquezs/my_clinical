<template>
  <div>
    <PageHeader title="Registros Médicos">
      <template #actions>
        <button @click="openCreateModal" class="btn btn-primary">
          Crear Registro Médico
        </button>
      </template>
    </PageHeader>

    <div class="card">
      <CommonTable
        :columns="columns"
        :data="doctorStore.medicalRecords"
        :loading="doctorStore.loading"
      />
    </div>

    <CommonModal
      :is-open="isModalOpen"
      title="Crear Registro Médico"
      size="xl"
      @close="closeModal"
    >
      <MedicalRecordForm
        @submit="handleSubmit"
        @cancel="closeModal"
      />
    </CommonModal>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useDoctorStore } from '@/stores/doctor'
import { doctorService } from '@/services/doctorService'
import { useToast } from '@/composables/useToast'
import { useDate } from '@/composables/useDate'
import PageHeader from '@/components/shared/PageHeader.vue'
import CommonTable from '@/components/shared/CommonTable.vue'
import CommonModal from '@/components/shared/CommonModal.vue'
import MedicalRecordForm from '@/components/forms/MedicalRecordForm.vue'

const doctorStore = useDoctorStore()
const toast = useToast()
const { formatDate } = useDate()

const isModalOpen = ref(false)

const columns = [
  { key: 'id', label: 'ID' },
  { key: 'patientDni', label: 'DNI Paciente' },
  { 
    key: 'date', 
    label: 'Fecha',
    formatter: (value) => formatDate(value)
  },
  { key: 'diagnosis', label: 'Diagnóstico' }
]

const openCreateModal = () => {
  isModalOpen.value = true
}

const handleSubmit = async (data) => {
  try {
    await doctorService.createMedicalRecord(data)
    toast.success('Registro médico creado exitosamente')
    closeModal()
  } catch (error) {
    // Error handled by interceptor
  }
}

const closeModal = () => {
  isModalOpen.value = false
}

onMounted(() => {
  // Load medical records if needed
})
</script>

