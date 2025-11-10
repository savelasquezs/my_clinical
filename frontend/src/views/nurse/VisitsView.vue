<template>
  <div>
    <PageHeader title="Visitas de Enfermería">
      <template #actions>
        <button @click="openCreateModal" class="btn btn-primary">
          Crear Visita
        </button>
      </template>
    </PageHeader>

    <div class="card">
      <CommonTable
        :columns="columns"
        :data="nurseStore.visits"
        :loading="nurseStore.loading"
      />
    </div>

    <CommonModal
      :is-open="isModalOpen"
      title="Crear Visita de Enfermería"
      size="xl"
      @close="closeModal"
    >
      <NurseVisitForm
        @submit="handleSubmit"
        @cancel="closeModal"
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
import NurseVisitForm from '@/components/forms/NurseVisitForm.vue'

const nurseStore = useNurseStore()
const toast = useToast()
const { formatDate } = useDate()

const isModalOpen = ref(false)

const columns = [
  { key: 'patientDni', label: 'DNI Paciente' },
  { 
    key: 'performedAt', 
    label: 'Fecha',
    formatter: (value) => formatDate(value)
  }
]

const openCreateModal = () => {
  isModalOpen.value = true
}

const handleSubmit = async (data) => {
  try {
    await nurseService.createNurseVisit(data)
    toast.success('Visita creada exitosamente')
    closeModal()
  } catch (error) {
    // Error handled by interceptor
  }
}

const closeModal = () => {
  isModalOpen.value = false
}

onMounted(() => {
  // Load visits if needed
})
</script>

