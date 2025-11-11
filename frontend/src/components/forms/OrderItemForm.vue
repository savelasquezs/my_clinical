<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <div class="grid grid-cols-2 gap-4">
      <div>
        <label class="label">Tipo de Item *</label>
        <select v-model="formData.itemType" class="input" required @change="onTypeChange">
          <option value="">Seleccione un tipo</option>
          <option value="Medication">Medicamento</option>
          <option value="Procedure">Procedimiento</option>
          <option value="DiagnosticAid">Ayuda Diagnóstica</option>
        </select>
      </div>
      <div>
        <label class="label">Costo *</label>
        <input 
          v-model.number="formData.cost" 
          type="number" 
          step="0.01" 
          class="input" 
          :readonly="formData.itemType === 'Medication' || formData.itemType === 'DiagnosticAid' || formData.itemType === 'Procedure'"
          :class="{ 'bg-gray-100 cursor-not-allowed': formData.itemType === 'Medication' || formData.itemType === 'DiagnosticAid' || formData.itemType === 'Procedure' }"
          required 
        />
        <p v-if="formData.itemType === 'Medication' || formData.itemType === 'DiagnosticAid' || formData.itemType === 'Procedure'" class="text-xs text-gray-500 mt-1">
          Costo calculado automáticamente
        </p>
      </div>

      <!-- Campos para Medication -->
      <template v-if="formData.itemType === 'Medication'">
        <div>
          <label class="label">Medicamento *</label>
          <select v-model.number="formData.medicationId" class="input" required>
            <option value="">Seleccione un medicamento</option>
            <option v-for="med in medications" :key="med.id" :value="med.id">
              {{ med.name }} - ${{ med.cost }}
            </option>
          </select>
        </div>
        <div>
          <label class="label">Dosis por día *</label>
          <input v-model.number="formData.dosePerDay" type="number" step="0.01" min="0" class="input" required />
        </div>
        <div>
          <label class="label">Duración del Tratamiento (días) *</label>
          <input v-model.number="formData.treatmentDuration" type="number" min="1" class="input" required />
        </div>
      </template>

      <!-- Campos para Procedure -->
      <template v-if="formData.itemType === 'Procedure'">
        <div>
          <label class="label">Procedimiento *</label>
          <select v-model.number="formData.procedureId" class="input" required>
            <option value="">Seleccione un procedimiento</option>
            <option v-for="proc in procedures" :key="proc.id" :value="proc.id">
              {{ proc.name }} - ${{ proc.cost }}
            </option>
          </select>
        </div>
        <div>
          <label class="label">Frecuencia *</label>
          <input v-model.number="formData.frequency" type="number" min="1" class="input" required />
        </div>
        <div class="col-span-2">
          <label class="flex items-center gap-2">
            <input v-model="formData.requiresSpecialist" type="checkbox" class="checkbox" />
            <span>Requiere Especialista</span>
          </label>
        </div>
        <div v-if="formData.requiresSpecialist">
          <label class="label">ID Tipo de Especialista</label>
          <input v-model.number="formData.specialistTypeId" type="number" class="input" />
        </div>
      </template>

      <!-- Campos para DiagnosticAid -->
      <template v-if="formData.itemType === 'DiagnosticAid'">
        <div>
          <label class="label">Ayuda Diagnóstica *</label>
          <select v-model.number="formData.diagnosticAidId" class="input" required>
            <option value="">Seleccione una ayuda diagnóstica</option>
            <option v-for="aid in diagnosticAids" :key="aid.id" :value="aid.id">
              {{ aid.name }} - ${{ aid.cost }}
            </option>
          </select>
        </div>
        <div>
          <label class="label">Cantidad *</label>
          <input v-model.number="formData.quantity" type="number" min="1" class="input" required />
        </div>
        <div class="col-span-2">
          <label class="flex items-center gap-2">
            <input v-model="formData.requiresSpecialist" type="checkbox" class="checkbox" />
            <span>Requiere Especialista</span>
          </label>
        </div>
        <div v-if="formData.requiresSpecialist">
          <label class="label">ID Tipo de Especialista</label>
          <input v-model.number="formData.specialistTypeId" type="number" class="input" />
        </div>
      </template>
    </div>

    <div class="flex justify-end gap-3 pt-4 border-t">
      <button type="button" @click="$emit('cancel')" class="btn btn-secondary">
        Cancelar
      </button>
      <button type="submit" class="btn btn-primary">
        {{ mode === 'edit' ? 'Actualizar' : 'Agregar' }}
      </button>
    </div>
  </form>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { supportService } from '@/services/supportService'

const props = defineProps({
  mode: {
    type: String,
    default: 'create',
    validator: (value) => ['create', 'edit'].includes(value)
  },
  initialData: {
    type: Object,
    default: null
  },
  orderNumber: {
    type: Number,
    required: true
  }
})

const emit = defineEmits(['submit', 'cancel'])

const medications = ref([])
const procedures = ref([])
const diagnosticAids = ref([])
const loading = ref(false)

const formData = ref({
  orderNumber: props.orderNumber,
  itemType: '',
  cost: 0,
  medicationId: null,
  dose: '',
  dosePerDay: null,
  treatmentDuration: null,
  procedureId: null,
  frequency: null,
  requiresSpecialist: false,
  specialistTypeId: null,
  diagnosticAidId: null,
  quantity: null
})

// Computed para obtener el medicamento seleccionado
const selectedMedication = computed(() => {
  if (!formData.value.medicationId) return null
  return medications.value.find(med => med.id === formData.value.medicationId)
})

// Computed para obtener la ayuda diagnóstica seleccionada
const selectedDiagnosticAid = computed(() => {
  if (!formData.value.diagnosticAidId) return null
  return diagnosticAids.value.find(aid => aid.id === formData.value.diagnosticAidId)
})

// Computed para obtener el procedimiento seleccionado
const selectedProcedure = computed(() => {
  if (!formData.value.procedureId) return null
  return procedures.value.find(proc => proc.id === formData.value.procedureId)
})

// Computed para calcular el costo automáticamente
const calculatedCost = computed(() => {
  // Para medicamentos: dosis por día × duración × costo del medicamento
  if (formData.value.itemType === 'Medication') {
    const dosePerDay = formData.value.dosePerDay || 0
    const treatmentDuration = formData.value.treatmentDuration || 0
    const medicationCost = selectedMedication.value?.cost || 0
    
    return dosePerDay * treatmentDuration * medicationCost
  }
  
  // Para procedimientos: frecuencia × costo del procedimiento
  if (formData.value.itemType === 'Procedure') {
    const frequency = formData.value.frequency || 0
    const procedureCost = selectedProcedure.value?.cost || 0
    
    return frequency * procedureCost
  }
  
  // Para ayudas diagnósticas: cantidad × costo de la ayuda diagnóstica
  if (formData.value.itemType === 'DiagnosticAid') {
    const quantity = formData.value.quantity || 0
    const diagnosticAidCost = selectedDiagnosticAid.value?.cost || 0
    
    return quantity * diagnosticAidCost
  }
  
  // Para otros tipos, usar el costo manual
  return formData.value.cost
})

// Watch para actualizar el costo cuando cambian los valores
watch([
  () => formData.value.itemType,
  () => formData.value.dosePerDay, 
  () => formData.value.treatmentDuration, 
  () => formData.value.medicationId,
  () => formData.value.frequency,
  () => formData.value.procedureId,
  () => formData.value.quantity,
  () => formData.value.diagnosticAidId,
  selectedMedication,
  selectedProcedure,
  selectedDiagnosticAid
], () => {
  if (formData.value.itemType === 'Medication' || formData.value.itemType === 'DiagnosticAid' || formData.value.itemType === 'Procedure') {
    formData.value.cost = calculatedCost.value
  }
}, { immediate: true })

const loadInventory = async () => {
  loading.value = true
  try {
    const [meds, procs, aids] = await Promise.all([
      supportService.getAllMedications(),
      supportService.getAllProcedures(),
      supportService.getAllDiagnosticAids()
    ])
    medications.value = meds
    procedures.value = procs
    diagnosticAids.value = aids
  } catch (error) {
    console.error('Error loading inventory:', error)
  } finally {
    loading.value = false
  }
}

const onTypeChange = () => {
  // Limpiar campos específicos cuando cambia el tipo
  formData.value.medicationId = null
  formData.value.procedureId = null
  formData.value.diagnosticAidId = null
  formData.value.dose = ''
  formData.value.dosePerDay = null
  formData.value.treatmentDuration = null
  formData.value.frequency = null
  formData.value.quantity = null
  formData.value.requiresSpecialist = false
  formData.value.specialistTypeId = null
  
  // Si no es medicamento, procedimiento ni ayuda diagnóstica, resetear el costo
  if (formData.value.itemType !== 'Medication' && formData.value.itemType !== 'DiagnosticAid' && formData.value.itemType !== 'Procedure') {
    formData.value.cost = 0
  }
}

const handleSubmit = () => {
  const data = {
    orderNumber: formData.value.orderNumber,
    itemType: formData.value.itemType,
    cost: formData.value.cost
  }

  if (formData.value.itemType === 'Medication') {
    data.medicationId = formData.value.medicationId
    // Convertir dosePerDay a string para el campo dose (mantener compatibilidad con backend)
    if (formData.value.dosePerDay != null) {
      data.dose = formData.value.dosePerDay.toString()
    }
    if (formData.value.treatmentDuration) data.treatmentDuration = formData.value.treatmentDuration
    // El costo ya está calculado automáticamente
  } else if (formData.value.itemType === 'Procedure') {
    data.procedureId = formData.value.procedureId
    if (formData.value.frequency) data.frequency = formData.value.frequency
    data.requiresSpecialist = formData.value.requiresSpecialist
    if (formData.value.specialistTypeId) data.specialistTypeId = formData.value.specialistTypeId
  } else if (formData.value.itemType === 'DiagnosticAid') {
    data.diagnosticAidId = formData.value.diagnosticAidId
    if (formData.value.quantity) data.quantity = formData.value.quantity
    data.requiresSpecialist = formData.value.requiresSpecialist
    if (formData.value.specialistTypeId) data.specialistTypeId = formData.value.specialistTypeId
  }

  emit('submit', data)
}

// Cargar datos iniciales si estamos en modo edición
watch(() => props.initialData, (newData) => {
  if (newData && props.mode === 'edit') {
    formData.value.itemType = newData.itemType || ''
    formData.value.cost = newData.cost || 0
    
    if (newData.itemType === 'Medication') {
      formData.value.medicationId = newData.medicationId || null
      formData.value.dose = newData.dose || ''
      // Intentar convertir dose a número para dosePerDay
      const doseNum = parseFloat(newData.dose)
      formData.value.dosePerDay = isNaN(doseNum) ? null : doseNum
      formData.value.treatmentDuration = newData.treatmentDuration || null
    } else if (newData.itemType === 'Procedure') {
      formData.value.procedureId = newData.procedureId || null
      formData.value.frequency = newData.frequency || null
      formData.value.requiresSpecialist = newData.requiresSpecialist || false
      formData.value.specialistTypeId = newData.specialistTypeId || null
    } else if (newData.itemType === 'DiagnosticAid') {
      formData.value.diagnosticAidId = newData.diagnosticAidId || null
      formData.value.quantity = newData.quantity || null
      formData.value.requiresSpecialist = newData.requiresSpecialist || false
      formData.value.specialistTypeId = newData.specialistTypeId || null
    }
  }
}, { immediate: true })

onMounted(async () => {
  await loadInventory()
  // Recalcular costo después de cargar inventario si es medicamento, procedimiento o ayuda diagnóstica
  if (formData.value.itemType === 'Medication' && formData.value.medicationId) {
    formData.value.cost = calculatedCost.value
  } else if (formData.value.itemType === 'Procedure' && formData.value.procedureId) {
    formData.value.cost = calculatedCost.value
  } else if (formData.value.itemType === 'DiagnosticAid' && formData.value.diagnosticAidId) {
    formData.value.cost = calculatedCost.value
  }
})
</script>

