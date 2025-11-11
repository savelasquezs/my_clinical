<template>
  <div v-if="invoice" class="space-y-6">
    <!-- Información del Paciente (REQ-ADMIN-020) -->
    <div class="bg-gray-50 p-4 rounded-lg border">
      <h3 class="font-semibold text-gray-900 mb-3">Información del Paciente</h3>
      <div class="grid grid-cols-3 gap-4 text-sm">
        <div>
          <span class="text-gray-600">Nombre:</span>
          <span class="ml-2 font-medium">{{ invoice.patient?.name || '-' }}</span>
        </div>
        <div>
          <span class="text-gray-600">Edad:</span>
          <span class="ml-2 font-medium">{{ patientAge !== null && patientAge !== undefined ? patientAge : '-' }} años</span>
        </div>
        <div>
          <span class="text-gray-600">Cédula:</span>
          <span class="ml-2 font-medium">{{ invoice.patient?.dni || '-' }}</span>
        </div>
      </div>
    </div>

    <!-- Información del Médico (REQ-ADMIN-021) -->
    <div class="bg-gray-50 p-4 rounded-lg border">
      <h3 class="font-semibold text-gray-900 mb-3">Médico Tratante</h3>
      <div class="text-sm">
        <span class="text-gray-600">Nombre:</span>
        <span class="ml-2 font-medium">{{ invoice.doctor?.name || '-' }}</span>
        <span class="ml-4 text-gray-600">Cédula:</span>
        <span class="ml-2 font-medium">{{ invoice.doctor?.dni || '-' }}</span>
      </div>
    </div>

    <!-- Información del Seguro (REQ-ADMIN-022, REQ-ADMIN-023, REQ-ADMIN-024, REQ-ADMIN-025) -->
    <div v-if="invoice.insurance" class="bg-blue-50 p-4 rounded-lg border">
      <h3 class="font-semibold text-gray-900 mb-3">Información del Seguro</h3>
      <div class="grid grid-cols-2 gap-4 text-sm">
        <div>
          <span class="text-gray-600">Compañía:</span>
          <span class="ml-2 font-medium">{{ invoice.insurance.companyName }}</span>
        </div>
        <div>
          <span class="text-gray-600">Número de Póliza:</span>
          <span class="ml-2 font-medium">{{ invoice.insurance.policyNumber }}</span>
        </div>
        <div>
          <span class="text-gray-600">Estado:</span>
          <span class="ml-2 font-medium" :class="isInsuranceActuallyActive ? 'text-emerald-600' : 'text-red-600'">
            {{ isInsuranceActuallyActive ? 'Activa' : 'Inactiva/Expirada' }}
          </span>
        </div>
        <div>
          <span class="text-gray-600">Días de Vigencia:</span>
          <span class="ml-2 font-medium">{{ invoice.insurance.daysUntilExpiration !== null ? invoice.insurance.daysUntilExpiration : '-' }}</span>
        </div>
        <div class="col-span-2">
          <span class="text-gray-600">Fecha de Finalización:</span>
          <span class="ml-2 font-medium">{{ formatDate(invoice.insurance.expirationDate) }}</span>
        </div>
      </div>
    </div>
    <div v-else class="bg-yellow-50 p-4 rounded-lg border">
      <p class="text-sm text-yellow-800">⚠️ El paciente no tiene seguro médico registrado</p>
    </div>

    <!-- Información de Órdenes (REQ-ADMIN-026) -->
    <div class="bg-gray-50 p-4 rounded-lg border">
      <h3 class="font-semibold text-gray-900 mb-3">Órdenes Generadas</h3>
      <div v-if="!invoice.orders || invoice.orders.length === 0" class="text-sm text-gray-500">
        No hay órdenes asociadas a esta factura
      </div>
      <div v-else class="space-y-4">
        <div 
          v-for="order in invoice.orders" 
          :key="order.orderNumber"
          class="bg-white p-4 rounded border"
        >
          <div class="flex justify-between items-center mb-3">
            <h4 class="font-semibold">Orden #{{ order.orderNumber }}</h4>
            <span class="text-sm text-gray-600">{{ formatDate(order.creationDate) }}</span>
          </div>
          
          <!-- Items de la Orden (REQ-ADMIN-027, REQ-ADMIN-028, REQ-ADMIN-029) -->
          <div v-if="!order.items || order.items.length === 0" class="text-sm text-gray-500">
            No hay items en esta orden
          </div>
          <div v-else class="space-y-2">
            <div 
              v-for="item in order.items" 
              :key="`${item.orderNumber}-${item.itemNumber}`"
              class="bg-gray-50 p-3 rounded border-l-4"
              :class="{
                'border-blue-500': item.itemType === 'Medication',
                'border-purple-500': item.itemType === 'Procedure',
                'border-orange-500': item.itemType === 'DiagnosticAid'
              }"
            >
              <div class="flex justify-between items-start mb-2">
                <div class="flex-1">
                  <div class="flex items-center gap-2 mb-1">
                    <span class="px-2 py-1 bg-emerald-100 text-emerald-800 rounded text-xs font-medium">
                      {{ getItemTypeLabel(item.itemType) }}
                    </span>
                    <span class="text-sm text-gray-600">Item #{{ item.itemNumber }}</span>
                  </div>
                  <p class="font-medium text-gray-900">{{ getItemName(item) }}</p>
                </div>
                <span class="text-emerald-600 font-semibold">{{ formatMoney(item.cost) }}</span>
              </div>
              
              <!-- Detalles específicos por tipo -->
              <div v-if="item.itemType === 'Medication'" class="text-sm text-gray-600 space-y-1">
                <p v-if="item.dose">Dosis: {{ item.dose }}</p>
                <p v-if="item.treatmentDuration">Duración del tratamiento: {{ item.treatmentDuration }} días</p>
              </div>
              
              <div v-else-if="item.itemType === 'Procedure'" class="text-sm text-gray-600 space-y-1">
                <p v-if="item.frequency">Frecuencia: {{ item.frequency }}</p>
                <p v-if="item.requiresSpecialist" class="text-orange-600">
                  Requiere especialista
                  <span v-if="item.specialistTypeId"> (Tipo: {{ item.specialistTypeId }})</span>
                </p>
              </div>
              
              <div v-else-if="item.itemType === 'DiagnosticAid'" class="text-sm text-gray-600 space-y-1">
                <p v-if="item.quantity">Cantidad: {{ item.quantity }}</p>
                <p v-if="item.requiresSpecialist" class="text-orange-600">
                  Requiere especialista
                  <span v-if="item.specialistTypeId"> (Tipo: {{ item.specialistTypeId }})</span>
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Montos y Reglas de Copago (REQ-ADMIN-030, REQ-ADMIN-031, REQ-ADMIN-032) -->
    <div class="bg-emerald-50 p-4 rounded-lg border">
      <h3 class="font-semibold text-gray-900 mb-3">Resumen de Facturación</h3>
      <div class="space-y-2 text-sm">
        <div class="flex justify-between">
          <span class="text-gray-600">Total de Servicios:</span>
          <span class="font-semibold text-lg">{{ formatMoney(invoice.amounts?.totalAmount || 0) }}</span>
        </div>
        <div class="flex justify-between">
          <span class="text-gray-600">Copago del Paciente:</span>
          <span class="font-semibold text-emerald-600">{{ formatMoney(invoice.amounts?.copaymentAmount || 0) }}</span>
        </div>
        <div class="flex justify-between">
          <span class="text-gray-600">Monto a Aseguradora:</span>
          <span class="font-semibold text-blue-600">{{ formatMoney(invoice.amounts?.insuranceAmount || 0) }}</span>
        </div>
        <div class="pt-2 border-t mt-2">
          <div class="flex justify-between">
            <span class="text-gray-600">Copago Acumulado del Año:</span>
            <span class="font-medium">{{ formatMoney(invoice.amounts?.annualCopaymentAccumulated || 0) }}</span>
          </div>
        </div>
      </div>
      
      <!-- Información sobre reglas aplicadas -->
      <div class="mt-4 pt-4 border-t">
        <p class="text-xs text-gray-600">
          <span v-if="invoice.insurance && isInsuranceActuallyActive">
            ✓ Póliza activa: Copago de $50,000 aplicado (REQ-ADMIN-030)
          </span>
          <span v-else-if="invoice.amounts?.annualCopaymentAccumulated >= 1000000" class="text-blue-600">
            ✓ Copago acumulado supera $1,000,000: Aseguradora asume totalidad (REQ-ADMIN-031)
          </span>
          <span v-else class="text-red-600">
            ⚠️ Póliza inactiva o sin seguro: Paciente paga totalidad (REQ-ADMIN-032)
          </span>
        </p>
      </div>
    </div>

    <!-- Información de la Factura -->
    <div class="bg-gray-50 p-4 rounded-lg border">
      <div class="grid grid-cols-2 gap-4 text-sm">
        <div>
          <span class="text-gray-600">Número de Factura:</span>
          <span class="ml-2 font-medium">#{{ invoice.invoiceNumber }}</span>
        </div>
        <div>
          <span class="text-gray-600">Fecha de Factura:</span>
          <span class="ml-2 font-medium">{{ formatDate(invoice.invoiceDate) }}</span>
        </div>
      </div>
    </div>
  </div>
  <div v-else class="p-8 text-center text-gray-500">
    No hay información de factura disponible
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useDate } from '@/composables/useDate'
import { useMoney } from '@/composables/useMoney'

const props = defineProps({
  invoice: {
    type: Object,
    default: null
  }
})

const { formatDate } = useDate()
const { formatMoney } = useMoney()

// Calcular edad del paciente si no viene del backend
const patientAge = computed(() => {
  // Verificar si la edad viene del backend (incluyendo 0 como valor válido)
  if (props.invoice?.patient?.age !== null && props.invoice?.patient?.age !== undefined && typeof props.invoice.patient.age === 'number') {
    return props.invoice.patient.age
  }
  
  // Si no viene la edad, intentar calcularla desde la fecha de nacimiento
  // Nota: El backend debería enviar la edad, pero por si acaso calculamos aquí
  if (props.invoice?.patient?.birthdate) {
    const birthdate = new Date(props.invoice.patient.birthdate)
    const today = new Date()
    let age = today.getFullYear() - birthdate.getFullYear()
    const monthDiff = today.getMonth() - birthdate.getMonth()
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthdate.getDate())) {
      age--
    }
    return age >= 0 ? age : null
  }
  
  return null
})

// Determinar si el seguro está realmente activo
// Nota: El backend ahora actualiza automáticamente isActive cuando la póliza expira,
// por lo que podemos confiar en el valor de isActive que viene del servidor
const isInsuranceActuallyActive = computed(() => {
  return props.invoice?.insurance?.isActive ?? false
})

const getItemTypeLabel = (itemType) => {
  const labels = {
    Medication: 'Medicamento',
    Procedure: 'Procedimiento',
    DiagnosticAid: 'Ayuda Diagnóstica'
  }
  return labels[itemType] || itemType
}

const getItemName = (item) => {
  if (item.itemType === 'Medication') {
    return item.medicationName || 'Medicamento'
  } else if (item.itemType === 'Procedure') {
    return item.procedureName || 'Procedimiento'
  } else if (item.itemType === 'DiagnosticAid') {
    return item.diagnosticAidName || 'Ayuda Diagnóstica'
  }
  return 'Item'
}
</script>

