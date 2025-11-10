<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <div class="grid grid-cols-2 gap-4">
      <div>
        <label class="label">ID *</label>
        <input 
          v-model.number="formData.id" 
          type="number" 
          class="input" 
          :disabled="mode === 'edit'"
          :required="mode === 'create'"
        />
      </div>
      <div>
        <label class="label">Nombre *</label>
        <input v-model="formData.name" type="text" class="input" required />
      </div>
      <div>
        <label class="label">Costo *</label>
        <input v-model.number="formData.cost" type="number" step="0.01" class="input" required />
      </div>
    </div>

    <!-- Medication specific fields -->
    <div v-if="type === 'medications'" class="border-t pt-4">
      <h3 class="text-lg font-medium mb-4">Información de Medicamento</h3>
      <div class="grid grid-cols-2 gap-4">
        <div>
          <label class="label">Dosis *</label>
          <input v-model="formData.dose" type="text" class="input" required />
        </div>
        <div>
          <label class="label">Duración del Tratamiento (días) *</label>
          <input v-model.number="formData.treatmentDuration" type="number" class="input" required />
        </div>
      </div>
    </div>

    <!-- Procedure specific fields -->
    <div v-if="type === 'procedures'" class="border-t pt-4">
      <h3 class="text-lg font-medium mb-4">Información de Procedimiento</h3>
      <div class="grid grid-cols-2 gap-4">
        <div>
          <label class="label">Frecuencia *</label>
          <input v-model.number="formData.frequency" type="number" class="input" required />
        </div>
        <div>
          <label class="label">Requiere Especialista</label>
          <input v-model="formData.requiresSpecialist" type="checkbox" class="mr-2" />
        </div>
        <div v-if="formData.requiresSpecialist">
          <label class="label">ID Tipo de Especialista</label>
          <input v-model.number="formData.specialistTypeId" type="number" class="input" />
        </div>
      </div>
    </div>

    <!-- Diagnostic Aid specific fields -->
    <div v-if="type === 'diagnosticAids'" class="border-t pt-4">
      <h3 class="text-lg font-medium mb-4">Información de Ayuda Diagnóstica</h3>
      <div class="grid grid-cols-2 gap-4">
        <div>
          <label class="label">Cantidad *</label>
          <input v-model.number="formData.quantity" type="number" class="input" required />
        </div>
        <div>
          <label class="label">Requiere Especialista</label>
          <input v-model="formData.requiresSpecialist" type="checkbox" class="mr-2" />
        </div>
        <div v-if="formData.requiresSpecialist">
          <label class="label">ID Tipo de Especialista</label>
          <input v-model.number="formData.specialistTypeId" type="number" class="input" />
        </div>
      </div>
    </div>

    <div class="flex justify-end gap-3 pt-4 border-t">
      <button type="button" @click="$emit('cancel')" class="btn btn-secondary">
        Cancelar
      </button>
      <button type="submit" class="btn btn-primary">
        {{ mode === 'create' ? 'Crear' : 'Actualizar' }}
      </button>
    </div>
  </form>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  type: {
    type: String,
    required: true,
    validator: (value) => ['medications', 'procedures', 'diagnosticAids'].includes(value)
  },
  mode: {
    type: String,
    default: 'create',
    validator: (value) => ['create', 'edit'].includes(value)
  },
  initialData: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['submit', 'cancel'])

const formData = ref({
  id: null,
  name: '',
  cost: null,
  // Medication
  dose: '',
  treatmentDuration: null,
  // Procedure
  frequency: null,
  requiresSpecialist: false,
  specialistTypeId: null,
  // Diagnostic Aid
  quantity: null
})

watch(() => props.initialData, (newData) => {
  if (newData) {
    Object.assign(formData.value, newData)
  }
}, { immediate: true })

const handleSubmit = () => {
  // Validar que el ID esté presente cuando se crea
  if (props.mode === 'create' && (!formData.value.id || formData.value.id <= 0)) {
    // El ID es requerido, pero dejamos que el backend valide esto
    // ya que puede haber casos donde se genere automáticamente
  }
  
  const data = { ...formData.value }
  
  // Mapear campos según el tipo para que coincidan con el backend
  if (props.type === 'medications') {
    // El backend espera defaultDose y treatmentDurationDays
    data.defaultDose = data.dose
    data.treatmentDurationDays = data.treatmentDuration
    // Eliminar campos que no pertenecen a medicamentos
    delete data.dose
    delete data.treatmentDuration
    delete data.frequency
    delete data.quantity
    delete data.requiresSpecialist
    delete data.specialistTypeId
  } else if (props.type === 'procedures') {
    // Eliminar campos que no pertenecen a procedimientos
    delete data.dose
    delete data.treatmentDuration
    delete data.quantity
  } else if (props.type === 'diagnosticAids') {
    // Eliminar campos que no pertenecen a ayudas diagnósticas
    delete data.dose
    delete data.treatmentDuration
    delete data.frequency
  }
  
  emit('submit', data)
}
</script>

