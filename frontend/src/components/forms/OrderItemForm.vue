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
        <input v-model.number="formData.cost" type="number" step="0.01" class="input" required />
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
          <label class="label">Dosis</label>
          <input v-model="formData.dose" type="text" class="input" />
        </div>
        <div>
          <label class="label">Duración del Tratamiento (días)</label>
          <input v-model.number="formData.treatmentDuration" type="number" class="input" />
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
          <label class="label">Frecuencia</label>
          <input v-model.number="formData.frequency" type="number" class="input" />
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
          <label class="label">Cantidad</label>
          <input v-model.number="formData.quantity" type="number" class="input" />
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
import { ref, onMounted, watch } from 'vue'
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
  treatmentDuration: null,
  procedureId: null,
  frequency: null,
  requiresSpecialist: false,
  specialistTypeId: null,
  diagnosticAidId: null,
  quantity: null
})

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
  formData.value.treatmentDuration = null
  formData.value.frequency = null
  formData.value.quantity = null
  formData.value.requiresSpecialist = false
  formData.value.specialistTypeId = null
}

const handleSubmit = () => {
  const data = {
    orderNumber: formData.value.orderNumber,
    itemType: formData.value.itemType,
    cost: formData.value.cost
  }

  if (formData.value.itemType === 'Medication') {
    data.medicationId = formData.value.medicationId
    if (formData.value.dose) data.dose = formData.value.dose
    if (formData.value.treatmentDuration) data.treatmentDuration = formData.value.treatmentDuration
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

onMounted(() => {
  loadInventory()
})
</script>

