<template>
  <div class="flex items-center justify-center" :class="containerClass">
    <div
      class="animate-spin rounded-full border-t-2 border-b-2"
      :class="spinnerClass"
    ></div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  size: {
    type: String,
    default: 'md',
    validator: (value) => ['sm', 'md', 'lg'].includes(value)
  },
  color: {
    type: String,
    default: 'emerald'
  }
})

const containerClass = 'p-4'
const spinnerClass = computed(() => {
  const sizes = {
    sm: 'h-4 w-4',
    md: 'h-8 w-8',
    lg: 'h-12 w-12'
  }
  const colors = {
    emerald: 'border-emerald-600',
    white: 'border-white',
    gray: 'border-gray-600'
  }
  return `${sizes[props.size]} ${colors[props.color] || colors.emerald}`
})
</script>

