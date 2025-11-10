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
        <input v-model="formData.email" type="email" class="input" :class="{ 'border-red-500': errors.email }" required
          @blur="validateEmail" />
        <span v-if="errors.email" class="text-red-500 text-sm mt-1 block">{{ errors.email }}</span>
      </div>
      <div>
        <label class="label">Teléfono *</label>
        <input v-model="formData.phonenumber" type="text" class="input"
          :class="{ 'border-red-500': errors.phonenumber }" required maxlength="10" @input="validatePhone" />
        <span v-if="errors.phonenumber" class="text-red-500 text-sm mt-1 block">{{ errors.phonenumber }}</span>
      </div>
      <div>
        <label class="label">Fecha de Nacimiento *</label>
        <input v-model="formData.birthdate" type="date" class="input" :class="{ 'border-red-500': errors.birthdate }"
          required @change="validateBirthdate" />
        <span v-if="errors.birthdate" class="text-red-500 text-sm mt-1 block">{{ errors.birthdate }}</span>
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
        <input v-model="formData.username" type="text" class="input" :class="{ 'border-red-500': errors.username }"
          required maxlength="15" @input="validateUsername" />
        <span v-if="errors.username" class="text-red-500 text-sm mt-1 block">{{ errors.username }}</span>
      </div>
      <div>
        <label class="label">Contraseña {{ mode === 'create' ? '*' : '(dejar vacío para no cambiar)' }}</label>
        <input v-model="formData.password" type="password" class="input" :class="{ 'border-red-500': errors.password }"
          :required="mode === 'create'" minlength="8" @input="validatePassword" />
        <span v-if="errors.password" class="text-red-500 text-sm mt-1 block">{{ errors.password }}</span>
      </div>
    </div>

    <div>
      <label class="label">Dirección *</label>
      <input v-model="formData.address" type="text" class="input" :class="{ 'border-red-500': errors.address }" required
        maxlength="30" @input="validateAddress" />
      <span v-if="errors.address" class="text-red-500 text-sm mt-1 block">{{ errors.address }}</span>
    </div>

    <div class="flex justify-end gap-3 pt-4 border-t">
      <button type="button" @click="$emit('cancel')" class="btn btn-secondary">
        Cancelar
      </button>
      <button type="submit" class="btn btn-primary" :disabled="hasErrors">
        {{ mode === 'create' ? 'Crear' : 'Actualizar' }}
      </button>
    </div>
  </form>
</template>

<script setup>
import { ref, watch, computed } from 'vue'

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

const errors = ref({
  username: '',
  password: '',
  email: '',
  phonenumber: '',
  birthdate: '',
  address: ''
})

watch(() => props.initialData, (newData) => {
  if (newData) {
    Object.assign(formData.value, newData)
    formData.value.password = '' // Don't show password in edit mode
    // Limpiar errores al cargar datos
    Object.keys(errors.value).forEach(key => {
      errors.value[key] = ''
    })
  }
}, { immediate: true })

const hasErrors = computed(() => {
  return Object.values(errors.value).some(error => error !== '')
})

const validateUsername = () => {
  const username = formData.value.username
  if (!username) {
    errors.value.username = ''
    return
  }
  if (!/^[a-zA-Z0-9]+$/.test(username)) {
    errors.value.username = 'El usuario solo puede contener letras y números'
  } else if (username.length > 15) {
    errors.value.username = 'El usuario no puede tener más de 15 caracteres'
  } else {
    errors.value.username = ''
  }
}

const validatePassword = () => {
  const password = formData.value.password
  // En modo edición, la contraseña es opcional
  if (props.mode === 'edit' && !password) {
    errors.value.password = ''
    return
  }
  if (!password) {
    errors.value.password = ''
    return
  }
  if (password.length < 8) {
    errors.value.password = 'La contraseña debe tener al menos 8 caracteres'
  } else if (!/[A-Z]/.test(password)) {
    errors.value.password = 'La contraseña debe tener al menos una letra mayúscula'
  } else if (!/[a-z]/.test(password)) {
    errors.value.password = 'La contraseña debe tener al menos una letra minúscula'
  } else if (!/[0-9]/.test(password)) {
    errors.value.password = 'La contraseña debe tener al menos un número'
  } else if (!/[^a-zA-Z0-9]/.test(password)) {
    errors.value.password = 'La contraseña debe tener al menos un carácter especial'
  } else {
    errors.value.password = ''
  }
}

const validateEmail = () => {
  const email = formData.value.email
  if (!email) {
    errors.value.email = ''
    return
  }
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!emailRegex.test(email)) {
    errors.value.email = 'El email debe tener un formato válido (ejemplo: usuario@dominio.com)'
  } else {
    errors.value.email = ''
  }
}

const validatePhone = () => {
  const phone = formData.value.phonenumber
  if (!phone) {
    errors.value.phonenumber = ''
    return
  }
  if (!/^[0-9]+$/.test(phone)) {
    errors.value.phonenumber = 'El teléfono solo puede contener números'
  } else if (phone.length < 1 || phone.length > 10) {
    errors.value.phonenumber = 'El teléfono debe contener entre 1 y 10 dígitos'
  } else {
    errors.value.phonenumber = ''
  }
}

const validateBirthdate = () => {
  const birthdate = formData.value.birthdate
  if (!birthdate) {
    errors.value.birthdate = ''
    return
  }
  const birthDate = new Date(birthdate)
  const today = new Date()
  const age = today.getFullYear() - birthDate.getFullYear()
  const monthDiff = today.getMonth() - birthDate.getMonth()
  const dayDiff = today.getDate() - birthDate.getDate()

  let actualAge = age
  if (monthDiff < 0 || (monthDiff === 0 && dayDiff < 0)) {
    actualAge--
  }

  if (actualAge > 150) {
    errors.value.birthdate = 'La fecha de nacimiento no puede ser mayor a 150 años'
  } else if (birthDate > today) {
    errors.value.birthdate = 'La fecha de nacimiento no puede ser en el futuro'
  } else {
    errors.value.birthdate = ''
  }
}

const validateAddress = () => {
  const address = formData.value.address
  if (!address) {
    errors.value.address = ''
    return
  }
  if (address.length > 30) {
    errors.value.address = 'La dirección no puede tener más de 30 caracteres'
  } else {
    errors.value.address = ''
  }
}

const handleSubmit = () => {
  // Validar todos los campos antes de enviar
  validateUsername()
  if (props.mode === 'create' || formData.value.password) {
    validatePassword()
  }
  validateEmail()
  validatePhone()
  validateBirthdate()
  validateAddress()

  // Si hay errores, no enviar
  if (hasErrors.value) {
    return
  }

  const data = { ...formData.value }
  if (props.mode === 'edit' && !data.password) {
    delete data.password
  }
  emit('submit', data)
}
</script>
