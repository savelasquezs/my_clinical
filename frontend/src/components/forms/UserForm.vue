<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <div class="grid grid-cols-2 gap-4">
      <div>
        <label class="label">Nombre Completo *</label>
        <input v-model="formData.fullname" type="text" class="input" required />
      </div>
      <div>
        <label class="label">DNI *</label>
        <input v-model="formData.dni" type="text" class="input" required :disabled="mode === 'edit'" />
      </div>
      <div>
        <label class="label">Email *</label>
        <input v-model="formData.email" type="email" class="input" required />
      </div>
      <div>
        <label class="label">Teléfono *</label>
        <input v-model="formData.phonenumber" type="text" class="input" required />
      </div>
      <div>
        <label class="label">Fecha de Nacimiento *</label>
        <input v-model="formData.birthdate" type="date" class="input" required />
      </div>
      <div>
        <label class="label">Rol *</label>
        <select v-model="formData.role" class="input" required>
          <option value="Admin">Admin</option>
          <option value="Doctor">Doctor</option>
          <option value="Nurse">Nurse</option>
          <option value="RRHH">RRHH</option>
          <option value="Support">Support</option>
        </select>
      </div>
      <div>
        <label class="label">Usuario *</label>
        <input v-model="formData.username" type="text" class="input" required />
      </div>
      <div>
        <label class="label">Contraseña {{ mode === 'create' ? '*' : '(dejar vacío para no cambiar)' }}</label>
        <input v-model="formData.password" type="password" class="input" :required="mode === 'create'" />
      </div>
    </div>
    
    <div>
      <label class="label">Dirección *</label>
      <input v-model="formData.address" type="text" class="input" required />
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
  fullname: '',
  dni: '',
  email: '',
  phonenumber: '',
  birthdate: '',
  address: '',
  role: 'Admin',
  username: '',
  password: ''
})

watch(() => props.initialData, (newData) => {
  if (newData) {
    Object.assign(formData.value, newData)
    formData.value.password = '' // Don't show password in edit mode
  }
}, { immediate: true })

const handleSubmit = () => {
  const data = { ...formData.value }
  if (props.mode === 'edit' && !data.password) {
    delete data.password
  }
  emit('submit', data)
}
</script>

