<template>
  <div v-if="loading" class="text-center py-8">
    <LoadingSpinner />
  </div>
  <div v-else-if="orderDetails" class="space-y-6">
    <div>
      <h3 class="text-lg font-semibold mb-2">Información de la Orden</h3>
      <div class="grid grid-cols-2 gap-4 text-sm">
        <div>
          <span class="font-medium">Número de Orden:</span>
          <span class="ml-2">{{ orderDetails.orderNumber }}</span>
        </div>
        <div>
          <span class="font-medium">Fecha de Creación:</span>
          <span class="ml-2">{{ formatDate(orderDetails.creationDate) }}</span>
        </div>
      </div>
    </div>

    <!-- Medicamentos -->
    <div v-if="medications.length > 0">
      <h3 class="text-lg font-semibold mb-3">Medicamentos</h3>
      <div class="space-y-4">
        <div 
          v-for="med in medications" 
          :key="`med-${med.itemNumber}`"
          class="border rounded-lg p-4"
        >
          <div class="flex justify-between items-start mb-2">
            <div>
              <h4 class="font-medium">{{ med.medicationName }}</h4>
              <p class="text-sm text-gray-600">
                Dosis por día: {{ med.dose }} | Duración: {{ med.treatmentDuration }} días
              </p>
            </div>
            <span 
              class="px-2 py-1 rounded text-xs"
              :class="getProgressClass(med)"
            >
              {{ getProgressText(med) }}
            </span>
          </div>
          <div class="mt-2">
            <div class="w-full bg-gray-200 rounded-full h-2">
              <div 
                class="bg-emerald-600 h-2 rounded-full transition-all"
                :style="{ width: `${getProgressPercentage(med)}%` }"
              ></div>
            </div>
            <p class="text-xs text-gray-600 mt-1">
              {{ getProgressDetails(med) }}
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Procedimientos -->
    <div v-if="procedures.length > 0">
      <h3 class="text-lg font-semibold mb-3">Procedimientos</h3>
      <div class="space-y-3">
        <div 
          v-for="proc in procedures" 
          :key="`proc-${proc.itemNumber}`"
          class="border rounded-lg p-4 flex justify-between items-center"
        >
          <div>
            <h4 class="font-medium">{{ proc.procedureName }}</h4>
            <p class="text-sm text-gray-600">Frecuencia: {{ proc.frequency }}</p>
          </div>
          <div class="flex items-center gap-3">
            <span 
              v-if="proc.procedureName.toLowerCase() === 'visita de enfermeria'"
              class="px-3 py-1 rounded text-sm font-medium bg-emerald-100 text-emerald-800"
            >
              Visita de Enfermería
            </span>
            <span 
              class="px-2 py-1 rounded text-xs"
              :class="proc.isPerformed ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'"
            >
              {{ proc.isPerformed ? 'Realizado' : 'Pendiente' }}
            </span>
            <button
              v-if="proc.procedureName.toLowerCase() === 'visita de enfermeria'"
              @click="handleCreateVisit(proc.itemNumber)"
              class="btn btn-sm btn-primary"
            >
              Crear Visita
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Ayudas Diagnósticas -->
    <div v-if="diagnosticAids.length > 0">
      <h3 class="text-lg font-semibold mb-3">Ayudas Diagnósticas</h3>
      <div class="space-y-3">
        <div 
          v-for="diag in diagnosticAids" 
          :key="`diag-${diag.itemNumber}`"
          class="border rounded-lg p-4"
        >
          <h4 class="font-medium">{{ diag.diagnosticAidName }}</h4>
          <p class="text-sm text-gray-600">Cantidad: {{ diag.quantity }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useNurseStore } from '@/stores/nurse'
import { useDate } from '@/composables/useDate'
import { calculateMedicationProgress, isProcedurePerformed } from '@/utils/nurseUtils'
import LoadingSpinner from '@/components/shared/LoadingSpinner.vue'

const props = defineProps({
  orderNumber: {
    type: Number,
    required: true
  }
})

const emit = defineEmits(['create-visit'])

const nurseStore = useNurseStore()
const { formatDate } = useDate()
const loading = ref(false)
const orderDetails = ref(null)

const medications = computed(() => {
  if (!orderDetails.value?.items) return []
  return orderDetails.value.items
    .filter(item => item.itemType === 'Medication')
    .map(item => {
      // Filtrar administeredMedications que corresponden a este item
      const relevantAdministered = (orderDetails.value.administeredMedications || [])
        .filter(am => am.orderNumber === item.orderNumber && am.itemNumber === item.itemNumber)
      
      const progress = calculateMedicationProgress(
        item,
        relevantAdministered
      )
      return {
        ...item,
        progress
      }
    })
})

const procedures = computed(() => {
  if (!orderDetails.value?.items) return []
  return orderDetails.value.items
    .filter(item => item.itemType === 'Procedure')
    .map(item => {
      // Filtrar performedProcedures que corresponden a este item
      const relevantPerformed = (orderDetails.value.performedProcedures || [])
        .filter(pp => pp.orderNumber === item.orderNumber && pp.itemNumber === item.itemNumber)
      
      const performed = isProcedurePerformed(
        item,
        relevantPerformed
      )
      return {
        ...item,
        isPerformed: performed || item.isPerformed
      }
    })
})

const diagnosticAids = computed(() => {
  if (!orderDetails.value?.items) return []
  return orderDetails.value.items.filter(item => item.itemType === 'DiagnosticAid')
})

const getProgressPercentage = (med) => {
  return med.progress?.percentage || 0
}

const getProgressText = (med) => {
  const progress = med.progress
  if (!progress) return 'Sin datos'
  return `${Math.round(progress.percentage)}%`
}

const getProgressDetails = (med) => {
  const progress = med.progress
  if (!progress) return 'Sin datos de administración'
  return `${progress.administered} de ${progress.total} dosis administradas (${progress.pending} pendientes)`
}

const getProgressClass = (med) => {
  const percentage = getProgressPercentage(med)
  if (percentage >= 100) return 'bg-green-100 text-green-800'
  if (percentage >= 50) return 'bg-yellow-100 text-yellow-800'
  return 'bg-red-100 text-red-800'
}

const handleCreateVisit = (itemNumber) => {
  emit('create-visit', itemNumber)
}

onMounted(async () => {
  loading.value = true
  try {
    orderDetails.value = await nurseStore.loadOrderDetails(props.orderNumber)
  } catch (error) {
    console.error('Error loading order details:', error)
  } finally {
    loading.value = false
  }
})
</script>

