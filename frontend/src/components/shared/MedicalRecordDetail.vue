<template>
  <div v-if="medicalRecord" class="space-y-6">
    <div class="border-b pb-4">
      <h3 class="text-xl font-semibold">Detalle del Registro Médico</h3>
    </div>

    <div class="grid grid-cols-2 gap-4">
      <div>
        <label class="label text-gray-600">ID</label>
        <p class="text-gray-900">{{ medicalRecord.id }}</p>
      </div>
      <div>
        <label class="label text-gray-600">Fecha</label>
        <p class="text-gray-900">{{ formatDate(medicalRecord.date) }}</p>
      </div>
      <div>
        <label class="label text-gray-600">Paciente</label>
        <p class="text-gray-900">{{ medicalRecord.patientName || medicalRecord.patientDni }}</p>
      </div>
      <div>
        <label class="label text-gray-600">DNI Paciente</label>
        <p class="text-gray-900">{{ medicalRecord.patientDni }}</p>
      </div>
      <div>
        <label class="label text-gray-600">Doctor</label>
        <p class="text-gray-900">{{ medicalRecord.doctorName || medicalRecord.doctorDni }}</p>
      </div>
      <div v-if="medicalRecord.orderNumber">
        <label class="label text-gray-600">Orden Asociada</label>
        <button 
          @click="$emit('view-order', medicalRecord.orderNumber)"
          class="text-emerald-600 hover:text-emerald-700 underline"
        >
          Orden #{{ medicalRecord.orderNumber }}
        </button>
      </div>
    </div>

    <div>
      <label class="label text-gray-600">Razón de Consulta</label>
      <p class="text-gray-900 whitespace-pre-wrap">{{ medicalRecord.consultationReason }}</p>
    </div>

    <div>
      <label class="label text-gray-600">Síntomas</label>
      <p class="text-gray-900 whitespace-pre-wrap">{{ medicalRecord.symptoms }}</p>
    </div>

    <div>
      <label class="label text-gray-600">Diagnóstico</label>
      <p class="text-gray-900 whitespace-pre-wrap">{{ medicalRecord.diagnosis }}</p>
    </div>

    <div class="flex justify-end pt-4 border-t">
      <button @click="$emit('close')" class="btn btn-secondary">
        Cerrar
      </button>
    </div>
  </div>
</template>

<script setup>
import { useDate } from '@/composables/useDate'

const props = defineProps({
  medicalRecord: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['close', 'view-order'])

const { formatDate } = useDate()
</script>

