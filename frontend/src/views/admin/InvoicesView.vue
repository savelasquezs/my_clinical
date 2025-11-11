<template>
  <div>
    <PageHeader title="Facturas Pagas">
      <template #actions>
        <router-link to="/admin/invoices/pending" class="btn btn-secondary">
          Ver Facturas por Generar
        </router-link>
      </template>
    </PageHeader>

    <div class="card">
      <div v-if="adminStore.loading" class="flex justify-center p-8">
        <LoadingSpinner />
      </div>
      <div v-else-if="adminStore.invoices.length === 0" class="p-8">
        <EmptyState 
          message="No hay facturas registradas"
          description="Las facturas aparecerán aquí una vez sean creadas"
        />
      </div>
      <div v-else>
        <CommonTable
          :columns="columns"
          :data="adminStore.invoices"
          :loading="false"
          :clickable="true"
          @row-click="viewInvoiceDetail"
        />
      </div>
    </div>

    <CommonModal
      v-if="selectedInvoice"
      :is-open="isDetailModalOpen"
      title="Detalle de Factura"
      size="xl"
      @close="closeDetailModal"
    >
      <InvoiceDetail
        v-if="selectedInvoice"
        :invoice="selectedInvoice"
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
import InvoiceDetail from '@/components/shared/InvoiceDetail.vue'
import LoadingSpinner from '@/components/shared/LoadingSpinner.vue'
import EmptyState from '@/components/shared/EmptyState.vue'

const adminStore = useAdminStore()
const toast = useToast()
const { formatDate } = useDate()
const { formatMoney } = useMoney()

const isDetailModalOpen = ref(false)
const selectedInvoice = ref(null)

const columns = [
  { key: 'invoiceNumber', label: 'Número' },
  { 
    key: 'patient', 
    label: 'Paciente',
    formatter: (value) => value ? `${value.name} (${value.dni})` : '-'
  },
  { 
    key: 'doctor', 
    label: 'Médico',
    formatter: (value) => value ? value.name : '-'
  },
  { 
    key: 'invoiceDate', 
    label: 'Fecha',
    formatter: (value) => formatDate(value)
  },
  { 
    key: 'amounts', 
    label: 'Total',
    formatter: (value) => value ? formatMoney(value.totalAmount) : '-'
  },
  { 
    key: 'amounts', 
    label: 'Copago',
    formatter: (value) => value ? formatMoney(value.copaymentAmount) : '-'
  }
]

const viewInvoiceDetail = (invoice) => {
  selectedInvoice.value = invoice
  isDetailModalOpen.value = true
}

const closeDetailModal = () => {
  setTimeout(() => {
    selectedInvoice.value = null
    isDetailModalOpen.value = false
  }, 300)
}

const loadInvoices = async () => {
  adminStore.setLoading(true)
  try {
    const invoices = await adminService.getAllInvoices()
    adminStore.setInvoices(invoices)
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

