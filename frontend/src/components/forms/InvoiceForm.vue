<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <!-- Información del Registro Médico (si viene desde medicalRecord) -->
    <div v-if="medicalRecord" class="bg-gray-50 p-4 rounded-lg border mb-4">
      <h3 class="font-semibold text-gray-900 mb-2">Información del Registro Médico</h3>
      <div class="grid grid-cols-2 gap-2 text-sm">
        <div>
          <span class="text-gray-600">Paciente:</span>
          <span class="ml-2 font-medium">{{ medicalRecord.patientName }} ({{ medicalRecord.patientDni }})</span>
        </div>
        <div>
          <span class="text-gray-600">Médico:</span>
          <span class="ml-2 font-medium">{{ medicalRecord.doctorName }} ({{ medicalRecord.doctorDni }})</span>
        </div>
        <div>
          <span class="text-gray-600">Fecha Atención:</span>
          <span class="ml-2 font-medium">{{ formatDate(medicalRecord.date) }}</span>
        </div>
        <div>
          <span class="text-gray-600">Diagnóstico:</span>
          <span class="ml-2 font-medium">{{ medicalRecord.diagnosis }}</span>
        </div>
        <div v-if="medicalRecord.order" class="col-span-2">
          <span class="text-gray-600">Orden:</span>
          <span class="ml-2 font-medium">#{{ medicalRecord.order.orderNumber }}</span>
          <span class="ml-4 text-gray-600">
            ({{ medicalRecord.order.items?.length || 0 }} item(s))
          </span>
        </div>
        <div v-else class="col-span-2 text-red-600">
          ⚠️ Este registro médico no tiene orden asociada
        </div>
      </div>
    </div>

    <!-- Información de la Orden (si viene desde medicalRecord) -->
    <div v-if="medicalRecord?.order && medicalRecord.order.items" class="bg-blue-50 p-4 rounded-lg border mb-4">
      <h3 class="font-semibold text-gray-900 mb-2">Detalle de la Orden</h3>
      <div class="space-y-2">
        <div 
          v-for="item in medicalRecord.order.items" 
          :key="`${item.orderNumber}-${item.itemNumber}`"
          class="text-sm bg-white p-2 rounded border"
        >
          <div class="flex justify-between">
            <span class="font-medium">{{ getItemName(item) }}</span>
            <span class="text-emerald-600 font-semibold">{{ formatMoney(item.cost) }}</span>
          </div>
          <div v-if="item.itemType === 'Medication'" class="text-gray-600 mt-1">
            Dosis: {{ item.dose }}, Duración: {{ item.treatmentDuration }} días
          </div>
          <div v-else-if="item.itemType === 'Procedure'" class="text-gray-600 mt-1">
            Frecuencia: {{ item.frequency }}
          </div>
          <div v-else-if="item.itemType === 'DiagnosticAid'" class="text-gray-600 mt-1">
            Cantidad: {{ item.quantity }}
          </div>
        </div>
        <div class="pt-2 border-t mt-2">
          <div class="flex justify-between font-semibold">
            <span>Total de la Orden:</span>
            <span class="text-emerald-600">{{ formatMoney(orderTotal) }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Cálculo Previo de Copago (si viene desde medicalRecord y tiene paciente) -->
    <div v-if="medicalRecord && patient && billingCalculation" class="p-4 rounded-lg border mb-4" 
         :class="insuranceStatusClass">
      <h3 class="font-semibold text-gray-900 mb-2">Cálculo Previo de Facturación</h3>
      
      <!-- Advertencias sobre el estado del seguro -->
      <div v-if="!patient.insurance || !patient.insurance.isActive || isInsuranceExpired" 
           class="mb-3 p-3 rounded bg-red-50 border border-red-200">
        <div class="flex items-start">
          <svg class="w-5 h-5 text-red-600 mt-0.5 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
          </svg>
          <div class="text-sm text-red-800">
            <p class="font-semibold mb-1">Advertencia sobre el Seguro:</p>
            <ul class="list-disc list-inside space-y-1">
              <li v-if="!patient.insurance">El paciente no posee seguro médico registrado.</li>
              <li v-else-if="!patient.insurance.isActive">La póliza está marcada como inactiva.</li>
              <li v-else-if="isInsuranceExpired">La póliza ha expirado ({{ formatDate(patient.insurance.expirationDate) }}).</li>
            </ul>
            <p class="mt-2 font-medium">Según REQ-ADMIN-032: El paciente pagará el total de los servicios prestados.</p>
          </div>
        </div>
      </div>
      
      <div v-else-if="billingCalculation.annualCopaymentAccumulated >= 1000000" 
           class="mb-3 p-3 rounded bg-blue-50 border border-blue-200">
        <div class="flex items-start">
          <svg class="w-5 h-5 text-blue-600 mt-0.5 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd" />
          </svg>
          <div class="text-sm text-blue-800">
            <p class="font-semibold">Información sobre Copago Acumulado:</p>
            <p class="mt-1">El paciente ha alcanzado el tope anual de copago ($1,000,000).</p>
            <p class="mt-1 font-medium">Según REQ-ADMIN-031: La aseguradora asumirá el total de esta factura.</p>
          </div>
        </div>
      </div>
      
      <div class="space-y-1 text-sm">
        <div class="flex justify-between">
          <span class="text-gray-600">Total:</span>
          <span class="font-medium">{{ formatMoney(billingCalculation.totalAmount) }}</span>
        </div>
        <div class="flex justify-between">
          <span class="text-gray-600">Copago:</span>
          <span class="font-medium">{{ formatMoney(billingCalculation.copaymentAmount) }}</span>
        </div>
        <div class="flex justify-between">
          <span class="text-gray-600">Aseguradora:</span>
          <span class="font-medium">{{ formatMoney(billingCalculation.insuranceAmount) }}</span>
        </div>
        <div v-if="patient.insurance" class="pt-2 border-t mt-2">
          <div class="text-xs text-gray-600">
            <div>Compañía: {{ patient.insurance.companyName }}</div>
            <div>Póliza: {{ patient.insurance.policyNumber }}</div>
            <div>Estado: 
              <span :class="patient.insurance.isActive && !isInsuranceExpired ? 'text-emerald-600 font-semibold' : 'text-red-600 font-semibold'">
                {{ patient.insurance.isActive && !isInsuranceExpired ? 'Activa' : 'Inactiva/Expirada' }}
              </span>
            </div>
            <div v-if="patient.insurance.expirationDate">
              Fecha de Expiración: {{ formatDate(patient.insurance.expirationDate) }}
            </div>
            <div v-if="billingCalculation.annualCopaymentAccumulated > 0" class="mt-1 text-gray-700">
              Copago Acumulado del Año: {{ formatMoney(billingCalculation.annualCopaymentAccumulated) }}
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-2 gap-4">
      <div>
        <label class="label">Número de Factura *</label>
        <input v-model.number="formData.invoiceNumber" type="number" class="input" required />
      </div>
      <div v-if="!medicalRecord">
        <PatientSelector 
          v-model="formData.patientDni"
          label="Paciente *"
          :required="true"
        />
      </div>
      <div v-if="!medicalRecord">
        <label class="label">DNI del Doctor *</label>
        <input v-model="formData.doctorDni" type="text" class="input" required />
      </div>
      <div>
        <label class="label">Fecha de Factura *</label>
        <input v-model="formData.invoiceDate" type="datetime-local" class="input" required />
      </div>
      <div v-if="!medicalRecord" class="col-span-2">
        <label class="label">Números de Orden (separados por comas)</label>
        <input v-model="formData.orderNumbers" type="text" class="input" placeholder="1, 2, 3" />
      </div>
    </div>

    <div class="flex justify-end gap-3 pt-4 border-t">
      <button type="button" @click="$emit('cancel')" class="btn btn-secondary">
        Cancelar
      </button>
      <button 
        type="submit" 
        class="btn btn-primary"
        :disabled="!canSubmit"
      >
        Crear Factura
      </button>
    </div>
  </form>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useToast } from '@/composables/useToast'
import { useDate } from '@/composables/useDate'
import { useMoney } from '@/composables/useMoney'
import { calculateBilling } from '@/utils/billingCalculator'
import { adminService } from '@/services/adminService'
import PatientSelector from '@/components/shared/PatientSelector.vue'

const props = defineProps({
  medicalRecord: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['submit', 'cancel'])

const toast = useToast()
const { formatDate } = useDate()
const { formatMoney } = useMoney()

const formData = ref({
  invoiceNumber: null,
  patientDni: '',
  doctorDni: '',
  invoiceDate: '',
  orderNumbers: []
})

const patient = ref(null)
const billingCalculation = ref(null)

// Calcular total de la orden
const orderTotal = computed(() => {
  if (!props.medicalRecord?.order?.items) return 0
  return props.medicalRecord.order.items.reduce((sum, item) => sum + (item.cost || 0), 0)
})

// Verificar si el seguro está expirado
const isInsuranceExpired = computed(() => {
  if (!patient.value?.insurance?.expirationDate) return false
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  const todayDateOnly = new Date(today.getFullYear(), today.getMonth(), today.getDate())
  const expirationDate = new Date(patient.value.insurance.expirationDate)
  const expirationDateOnly = new Date(expirationDate.getFullYear(), expirationDate.getMonth(), expirationDate.getDate())
  return expirationDateOnly < todayDateOnly
})

// Clase CSS según el estado del seguro
const insuranceStatusClass = computed(() => {
  if (!patient.value?.insurance || !patient.value.insurance.isActive || isInsuranceExpired.value) {
    return 'bg-red-50 border-red-200'
  }
  if (billingCalculation.value?.annualCopaymentAccumulated >= 1000000) {
    return 'bg-blue-50 border-blue-200'
  }
  return 'bg-emerald-50 border-emerald-200'
})

// Validar que se pueda enviar
const canSubmit = computed(() => {
  if (props.medicalRecord) {
    // Si viene desde medicalRecord, debe tener orden
    if (!props.medicalRecord.order || !props.medicalRecord.order.orderNumber) {
      return false
    }
    return formData.value.invoiceNumber && formData.value.invoiceDate
  } else {
    // Modo manual: validar campos básicos
    return formData.value.invoiceNumber && 
           formData.value.patientDni && 
           formData.value.doctorDni && 
           formData.value.invoiceDate &&
           formData.value.orderNumbers.length > 0
  }
})

// Cargar paciente si viene desde medicalRecord
const loadPatient = async () => {
  if (props.medicalRecord && props.medicalRecord.patientDni) {
    try {
      const patientData = await adminService.getPatientByDni(props.medicalRecord.patientDni)
      patient.value = patientData
      
      // Calcular facturación
      if (patientData && orderTotal.value > 0) {
        const total = orderTotal.value
        const calculation = calculateBilling(
          patientData,
          total,
          0 // Por ahora no tenemos el acumulado anual
        )
        billingCalculation.value = {
          totalAmount: total,
          ...calculation
        }
      }
    } catch (error) {
      console.error('Error loading patient:', error)
    }
  }
}

// Inicializar formulario desde medicalRecord
watch(() => props.medicalRecord, (newRecord) => {
  if (newRecord) {
    formData.value.patientDni = newRecord.patientDni
    formData.value.doctorDni = newRecord.doctorDni
    if (newRecord.order && newRecord.order.orderNumber) {
      formData.value.orderNumbers = [newRecord.order.orderNumber]
    }
    // Establecer fecha actual como predeterminada
    const now = new Date()
    const year = now.getFullYear()
    const month = String(now.getMonth() + 1).padStart(2, '0')
    const day = String(now.getDate()).padStart(2, '0')
    const hours = String(now.getHours()).padStart(2, '0')
    const minutes = String(now.getMinutes()).padStart(2, '0')
    formData.value.invoiceDate = `${year}-${month}-${day}T${hours}:${minutes}`
    
    loadPatient()
  }
}, { immediate: true })

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

const handleSubmit = () => {
  // Validar que tenga orden si viene desde medicalRecord
  if (props.medicalRecord && (!props.medicalRecord.order || !props.medicalRecord.order.orderNumber)) {
    toast.error('Este registro médico no tiene orden asociada. No se puede generar factura.')
    return
  }

  // Validar que la orden no esté ya facturada (verificación adicional)
  // Esto se hace en el backend también, pero es buena práctica validar aquí

  const data = {
    invoiceNumber: formData.value.invoiceNumber,
    patientDni: formData.value.patientDni,
    doctorDni: formData.value.doctorDni,
    invoiceDate: formData.value.invoiceDate,
    orderNumbers: props.medicalRecord 
      ? [props.medicalRecord.order.orderNumber]
      : formData.value.orderNumbers
  }
  
  emit('submit', data)
}
</script>

