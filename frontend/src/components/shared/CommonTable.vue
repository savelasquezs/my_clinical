<template>
  <div class="overflow-x-auto">
    <table class="min-w-full divide-y divide-gray-200">
      <thead class="bg-gray-50">
        <tr>
          <th
            v-for="column in columns"
            :key="column.key"
            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
            :class="column.class"
          >
            {{ column.label }}
          </th>
          <th v-if="hasActions" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
            Acciones
          </th>
        </tr>
      </thead>
      <tbody class="bg-white divide-y divide-gray-200">
        <tr v-if="loading">
          <td :colspan="columns.length + (hasActions ? 1 : 0)" class="px-6 py-12 text-center">
            <LoadingSpinner />
          </td>
        </tr>
        <tr v-else-if="!data || data.length === 0">
          <td :colspan="columns.length + (hasActions ? 1 : 0)" class="px-6 py-12">
            <EmptyState :title="emptyTitle" :message="emptyMessage" />
          </td>
        </tr>
        <tr v-else v-for="(row, index) in data" :key="index" class="hover:bg-gray-50">
          <td
            v-for="column in columns"
            :key="column.key"
            class="px-6 py-4 whitespace-nowrap text-sm text-gray-900"
            :class="column.class"
          >
            <slot :name="`cell-${column.key}`" :row="row" :value="row[column.key]">
              {{ formatCellValue(row[column.key], column) }}
            </slot>
          </td>
          <td v-if="hasActions" class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
            <slot name="actions" :row="row" :index="index">
              <button
                v-if="showEdit"
                @click="$emit('edit', row)"
                class="text-emerald-600 hover:text-emerald-900 mr-3"
              >
                Editar
              </button>
              <button
                v-if="showDelete"
                @click="$emit('delete', row)"
                class="text-red-600 hover:text-red-900"
              >
                Eliminar
              </button>
            </slot>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import LoadingSpinner from './LoadingSpinner.vue'
import EmptyState from './EmptyState.vue'

const props = defineProps({
  columns: {
    type: Array,
    required: true
  },
  data: {
    type: Array,
    default: () => []
  },
  loading: {
    type: Boolean,
    default: false
  },
  showEdit: {
    type: Boolean,
    default: true
  },
  showDelete: {
    type: Boolean,
    default: true
  },
  emptyTitle: {
    type: String,
    default: 'No hay datos'
  },
  emptyMessage: {
    type: String,
    default: 'No se encontraron registros'
  }
})

defineEmits(['edit', 'delete'])

const hasActions = computed(() => props.showEdit || props.showDelete)

const formatCellValue = (value, column) => {
  if (value === null || value === undefined) return '-'
  if (column.formatter && typeof column.formatter === 'function') {
    return column.formatter(value)
  }
  return value
}
</script>

