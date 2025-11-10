<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <div class="grid grid-cols-2 gap-4">
      <div>
        <PatientSelector 
          v-model="formData.patientDni"
          label="Paciente *"
          :required="true"
        />
      </div>
      <div>
        <label class="label">Fecha *</label>
        <input v-model="formData.date" type="datetime-local" class="input" required />
      </div>
      <div class="col-span-2">
        <label class="label">Razón de Consulta *</label>
        <textarea v-model="formData.consultationReason" class="input" rows="3" required></textarea>
      </div>
      <div class="col-span-2">
        <label class="label">Síntomas *</label>
        <textarea v-model="formData.symptoms" class="input" rows="3" required></textarea>
      </div>
      <div class="col-span-2">
        <label class="label">Diagnóstico *</label>
        <textarea v-model="formData.diagnosis" class="input" rows="3" required></textarea>
      </div>
      <div>
        <label class="label">Número de Orden (opcional)</label>
        <input v-model.number="formData.orderNumber" type="number" class="input" />
      </div>
    </div>

    <div class="flex justify-end gap-3 pt-4 border-t">
      <button type="button" @click="$emit('cancel')" class="btn btn-secondary">
        Cancelar
      </button>
      <button type="submit" class="btn btn-primary">
        Crear
      </button>
    </div>
  </form>
</template>

<script setup>
import { ref } from 'vue'
import PatientSelector from '@/components/shared/PatientSelector.vue'

const emit = defineEmits(['submit', 'cancel'])

const formData = ref({
  patientDni: '',
  date: '',
  consultationReason: '',
  symptoms: '',
  diagnosis: '',
  orderNumber: null
})

const handleSubmit = () => {
  emit('submit', { ...formData.value })
}
</script>

