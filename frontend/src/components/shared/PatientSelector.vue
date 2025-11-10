<template>
  <div class="relative">
    <label class="label">{{ label }}</label>
    <div class="relative">
      <input 
        v-model="searchQuery"
        @input="handleSearch"
        @focus="handleFocus"
        @blur="handleBlur"
        @keydown="handleKeydown"
        :placeholder="placeholder"
        :required="required"
        :disabled="disabled"
        class="input"
        :class="{ 'border-red-500': hasError }"
      />
      <div v-if="isLoading" class="absolute right-3 top-1/2 transform -translate-y-1/2">
        <LoadingSpinner size="sm" />
      </div>
    </div>
    
    <!-- Dropdown de resultados -->
    <div 
      v-if="showDropdown && searchResults.length > 0" 
      class="absolute z-50 w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg max-h-60 overflow-auto"
    >
      <div 
        v-for="(patient, index) in searchResults" 
        :key="patient.dni"
        @click="selectPatient(patient)"
        @mouseenter="highlightedIndex = index"
        class="px-4 py-2 cursor-pointer hover:bg-emerald-50 transition-colors"
        :class="{ 'bg-emerald-100': highlightedIndex === index }"
      >
        <div class="font-medium text-gray-900">{{ patient.fullname }}</div>
        <div class="text-sm text-gray-500">DNI: {{ patient.dni }}</div>
        <div v-if="patient.email" class="text-xs text-gray-400">{{ patient.email }}</div>
      </div>
    </div>
    
    <!-- Mensaje cuando no hay resultados -->
    <div 
      v-if="showDropdown && searchQuery && searchResults.length === 0 && !isLoading" 
      class="absolute z-50 w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg p-4 text-center text-gray-500"
    >
      No se encontraron pacientes
    </div>
    
    <!-- Información del paciente seleccionado -->
    <div v-if="selectedPatient && !showDropdown" class="mt-2 p-3 bg-emerald-50 border border-emerald-200 rounded-md">
      <div class="flex items-center justify-between">
        <div>
          <div class="font-medium text-emerald-900">{{ selectedPatient.fullname }}</div>
          <div class="text-sm text-emerald-700">DNI: {{ selectedPatient.dni }}</div>
        </div>
        <button 
          @click="clearSelection"
          type="button"
          class="text-emerald-600 hover:text-emerald-800 text-sm"
        >
          Cambiar
        </button>
      </div>
    </div>
    
    <!-- Mensaje de error -->
    <div v-if="hasError" class="mt-1 text-sm text-red-600">
      {{ errorMessage }}
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useAdminStore } from '@/stores/admin'
import LoadingSpinner from './LoadingSpinner.vue'

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  },
  label: {
    type: String,
    default: 'Paciente'
  },
  placeholder: {
    type: String,
    default: 'Buscar por DNI o nombre...'
  },
  required: {
    type: Boolean,
    default: false
  },
  disabled: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['update:modelValue', 'patient-selected'])

const adminStore = useAdminStore()
const searchQuery = ref('')
const showDropdown = ref(false)
const highlightedIndex = ref(-1)
const selectedPatient = ref(null)
const isLoading = ref(false)
const hasError = ref(false)
const errorMessage = ref('')
let searchTimeout = null
let blurTimeout = null

// Cargar pacientes al montar el componente
onMounted(async () => {
  try {
    isLoading.value = true
    await adminStore.loadPatientsIfNeeded()
  } catch (error) {
    hasError.value = true
    errorMessage.value = 'Error al cargar pacientes'
  } finally {
    isLoading.value = false
  }
  
  // Si hay un valor inicial, buscar el paciente
  if (props.modelValue) {
    const patient = adminStore.patients.find(p => p.dni === props.modelValue)
    if (patient) {
      selectedPatient.value = patient
      searchQuery.value = `${patient.fullname} (${patient.dni})`
    }
  }
})

// Búsqueda local en los pacientes del store
const searchResults = computed(() => {
  if (!searchQuery.value || searchQuery.value.length < 2) {
    return []
  }
  
  const query = searchQuery.value.toLowerCase().trim()
  const allPatients = adminStore.patients || []
  
  // Filtrar pacientes
  const results = allPatients.filter(patient => {
    const dni = patient.dni?.toLowerCase() || ''
    const fullname = patient.fullname?.toLowerCase() || ''
    
    // Buscar por DNI exacto o parcial
    if (dni.includes(query)) {
      return true
    }
    
    // Buscar por nombre completo
    if (fullname.includes(query)) {
      return true
    }
    
    return false
  })
  
  // Ordenar: primero coincidencias exactas de DNI, luego por nombre
  return results
    .sort((a, b) => {
      const aDni = a.dni?.toLowerCase() || ''
      const bDni = b.dni?.toLowerCase() || ''
      const queryLower = query.toLowerCase()
      
      // Priorizar coincidencias exactas de DNI
      if (aDni === queryLower && bDni !== queryLower) return -1
      if (bDni === queryLower && aDni !== queryLower) return 1
      
      // Luego ordenar alfabéticamente por nombre
      return (a.fullname || '').localeCompare(b.fullname || '')
    })
    .slice(0, 10) // Limitar a 10 resultados
})

const handleSearch = () => {
  // Limpiar selección si el usuario está escribiendo
  if (selectedPatient.value) {
    selectedPatient.value = null
    emit('update:modelValue', '')
    emit('patient-selected', null)
  }
  
  // Debounce de búsqueda
  if (searchTimeout) {
    clearTimeout(searchTimeout)
  }
  
  searchTimeout = setTimeout(() => {
    if (searchQuery.value.length >= 2) {
      showDropdown.value = true
      highlightedIndex.value = -1
    } else {
      showDropdown.value = false
    }
  }, 300)
}

const handleFocus = () => {
  if (blurTimeout) {
    clearTimeout(blurTimeout)
  }
  if (searchQuery.value.length >= 2) {
    showDropdown.value = true
  }
}

const handleBlur = () => {
  // Delay para permitir que el click en el dropdown funcione
  blurTimeout = setTimeout(() => {
    showDropdown.value = false
    highlightedIndex.value = -1
  }, 200)
}

const handleKeydown = (event) => {
  if (!showDropdown.value || searchResults.value.length === 0) {
    return
  }
  
  switch (event.key) {
    case 'ArrowDown':
      event.preventDefault()
      highlightedIndex.value = Math.min(
        highlightedIndex.value + 1,
        searchResults.value.length - 1
      )
      break
    case 'ArrowUp':
      event.preventDefault()
      highlightedIndex.value = Math.max(highlightedIndex.value - 1, -1)
      break
    case 'Enter':
      event.preventDefault()
      if (highlightedIndex.value >= 0 && highlightedIndex.value < searchResults.value.length) {
        selectPatient(searchResults.value[highlightedIndex.value])
      }
      break
    case 'Escape':
      showDropdown.value = false
      highlightedIndex.value = -1
      break
  }
}

const selectPatient = (patient) => {
  selectedPatient.value = patient
  searchQuery.value = `${patient.fullname} (${patient.dni})`
  showDropdown.value = false
  highlightedIndex.value = -1
  hasError.value = false
  errorMessage.value = ''
  
  emit('update:modelValue', patient.dni)
  emit('patient-selected', patient)
}

const clearSelection = () => {
  selectedPatient.value = null
  searchQuery.value = ''
  emit('update:modelValue', '')
  emit('patient-selected', null)
  showDropdown.value = false
}

// Watch para sincronizar con cambios externos al modelValue
watch(() => props.modelValue, (newValue) => {
  if (!newValue && selectedPatient.value) {
    clearSelection()
  } else if (newValue && (!selectedPatient.value || selectedPatient.value.dni !== newValue)) {
    const patient = adminStore.patients.find(p => p.dni === newValue)
    if (patient) {
      selectedPatient.value = patient
      searchQuery.value = `${patient.fullname} (${patient.dni})`
    }
  }
})
</script>

