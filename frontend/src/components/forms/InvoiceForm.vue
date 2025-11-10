<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <div class="grid grid-cols-2 gap-4">
      <div>
        <label class="label">Número de Factura *</label>
        <input v-model.number="formData.invoiceNumber" type="number" class="input" required />
      </div>
      <div>
        <PatientSelector 
          v-model="formData.patientDni"
          label="Paciente *"
          :required="true"
        />
      </div>
      <div>
        <label class="label">DNI del Doctor *</label>
        <input v-model="formData.doctorDni" type="text" class="input" required />
      </div>
      <div>
        <label class="label">Fecha de Factura *</label>
        <input v-model="formData.invoiceDate" type="datetime-local" class="input" required />
      </div>
      <div class="col-span-2">
        <label class="label">Números de Orden (separados por comas)</label>
        <input v-model="formData.orderNumbers" type="text" class="input" placeholder="1, 2, 3" />
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
  invoiceNumber: null,
  patientDni: '',
  doctorDni: '',
  invoiceDate: '',
  orderNumbers: ''
})

const handleSubmit = () => {
  const data = {
    ...formData.value,
    orderNumbers: formData.value.orderNumbers
      .split(',')
      .map(n => parseInt(n.trim()))
      .filter(n => !isNaN(n))
  }
  emit('submit', data)
}
</script>

