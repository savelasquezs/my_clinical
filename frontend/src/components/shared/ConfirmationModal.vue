<template>
  <CommonModal :is-open="isOpen" :title="title" size="md" @close="$emit('cancel')">
    <p class="text-gray-600 mb-6">{{ message }}</p>
    
    <template #footer>
      <button
        @click="$emit('cancel')"
        class="btn btn-secondary"
      >
        {{ cancelText }}
      </button>
      <button
        @click="$emit('confirm')"
        class="btn btn-primary"
      >
        {{ confirmText }}
      </button>
    </template>
  </CommonModal>
</template>

<script setup>
import CommonModal from './CommonModal.vue'

defineProps({
  isOpen: {
    type: Boolean,
    default: false
  },
  title: {
    type: String,
    default: 'Confirmar acción'
  },
  message: {
    type: String,
    default: '¿Estás seguro?'
  },
  confirmText: {
    type: String,
    default: 'Confirmar'
  },
  cancelText: {
    type: String,
    default: 'Cancelar'
  }
})

defineEmits(['confirm', 'cancel'])
</script>

