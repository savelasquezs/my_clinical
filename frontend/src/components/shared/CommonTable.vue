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
          <th v-if="hasActionsColumn" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
            Acciones
          </th>
        </tr>
      </thead>
      <tbody class="bg-white divide-y divide-gray-200">
        <tr v-if="loading">
          <td :colspan="columns.length + (hasActionsColumn ? 1 : 0)" class="px-6 py-12 text-center">
            <LoadingSpinner />
          </td>
        </tr>
        <tr v-else-if="!data || data.length === 0">
          <td :colspan="columns.length + (hasActionsColumn ? 1 : 0)" class="px-6 py-12">
            <EmptyState :title="emptyTitle" :message="emptyMessage" />
          </td>
        </tr>
        <tr 
          v-else 
          v-for="(row, index) in data" 
          :key="getRowKey(row, index)" 
          class="hover:bg-gray-50"
          :class="{ 'cursor-pointer': clickable }"
          @click="clickable ? $emit('row-click', row) : null"
        >
          <td
            v-for="column in columns"
            :key="column.key"
            class="px-6 py-4 whitespace-nowrap text-sm text-gray-900"
            :class="column.class"
          >
            <slot :name="`cell-${column.key}`" :row="row" :value="row[column.key]">
              {{ formatCellValue(row[column.key], column, row) }}
            </slot>
          </td>
          <td v-if="hasActionsColumn" class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
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
  },
  clickable: {
    type: Boolean,
    default: false
  },
  hasActions: {
    type: Boolean,
    default: undefined // undefined = auto-detect from showEdit/showDelete
  }
})

const slots = defineSlots()

defineEmits(['edit', 'delete', 'row-click'])

const hasActionsColumn = computed(() => {
  if (props.hasActions !== undefined) {
    return props.hasActions
  }
  // Auto-detect: si hay slot de acciones o showEdit/showDelete
  return props.showEdit || props.showDelete || !!slots.actions
})

const formatCellValue = (value, column, row) => {
  if (value === null || value === undefined) return '-'
  if (column.formatter && typeof column.formatter === 'function') {
    return column.formatter(value, row)
  }
  return value
}

const getRowKey = (row, index) => {
  // Intentar usar un ID único si existe
  if (row.id !== undefined) return row.id
  if (row.dni !== undefined) return row.dni
  if (row.orderNumber !== undefined) return `order-${row.orderNumber}`
  if (row.invoiceNumber !== undefined) return `invoice-${row.invoiceNumber}`
  // Fallback al índice
  return index
}
</script>

