<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <div class="grid grid-cols-2 gap-4">
      <div>
        <label class="label">ID *</label>
        <input v-model.number="formData.id" type="number" class="input" :disabled="mode === 'edit'" required />
      </div>
      <div>
        <PatientSelector 
          v-model="formData.patientDni"
          label="Paciente *"
          :required="true"
          :disabled="mode === 'edit'"
        />
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
        {{ mode === 'create' ? 'Crear' : 'Actualizar' }}
      </button>
    </div>
  </form>
</template>

<script setup>
import { ref, watch } from 'vue'
import PatientSelector from '@/components/shared/PatientSelector.vue'

const props = defineProps({
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
  patientDni: '',
  date: ''
})

watch(() => props.initialData, (newData) => {
  if (newData) {
    formData.value.id = newData.id1 || null
    formData.value.patientDni = newData.patientDni || ''
    if (newData.date1) {
      const date = new Date(newData.date1)
      const year = date.getFullYear()
      const month = String(date.getMonth() + 1).padStart(2, '0')
      const day = String(date.getDate()).padStart(2, '0')
      const hours = String(date.getHours()).padStart(2, '0')
      const minutes = String(date.getMinutes()).padStart(2, '0')
      formData.value.date = `${year}-${month}-${day}T${hours}:${minutes}`
    }
  }
}, { immediate: true })

const handleSubmit = () => {
  const data = { ...formData.value }
  if (props.mode === 'edit') {
    delete data.id
    delete data.patientDni
  }
  emit('submit', data)
}
</script>

