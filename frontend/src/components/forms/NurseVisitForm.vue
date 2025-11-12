<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <div class="grid grid-cols-2 gap-4">
      <div>
        <label class="label">Número de Orden *</label>
        <input v-model.number="formData.orderNumber" type="number" class="input" required readonly />
      </div>
      <div>
        <label class="label">Item de Visita de Enfermería *</label>
        <input v-model.number="formData.itemNumber" type="number" class="input" required readonly />
      </div>
      <div class="col-span-2">
        <label class="label">Fecha de Realización *</label>
        <input v-model="formData.performedAt" type="datetime-local" class="input" required />
      </div>
      <div class="col-span-2">
        <label class="label">Fecha de Visita *</label>
        <input v-model="formData.visitTime" type="datetime-local" class="input" required />
      </div>
    </div>

    <div class="border-t pt-4">
      <h3 class="text-lg font-medium mb-4">Datos Vitales</h3>
      <div class="grid grid-cols-2 gap-4">
        <div>
          <label class="label">Presión Arterial</label>
          <input v-model="formData.bloodPressure" type="text" class="input" />
        </div>
        <div>
          <label class="label">Temperatura (°C)</label>
          <input v-model.number="formData.temperature" type="number" step="0.1" class="input" />
        </div>
        <div>
          <label class="label">Pulso (bpm)</label>
          <input v-model.number="formData.pulse" type="number" class="input" />
        </div>
        <div>
          <label class="label">Nivel de Oxígeno (%)</label>
          <input v-model.number="formData.oxygenLevel" type="number" class="input" />
        </div>
      </div>
    </div>

    <!-- Medicamentos a Administrar -->
    <div v-if="pendingMedications.length > 0" class="border-t pt-4">
      <h3 class="text-lg font-medium mb-4">Medicamentos a Administrar</h3>
      <div class="space-y-4">
        <div 
          v-for="med in pendingMedications" 
          :key="`med-${med.itemNumber}`"
          class="border rounded-lg p-4"
        >
          <div class="flex items-start justify-between mb-3">
            <div class="flex-1">
              <h4 class="font-medium">{{ med.medicationName }}</h4>
              <p class="text-sm text-gray-600">
                Dosis por día: {{ med.dose }} | Duración: {{ med.treatmentDuration }} días
              </p>
              <p class="text-xs text-gray-500 mt-1">
                Pendientes: {{ getPendingDoses(med) }} dosis
              </p>
            </div>
            <label class="flex items-center gap-2">
              <input 
                v-model="selectedMedications" 
                :value="med.itemNumber" 
                type="checkbox" 
                class="checkbox"
              />
              <span class="text-sm">Administrar</span>
            </label>
          </div>
          <div v-if="selectedMedications.includes(med.itemNumber)" class="mt-3 space-y-2">
            <div>
              <label class="label text-sm">Dosis a Administrar *</label>
              <input 
                v-model.number="medicationDoses[med.itemNumber]" 
                type="number" 
                step="0.01"
                min="0"
                :max="getPendingDoses(med)"
                class="input" 
                required
              />
              <p class="text-xs text-gray-500 mt-1">
                Máximo: {{ getPendingDoses(med) }} dosis
              </p>
            </div>
            <div>
              <label class="label text-sm">Ruta de Administración *</label>
              <input 
                v-model="medicationRoutes[med.itemNumber]" 
                type="text" 
                class="input" 
                placeholder="Ej: Oral, Intravenosa, etc."
                required
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Procedimientos a Realizar -->
    <div v-if="pendingProcedures.length > 0" class="border-t pt-4">
      <h3 class="text-lg font-medium mb-4">Procedimientos a Realizar</h3>
      <div class="space-y-3">
        <div 
          v-for="proc in pendingProcedures" 
          :key="`proc-${proc.itemNumber}`"
          class="border rounded-lg p-4 flex items-center justify-between"
        >
          <div>
            <h4 class="font-medium">{{ proc.procedureName }}</h4>
            <p class="text-sm text-gray-600">Frecuencia: {{ proc.frequency }}</p>
          </div>
          <label class="flex items-center gap-2">
            <input 
              v-model="selectedProcedures" 
              :value="proc.itemNumber" 
              type="checkbox" 
              class="checkbox"
            />
            <span class="text-sm">Marcar como realizado</span>
          </label>
        </div>
      </div>
    </div>

    <div>
      <label class="label">Pruebas Realizadas</label>
      <textarea v-model="formData.testsPerformed" class="input" rows="3"></textarea>
    </div>

    <div>
      <label class="label">Notas</label>
      <textarea v-model="formData.notes" class="input" rows="3"></textarea>
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
        Crear
      </button>
    </div>
  </form>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { calculateMedicationProgress, isProcedurePerformed } from '@/utils/nurseUtils'
import { useToast } from '@/composables/useToast'

const props = defineProps({
  orderNumber: {
    type: Number,
    required: true
  },
  nurseVisitItemNumber: {
    type: Number,
    required: true
  },
  patientDni: {
    type: String,
    default: ''
  },
  orderDetails: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['submit', 'cancel'])
const toast = useToast()

const formData = ref({
  patientDni: '',
  orderNumber: props.orderNumber,
  itemNumber: props.nurseVisitItemNumber,
  performedAt: '',
  visitTime: '',
  bloodPressure: '',
  temperature: null,
  pulse: null,
  oxygenLevel: null,
  testsPerformed: '',
  notes: ''
})

const selectedMedications = ref([])
const medicationDoses = ref({})
const medicationRoutes = ref({})
const selectedProcedures = ref([])

const pendingMedications = computed(() => {
  if (!props.orderDetails?.items) return []
  return props.orderDetails.items
    .filter(item => item.itemType === 'Medication')
    .map(item => {
      // Filtrar administeredMedications que corresponden a este item
      const relevantAdministered = (props.orderDetails.administeredMedications || [])
        .filter(am => am.orderNumber === item.orderNumber && am.itemNumber === item.itemNumber)
      
      const progress = calculateMedicationProgress(
        item,
        relevantAdministered
      )
      return {
        ...item,
        progress,
        pendingDoses: progress.pending
      }
    })
    .filter(med => med.pendingDoses > 0)
})

const pendingProcedures = computed(() => {
  if (!props.orderDetails?.items) return []
  return props.orderDetails.items
    .filter(item => 
      item.itemType === 'Procedure' && 
      item.procedureName?.toLowerCase() !== 'visita de enfermeria' &&
      !item.isPerformed
    )
})

const canSubmit = computed(() => {
  // Debe haber al menos un medicamento o procedimiento seleccionado
  const hasMedications = selectedMedications.value.length > 0
  const hasProcedures = selectedProcedures.value.length > 0
  
  // Validar que los medicamentos seleccionados tengan dosis y ruta
  if (hasMedications) {
    for (const medItemNumber of selectedMedications.value) {
      if (!medicationDoses.value[medItemNumber] || medicationDoses.value[medItemNumber] <= 0) {
        return false
      }
      if (!medicationRoutes.value[medItemNumber] || medicationRoutes.value[medItemNumber].trim() === '') {
        return false
      }
    }
  }
  
  return hasMedications || hasProcedures
})

const getPendingDoses = (med) => {
  return med.pendingDoses || 0
}

watch(selectedMedications, (newVal, oldVal) => {
  // Limpiar dosis y rutas de medicamentos deseleccionados
  oldVal?.forEach(itemNumber => {
    if (!newVal.includes(itemNumber)) {
      delete medicationDoses.value[itemNumber]
      delete medicationRoutes.value[itemNumber]
    }
  })
})

const handleSubmit = () => {
  // Validar dosis no exceda pendientes
  for (const medItemNumber of selectedMedications.value) {
    const med = pendingMedications.value.find(m => m.itemNumber === medItemNumber)
    if (med && medicationDoses.value[medItemNumber] > getPendingDoses(med)) {
      toast.error(`La dosis a administrar no puede exceder las dosis pendientes (${getPendingDoses(med)})`)
      return
    }
  }

  const administeredMedications = selectedMedications.value.map(medItemNumber => {
    const med = pendingMedications.value.find(m => m.itemNumber === medItemNumber)
    return {
      medicationOrderItemNumber: medItemNumber,
      dose: medicationDoses.value[medItemNumber].toString(),
      administrationRoute: medicationRoutes.value[medItemNumber]
    }
  })

  const data = {
    ...formData.value,
    administeredMedications: administeredMedications.length > 0 ? administeredMedications : undefined,
    performedProcedures: selectedProcedures.value.length > 0 ? selectedProcedures.value : undefined,
    cost: 0 // El costo se calcula en el backend
  }

  emit('submit', data)
}

onMounted(() => {
  // Establecer fecha actual por defecto
  const now = new Date()
  const nowStr = now.toISOString().slice(0, 16)
  formData.value.performedAt = nowStr
  formData.value.visitTime = nowStr
  
  // Establecer patientDni desde props
  if (props.patientDni) {
    formData.value.patientDni = props.patientDni
  }
})
</script>
