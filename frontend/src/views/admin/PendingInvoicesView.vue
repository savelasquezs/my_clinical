<template>
  <div>
    <PageHeader title="Facturas por Generar">
      <template #actions>
        <router-link to="/admin/invoices" class="btn btn-secondary">
          Ver Facturas Pagas
        </router-link>
      </template>
    </PageHeader>

    <div class="card">
      <div v-if="loading" class="flex justify-center p-8">
        <LoadingSpinner />
      </div>
      <div v-else-if="pendingInvoices.length === 0" class="p-8">
        <EmptyState 
          message="No hay facturas por generar"
          description="Todos los registros médicos con órdenes ya han sido facturados"
        />
      </div>
      <div v-else>
        <CommonTable
          :columns="columns"
          :data="pendingInvoices"
          :loading="false"
          :show-edit="false"
          :show-delete="false"
          :has-actions="true"
        >
          <template #actions="{ row }">
            <button 
              @click="openInvoiceModal(row)" 
              class="btn btn-primary btn-sm"
            >
              Generar Factura
            </button>
          </template>
        </CommonTable>
      </div>
    </div>

    <CommonModal
      v-if="selectedMedicalRecord"
      :is-open="isInvoiceModalOpen"
      title="Generar Factura"
      size="lg"
      @close="closeInvoiceModal"
    >
      <InvoiceForm
        v-if="selectedMedicalRecord"
        :medical-record="selectedMedicalRecord"
        @submit="handleCreateInvoice"
        @cancel="closeInvoiceModal"
      />
    </CommonModal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useToast } from '@/composables/useToast'
import { useDate } from '@/composables/useDate'
import { useMoney } from '@/composables/useMoney'
import { adminService } from '@/services/adminService'
import { doctorService } from '@/services/doctorService'
import { getPendingInvoices } from '@/utils/invoiceUtils'
import { calculateBilling } from '@/utils/billingCalculator'
import PageHeader from '@/components/shared/PageHeader.vue'
import CommonTable from '@/components/shared/CommonTable.vue'
import CommonModal from '@/components/shared/CommonModal.vue'
import InvoiceForm from '@/components/forms/InvoiceForm.vue'
import LoadingSpinner from '@/components/shared/LoadingSpinner.vue'
import EmptyState from '@/components/shared/EmptyState.vue'

const toast = useToast()
const { formatDate } = useDate()
const { formatMoney } = useMoney()

const loading = ref(false)
const medicalRecords = ref([])
const invoices = ref([])
const orders = ref([]) // Cache de órdenes cargadas
const patients = ref([]) // Cache de pacientes cargados
const isInvoiceModalOpen = ref(false)
const selectedMedicalRecord = ref(null)

// Filtrar registros médicos facturables
const pendingInvoices = computed(() => {
  // Enriquecer registros médicos con información de órdenes completas
  const enrichedRecords = medicalRecords.value.map(mr => {
    if (mr.orderNumber) {
      // Buscar la orden completa en el cache
      const order = orders.value.find(o => o.orderNumber === mr.orderNumber)
      if (order) {
        // Buscar el paciente completo
        const patient = patients.value.find(p => p.dni === mr.patientDni)
        return {
          ...mr,
          order: order,
          patient: patient
        }
      }
    }
    // Buscar el paciente completo aunque no tenga orden
    const patient = patients.value.find(p => p.dni === mr.patientDni)
    return {
      ...mr,
      patient: patient
    }
  })

  // Filtrar usando invoiceUtils
  const filtered = getPendingInvoices(enrichedRecords, invoices.value)
  
  // Enriquecer con cálculos estimados
  return filtered.map(mr => {
    if (!mr.order || !mr.order.items) {
      return {
        ...mr,
        estimatedTotal: 0,
        estimatedCopayment: 0,
        estimatedInsurance: 0
      }
    }

    // Calcular total de la orden
    const totalAmount = mr.order.items.reduce((sum, item) => sum + (item.cost || 0), 0)
    
    // Calcular copago estimado si tenemos el paciente
    let estimatedCopayment = 0
    let estimatedInsurance = 0
    
    if (mr.patient) {
      try {
        const billing = calculateBilling(
          mr.patient,
          totalAmount,
          0 // Por ahora no tenemos el acumulado anual
        )
        estimatedCopayment = billing.copaymentAmount
        estimatedInsurance = billing.insuranceAmount
      } catch (error) {
        console.error('Error calculating billing:', error)
      }
    }

    return {
      ...mr,
      estimatedTotal: totalAmount,
      estimatedCopayment: estimatedCopayment,
      estimatedInsurance: estimatedInsurance
    }
  })
})

const columns = [
  { 
    key: 'patientName', 
    label: 'Paciente',
    formatter: (value, row) => `${value} (${row.patientDni})`
  },
  { 
    key: 'date', 
    label: 'Fecha Atención',
    formatter: (value) => formatDate(value)
  },
  { 
    key: 'doctorName', 
    label: 'Médico'
  },
  { 
    key: 'orderNumber', 
    label: 'Orden',
    formatter: (value) => value ? `#${value}` : 'Sin orden'
  },
  { 
    key: 'estimatedTotal', 
    label: 'Total Estimado',
    formatter: (value) => formatMoney(value || 0)
  },
  { 
    key: 'estimatedCopayment', 
    label: 'Copago Estimado',
    formatter: (value) => formatMoney(value || 0)
  },
  { 
    key: 'estimatedInsurance', 
    label: 'Seguro Estimado',
    formatter: (value) => formatMoney(value || 0)
  }
]

const loadData = async () => {
  loading.value = true
  try {
    // Cargar registros médicos
    const records = await doctorService.getAllMedicalRecords()
    medicalRecords.value = records

    // Cargar facturas
    const invoicesData = await adminService.getAllInvoices()
    invoices.value = invoicesData || []

    // Cargar pacientes únicos de los registros médicos
    const patientDnis = new Set(records.map(mr => mr.patientDni).filter(Boolean))
    const patientPromises = Array.from(patientDnis).map(dni =>
      adminService.getPatientByDni(dni)
        .then(patient => ({ dni, patient }))
        .catch(() => ({ dni, patient: null }))
    )
    const patientResults = await Promise.all(patientPromises)
    patients.value = patientResults
      .filter(r => r.patient !== null)
      .map(r => r.patient)

    // Cargar órdenes para los registros médicos que tienen orderNumber
    const orderNumbers = new Set()
    records.forEach(mr => {
      if (mr.orderNumber) {
        orderNumbers.add(mr.orderNumber)
      }
    })

    // Cargar cada orden completa
    const orderPromises = Array.from(orderNumbers).map(orderNumber =>
      doctorService.getOrderByNumber(orderNumber)
        .then(order => ({ orderNumber, order }))
        .catch(() => ({ orderNumber, order: null }))
    )

    const orderResults = await Promise.all(orderPromises)
    orders.value = orderResults
      .filter(r => r.order !== null)
      .map(r => r.order)
  } catch (error) {
    console.error('Error loading data:', error)
    toast.error('Error al cargar los datos: ' + error.message)
  } finally {
    loading.value = false
  }
}

const openInvoiceModal = (medicalRecord) => {
  selectedMedicalRecord.value = medicalRecord
  isInvoiceModalOpen.value = true
}

const closeInvoiceModal = () => {
  setTimeout(() => {
    selectedMedicalRecord.value = null
    isInvoiceModalOpen.value = false
  }, 300)
}

const handleCreateInvoice = async (data) => {
  try {
    await adminService.createInvoice(data)
    toast.success('Factura creada exitosamente')
    closeInvoiceModal()
    // Recargar datos para actualizar la lista
    await loadData()
  } catch (error) {
    // Error handled by interceptor
  }
}

onMounted(() => {
  loadData()
})
</script>

