<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <div class="grid grid-cols-2 gap-4">
      <div>
        <label class="label">DNI del Paciente *</label>
        <input v-model="formData.patientDni" type="text" class="input" required />
      </div>
      <div>
        <label class="label">Número de Orden *</label>
        <input v-model.number="formData.orderNumber" type="number" class="input" required />
      </div>
      <div>
        <label class="label">Número de Item *</label>
        <input v-model.number="formData.itemNumber" type="number" class="input" required />
      </div>
      <div>
        <label class="label">Fecha de Realización *</label>
        <input v-model="formData.performedAt" type="datetime-local" class="input" required />
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
      <button type="submit" class="btn btn-primary">
        Crear
      </button>
    </div>
  </form>
</template>

<script setup>
import { ref } from 'vue'

const emit = defineEmits(['submit', 'cancel'])

const formData = ref({
  patientDni: '',
  orderNumber: null,
  itemNumber: null,
  performedAt: '',
  bloodPressure: '',
  temperature: null,
  pulse: null,
  oxygenLevel: null,
  testsPerformed: '',
  notes: ''
})

const handleSubmit = () => {
  emit('submit', { ...formData.value })
}
</script>

