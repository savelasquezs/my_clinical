<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <div class="grid grid-cols-2 gap-4">
      <div>
        <label class="label">Nombre Completo *</label>
        <input v-model="formData.fullname" type="text" class="input" required />
      </div>
      <div>
        <label class="label">DNI *</label>
        <input v-model="formData.dni" type="text" class="input" :disabled="mode === 'edit'" required />
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
        <label class="label">Género *</label>
        <select v-model="formData.gender" class="input" required>
          <option value="Male">Masculino</option>
          <option value="Female">Femenino</option>
          <option value="Other">Otro</option>
        </select>
      </div>
    </div>
    
    <div>
      <label class="label">Dirección *</label>
      <input v-model="formData.address" type="text" class="input" required />
    </div>

    <div class="border-t pt-4">
      <h3 class="text-lg font-medium mb-4">Contacto de Emergencia</h3>
      <div class="grid grid-cols-2 gap-4">
        <div>
          <label class="label">Nombre *</label>
          <input v-model="formData.emergencyFirstName" type="text" class="input" required />
        </div>
        <div>
          <label class="label">Apellido *</label>
          <input v-model="formData.emergencyLastName" type="text" class="input" required />
        </div>
        <div>
          <label class="label">Relación *</label>
          <input v-model="formData.emergencyRelationship" type="text" class="input" required />
        </div>
        <div>
          <label class="label">Teléfono *</label>
          <input v-model="formData.emergencyPhone" type="text" class="input" required />
        </div>
      </div>
    </div>

    <div class="border-t pt-4">
      <h3 class="text-lg font-medium mb-4">Seguro de Salud</h3>
      <div class="grid grid-cols-2 gap-4">
        <div>
          <label class="label">Compañía *</label>
          <input v-model="formData.insuranceCompanyName" type="text" class="input" required />
        </div>
        <div>
          <label class="label">Número de Póliza *</label>
          <input v-model="formData.insurancePolicyNumber" type="text" class="input" required />
        </div>
        <div>
          <label class="label">Fecha de Expiración *</label>
          <input v-model="formData.insuranceExpirationDate" type="datetime-local" class="input" required />
        </div>
        <div class="flex items-center">
          <input v-model="formData.insuranceIsActive" type="checkbox" class="mr-2" />
          <label>Activo</label>
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
  gender: 'Male',
  emergencyFirstName: '',
  emergencyLastName: '',
  emergencyRelationship: '',
  emergencyPhone: '',
  insuranceCompanyName: '',
  insurancePolicyNumber: '',
  insuranceIsActive: true,
  insuranceExpirationDate: ''
})

watch(() => props.initialData, (newData) => {
  if (newData) {
    // Mapear datos básicos
    formData.value.fullname = newData.fullname || ''
    formData.value.dni = newData.dni || ''
    formData.value.email = newData.email || ''
    formData.value.phonenumber = newData.phonenumber || ''
    formData.value.birthdate = newData.birthdate || ''
    formData.value.address = newData.address || ''
    formData.value.gender = newData.gender || 'Male'
    
    // Mapear contacto de emergencia
    if (newData.emergencyContact) {
      formData.value.emergencyFirstName = newData.emergencyContact.firstname || ''
      formData.value.emergencyLastName = newData.emergencyContact.lastname || ''
      formData.value.emergencyRelationship = newData.emergencyContact.relationship || ''
      formData.value.emergencyPhone = newData.emergencyContact.phoneNumber || ''
    }
    
    // Mapear seguro de salud
    if (newData.insurance) {
      formData.value.insuranceCompanyName = newData.insurance.companyName || ''
      formData.value.insurancePolicyNumber = newData.insurance.policyNumber || ''
      formData.value.insuranceIsActive = newData.insurance.isActive ?? true
      // Convertir fecha de expiración a formato datetime-local
      if (newData.insurance.expirationDate) {
        const date = new Date(newData.insurance.expirationDate)
        const year = date.getFullYear()
        const month = String(date.getMonth() + 1).padStart(2, '0')
        const day = String(date.getDate()).padStart(2, '0')
        const hours = String(date.getHours()).padStart(2, '0')
        const minutes = String(date.getMinutes()).padStart(2, '0')
        formData.value.insuranceExpirationDate = `${year}-${month}-${day}T${hours}:${minutes}`
      }
    }
  }
}, { immediate: true })

const handleSubmit = () => {
  const data = { ...formData.value }
  // En modo edición, no enviar DNI ya que es la clave primaria
  if (props.mode === 'edit') {
    delete data.dni
  }
  emit('submit', data)
}
</script>

