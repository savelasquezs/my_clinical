<template>
  <div>
    <PageHeader title="Órdenes">
      <template #actions>
        <button @click="openCreateModal" class="btn btn-primary">
          Crear Orden
        </button>
      </template>
    </PageHeader>

    <div class="card">
      <CommonTable
        :columns="columns"
        :data="doctorStore.orders"
        :loading="doctorStore.loading"
      />
    </div>

    <CommonModal
      :is-open="isModalOpen"
      title="Crear Orden"
      size="lg"
      @close="closeModal"
    >
      <OrderForm
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
import OrderForm from '@/components/forms/OrderForm.vue'

const doctorStore = useDoctorStore()
const toast = useToast()
const { formatDate } = useDate()

const isModalOpen = ref(false)

const columns = [
  { key: 'orderNumber', label: 'Número' },
  { 
    key: 'creationDate', 
    label: 'Fecha de Creación',
    formatter: (value) => formatDate(value)
  }
]

const openCreateModal = () => {
  isModalOpen.value = true
}

const handleSubmit = async (data) => {
  try {
    await doctorService.createOrder(data)
    toast.success('Orden creada exitosamente')
    closeModal()
  } catch (error) {
    // Error handled by interceptor
  }
}

const closeModal = () => {
  isModalOpen.value = false
}

onMounted(() => {
  // Load orders if needed
})
</script>

