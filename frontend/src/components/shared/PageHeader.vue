<template>
  <div class="mb-6 flex items-center justify-between">
    <div>
      <h1 class="text-2xl font-bold text-gray-900">{{ title }}</h1>
      <p v-if="subtitle" class="mt-1 text-sm text-gray-500">{{ subtitle }}</p>
    </div>
    <div v-if="$slots.actions" class="flex items-center gap-2">
      <slot name="actions"></slot>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    required: true
  },
  subtitle: {
    type: String,
    default: ''
  }
})
</script>

