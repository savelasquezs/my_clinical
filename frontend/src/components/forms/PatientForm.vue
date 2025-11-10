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
      <input v-model="formData.address" type="text" class="input" :class="{ 'border-red-500': errors.address }" required
        maxlength="30" @input="validateAddress" />
      <span v-if="errors.address" class="text-red-500 text-sm mt-1 block">{{ errors.address }}</span>
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
          <input v-model="formData.emergencyPhone" type="text" class="input"
            :class="{ 'border-red-500': errors.emergencyPhone }" required maxlength="10"
            @input="validateEmergencyPhone" />
          <span v-if="errors.emergencyPhone" class="text-red-500 text-sm mt-1 block">{{ errors.emergencyPhone }}</span>
        </div>
      </div>
    </div>

    <div class="border-t pt-4">
      <h3 class="text-lg font-medium mb-4">Seguro de Salud</h3>
      <div class="grid grid-cols-2 gap-4">
        <div>
          <label class="label">Compañía *</label>
          <input v-model="formData.insuranceCompanyName" type="text" class="input"
            :class="{ 'border-red-500': errors.insuranceCompanyName }" required list="insurance-companies"
            @blur="validateInsuranceCompany" />
          <datalist id="insurance-companies">
            <option v-for="company in validInsuranceCompanies" :key="company" :value="company" />
          </datalist>
          <span v-if="errors.insuranceCompanyName" class="text-red-500 text-sm mt-1 block">{{
            errors.insuranceCompanyName }}</span>
        </div>
        <div>
          <label class="label">Número de Póliza *</label>
          <input v-model="formData.insurancePolicyNumber" type="text" class="input"
            :class="{ 'border-red-500': errors.insurancePolicyNumber }" required maxlength="50"
            @input="validatePolicyNumber" />
          <span v-if="errors.insurancePolicyNumber" class="text-red-500 text-sm mt-1 block">{{
            errors.insurancePolicyNumber }}</span>
        </div>
        <div>
          <label class="label">Fecha de Expiración *</label>
          <input v-model="formData.insuranceExpirationDate" type="datetime-local" class="input"
            :class="{ 'border-red-500': errors.insuranceExpirationDate }" required
            @change="validateInsuranceExpirationDate" />
          <span v-if="errors.insuranceExpirationDate" class="text-red-500 text-sm mt-1 block">{{
            errors.insuranceExpirationDate }}</span>
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
      <button type="submit" class="btn btn-primary" :disabled="hasErrors">
        {{ mode === 'create' ? 'Crear' : 'Actualizar' }}
      </button>
    </div>
  </form>
</template>

<script setup>
import { ref, watch, computed } from 'vue'
import { VALID_INSURANCE_COMPANIES } from '@/utils/constants'

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

const errors = ref({
  email: '',
  phonenumber: '',
  birthdate: '',
  address: '',
  emergencyPhone: '',
  insuranceCompanyName: '',
  insurancePolicyNumber: '',
  insuranceExpirationDate: ''
})

const validInsuranceCompanies = VALID_INSURANCE_COMPANIES

const hasErrors = computed(() => {
  return Object.values(errors.value).some(error => error !== '')
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

    // Limpiar errores al cargar datos
    Object.keys(errors.value).forEach(key => {
      errors.value[key] = ''
    })
  }
}, { immediate: true })

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
  } else if (phone.length !== 10) {
    errors.value.phonenumber = 'El teléfono debe tener exactamente 10 dígitos'
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

const validateEmergencyPhone = () => {
  const phone = formData.value.emergencyPhone
  if (!phone) {
    errors.value.emergencyPhone = ''
    return
  }
  if (!/^[0-9]+$/.test(phone)) {
    errors.value.emergencyPhone = 'El teléfono solo puede contener números'
  } else if (phone.length !== 10) {
    errors.value.emergencyPhone = 'El teléfono debe tener exactamente 10 dígitos'
  } else {
    errors.value.emergencyPhone = ''
  }
}

const validateInsuranceCompany = () => {
  const company = formData.value.insuranceCompanyName
  if (!company) {
    errors.value.insuranceCompanyName = ''
    return
  }
  const trimmedCompany = company.trim()
  const isValid = validInsuranceCompanies.some(c =>
    c.toLowerCase() === trimmedCompany.toLowerCase()
  )
  if (!isValid) {
    errors.value.insuranceCompanyName = `La compañía de seguros no está registrada. Seleccione una del listado.`
  } else {
    errors.value.insuranceCompanyName = ''
    // Actualizar el valor con el nombre exacto del catálogo (case-sensitive)
    const exactMatch = validInsuranceCompanies.find(c =>
      c.toLowerCase() === trimmedCompany.toLowerCase()
    )
    if (exactMatch) {
      formData.value.insuranceCompanyName = exactMatch
    }
  }
}

const validatePolicyNumber = () => {
  const policy = formData.value.insurancePolicyNumber
  if (!policy) {
    errors.value.insurancePolicyNumber = ''
    return
  }
  if (!/^[a-zA-Z0-9\-]+$/.test(policy)) {
    errors.value.insurancePolicyNumber = 'El número de póliza solo puede contener letras, números y guiones'
  } else {
    errors.value.insurancePolicyNumber = ''
  }
}

const validateInsuranceExpirationDate = () => {
  const expirationDate = formData.value.insuranceExpirationDate
  if (!expirationDate) {
    errors.value.insuranceExpirationDate = ''
    return
  }
  const expDate = new Date(expirationDate)
  const today = new Date()
  if (expDate > today) {
    errors.value.insuranceExpirationDate = 'La fecha de expiración no puede ser en el futuro'
  } else {
    errors.value.insuranceExpirationDate = ''
  }
}

const handleSubmit = () => {
  // Validar todos los campos antes de enviar
  validateEmail()
  validatePhone()
  validateBirthdate()
  validateAddress()
  validateEmergencyPhone()
  validateInsuranceCompany()
  validatePolicyNumber()
  validateInsuranceExpirationDate()

  // Si hay errores, no enviar
  if (hasErrors.value) {
    return
  }

  const data = { ...formData.value }
  // En modo edición, no enviar DNI ya que es la clave primaria
  if (props.mode === 'edit') {
    delete data.dni
  }
  emit('submit', data)
}
</script>
