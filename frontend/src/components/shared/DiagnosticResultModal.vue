<template>
  <CommonModal :is-open="isOpen" title="Resultado de Ayuda Diagnóstica" size="lg" @close="handleClose">
    <div v-if="loading" class="flex justify-center p-8">
      <LoadingSpinner />
    </div>
    <div v-else class="space-y-6">
      <div>
        <label class="label text-gray-600">Ayuda Diagnóstica</label>
        <p class="text-gray-900 font-medium">{{ diagnosticAid?.name || 'N/A' }}</p>
      </div>

      <div>
        <label class="label text-gray-600">Resultado</label>
        <div class="p-4 bg-yellow-50 border border-yellow-200 rounded-lg">
          <p class="text-gray-900 whitespace-pre-wrap">{{ result }}</p>
        </div>
        <p class="text-xs text-gray-500 mt-2">
          Este es un resultado por defecto. Se recomienda crear un nuevo registro médico con el diagnóstico actualizado.
        </p>
      </div>

      <div class="flex justify-end gap-3 pt-4 border-t">
        <button @click="handleClose" class="btn btn-secondary">
          Cerrar
        </button>
        <button @click="handleCreateRecord" class="btn btn-primary">
          Crear Nuevo Registro Médico
        </button>
      </div>
    </div>
  </CommonModal>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { useDiagnosticResultsStore } from '@/stores/diagnosticResults'
import CommonModal from '@/components/shared/CommonModal.vue'
import LoadingSpinner from '@/components/shared/LoadingSpinner.vue'

const props = defineProps({
  isOpen: {
    type: Boolean,
    default: false
  },
  diagnosticAid: {
    type: Object,
    default: null
  },
  orderNumber: {
    type: Number,
    default: null
  },
  patientDni: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['close', 'create-record'])

const diagnosticResultsStore = useDiagnosticResultsStore()
const loading = ref(false)

const result = computed(() => {
  if (!props.diagnosticAid?.name) {
    return 'No se ha especificado una ayuda diagnóstica.'
  }
  return diagnosticResultsStore.getDefaultResult(props.diagnosticAid.name)
})

const handleClose = () => {
  emit('close')
}

const handleCreateRecord = () => {
  emit('create-record', {
    patientDni: props.patientDni,
    orderNumber: props.orderNumber,
    diagnosticAid: props.diagnosticAid,
    result: result.value
  })
  handleClose()
}
</script>

