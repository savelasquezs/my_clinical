<template>
  <div class="flex flex-col items-center justify-center py-12 px-4">
    <div v-if="icon" class="mb-4 text-gray-400">
      <component :is="icon" class="h-12 w-12" />
    </div>
    <h3 class="text-lg font-medium text-gray-900 mb-2">{{ title }}</h3>
    <p class="text-sm text-gray-500 text-center max-w-sm">{{ message }}</p>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    default: 'No hay datos'
  },
  message: {
    type: String,
    default: 'No se encontraron registros'
  },
  icon: {
    type: Object,
    default: null
  }
})
</script>

