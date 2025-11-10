<template>
  <div>
    <PageHeader title="Facturas">
      <template #actions>
        <button @click="openCreateModal" class="btn btn-primary">
          Crear Factura
        </button>
      </template>
    </PageHeader>

    <div class="card">
      <CommonTable
        :columns="columns"
        :data="adminStore.invoices"
        :loading="adminStore.loading"
      />
    </div>

    <CommonModal
      :is-open="isModalOpen"
      title="Crear Factura"
      size="lg"
      @close="closeModal"
    >
      <InvoiceForm
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
import { useMoney } from '@/composables/useMoney'
import PageHeader from '@/components/shared/PageHeader.vue'
import CommonTable from '@/components/shared/CommonTable.vue'
import CommonModal from '@/components/shared/CommonModal.vue'
import InvoiceForm from '@/components/forms/InvoiceForm.vue'

const adminStore = useAdminStore()
const toast = useToast()
const { formatDate } = useDate()
const { formatMoney } = useMoney()

const isModalOpen = ref(false)

const columns = [
  { key: 'invoiceNumber', label: 'Número' },
  { key: 'patientDni', label: 'DNI Paciente' },
  { 
    key: 'invoiceDate', 
    label: 'Fecha',
    formatter: (value) => formatDate(value)
  },
  { 
    key: 'totalAmount', 
    label: 'Total',
    formatter: (value) => formatMoney(value)
  }
]

const openCreateModal = () => {
  isModalOpen.value = true
}

const handleSubmit = async (data) => {
  try {
    await adminService.createInvoice(data)
    toast.success('Factura creada exitosamente')
    await loadInvoices()
    closeModal()
  } catch (error) {
    // Error handled by interceptor
  }
}

const closeModal = () => {
  isModalOpen.value = false
}

const loadInvoices = async () => {
  adminStore.setLoading(true)
  try {
    // Load invoices logic here
  } catch (error) {
    // Error handled by interceptor
  } finally {
    adminStore.setLoading(false)
  }
}

onMounted(() => {
  loadInvoices()
})
</script>

