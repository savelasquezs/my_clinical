<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <div class="grid grid-cols-2 gap-4">
      <div>
        <label class="label">Número de Orden *</label>
        <input v-model.number="formData.orderNumber" type="number" class="input" required />
      </div>
      <div>
        <label class="label">Fecha de Creación *</label>
        <input v-model="formData.creationDate" type="datetime-local" class="input" required />
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
  orderNumber: null,
  creationDate: ''
})

const handleSubmit = () => {
  emit('submit', { ...formData.value })
}
</script>

