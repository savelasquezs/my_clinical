<template>
  <ConfirmationModal
    :is-open="isOpen"
    :title="title"
    :message="message"
    confirm-text="Eliminar"
    cancel-text="Cancelar"
    @confirm="$emit('confirm')"
    @cancel="$emit('cancel')"
  />
</template>

<script setup>
import { computed } from 'vue'
import ConfirmationModal from './ConfirmationModal.vue'

const props = defineProps({
  isOpen: {
    type: Boolean,
    default: false
  },
  itemName: {
    type: String,
    default: ''
  },
  itemType: {
    type: String,
    default: 'elemento'
  }
})

defineEmits(['confirm', 'cancel'])

const title = computed(() => `Eliminar ${props.itemType}`)
const message = computed(() => 
  `¿Estás seguro de que deseas eliminar ${props.itemName || `este ${props.itemType}`}? Esta acción no se puede deshacer.`
)
</script>

