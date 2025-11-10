<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <div class="grid grid-cols-2 gap-4">
      <div>
        <label class="label">ID *</label>
        <input v-model.number="formData.id" type="number" class="input" required />
      </div>
      <div>
        <label class="label">DNI del Paciente *</label>
        <input v-model="formData.patientDni" type="text" class="input" required />
      </div>
      <div class="col-span-2">
        <label class="label">Fecha y Hora *</label>
        <input v-model="formData.date" type="datetime-local" class="input" required />
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

const emit = defineEmits(['submit', 'cancel'])

const formData = ref({
  id: null,
  patientDni: '',
  date: ''
})

const handleSubmit = () => {
  emit('submit', { ...formData.value })
}
</script>

