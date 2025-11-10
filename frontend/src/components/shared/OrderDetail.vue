<template>
  <div class="space-y-6">
    <div class="border-b pb-4">
      <h3 class="text-xl font-semibold">Detalle de la Orden</h3>
    </div>

    <div class="grid grid-cols-2 gap-4">
      <div>
        <label class="label text-gray-600">Número de Orden</label>
        <p class="text-gray-900">{{ order.orderNumber }}</p>
      </div>
      <div>
        <label class="label text-gray-600">Fecha de Creación</label>
        <p class="text-gray-900">{{ formatDate(order.creationDate) }}</p>
      </div>
    </div>

    <div>
      <div class="flex justify-between items-center mb-4">
        <label class="label text-gray-600">Items de la Orden</label>
        <button @click="openAddItemModal" class="btn btn-sm btn-primary">
          Agregar Item
        </button>
      </div>

      <div v-if="!order.items || order.items.length === 0" class="text-sm text-gray-500 p-4 bg-gray-50 rounded border">
        No hay items en esta orden.
      </div>

      <div v-else-if="order.items && order.items.length > 0" class="space-y-3">
        <div 
          v-for="item in order.items" 
          :key="`${item.orderNumber}-${item.itemNumber}`"
          class="p-4 bg-gray-50 rounded-lg border"
        >
          <div class="flex justify-between items-start">
            <div class="flex-1">
              <div class="flex items-center gap-2 mb-2">
                <span class="px-2 py-1 bg-emerald-100 text-emerald-800 rounded text-xs font-medium">
                  {{ getItemTypeLabel(item.itemType) }}
                </span>
                <span class="text-sm text-gray-600">Item #{{ item.itemNumber }}</span>
              </div>
              
              <div class="space-y-1 text-sm">
                <p class="font-medium text-gray-900">
                  {{ getItemName(item) }}
                </p>
                <p class="text-gray-600">Costo: ${{ item.cost }}</p>
                
                <template v-if="item.itemType === 'Medication'">
                  <p v-if="item.dose" class="text-gray-600">Dosis: {{ item.dose }}</p>
                  <p v-if="item.treatmentDuration" class="text-gray-600">
                    Duración: {{ item.treatmentDuration }} días
                  </p>
                </template>
                
                <template v-else-if="item.itemType === 'Procedure'">
                  <p v-if="item.frequency" class="text-gray-600">Frecuencia: {{ item.frequency }}</p>
                  <p v-if="item.requiresSpecialist" class="text-gray-600">
                    Requiere especialista
                    <span v-if="item.specialistTypeId"> (Tipo: {{ item.specialistTypeId }})</span>
                  </p>
                </template>
                
                <template v-else-if="item.itemType === 'DiagnosticAid'">
                  <p v-if="item.quantity" class="text-gray-600">Cantidad: {{ item.quantity }}</p>
                  <p v-if="item.requiresSpecialist" class="text-gray-600">
                    Requiere especialista
                    <span v-if="item.specialistTypeId"> (Tipo: {{ item.specialistTypeId }})</span>
                  </p>
                </template>
              </div>
            </div>
            
            <div class="flex gap-2 ml-4">
              <button 
                @click="openEditItemModal(item)"
                class="btn btn-sm btn-secondary"
              >
                Editar
              </button>
              <button 
                @click="handleDeleteItem(item)"
                class="btn btn-sm btn-danger"
              >
                Eliminar
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="flex justify-end pt-4 border-t">
      <button @click="$emit('close')" class="btn btn-secondary">
        Cerrar
      </button>
    </div>

    <!-- Modal para agregar/editar item -->
    <CommonModal
      v-if="isItemModalOpen"
      :is-open="isItemModalOpen"
      :title="editingItem ? 'Editar Item' : 'Agregar Item a la Orden'"
      size="lg"
      @close="closeItemModal"
    >
      <OrderItemForm
        v-if="isItemModalOpen"
        :key="editingItem ? `edit-${editingItem.itemNumber}` : 'create'"
        :mode="editingItem ? 'edit' : 'create'"
        :order-number="order.orderNumber"
        :initial-data="editingItem"
        @submit="handleItemSubmit"
        @cancel="closeItemModal"
      />
    </CommonModal>

    <!-- Modal de confirmación para eliminar -->
    <DeleteModal
      :is-open="isDeleteModalOpen"
      item-type="item"
      :item-name="`el item #${itemToDelete?.itemNumber}`"
      @confirm="confirmDeleteItem"
      @cancel="closeDeleteModal"
    />
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import { useDate } from '@/composables/useDate'
import { useToast } from '@/composables/useToast'
import { doctorService } from '@/services/doctorService'
import OrderItemForm from '@/components/forms/OrderItemForm.vue'
import CommonModal from '@/components/shared/CommonModal.vue'
import DeleteModal from '@/components/shared/DeleteModal.vue'

const props = defineProps({
  order: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['close', 'updated'])

const { formatDate } = useDate()
const toast = useToast()

// Debug: verificar que los datos lleguen
watch(() => props.order, (newOrder) => {
  if (newOrder) {
    console.log('OrderDetail - Order recibida:', newOrder)
    console.log('OrderDetail - Items:', newOrder.items)
  }
}, { immediate: true, deep: true })

const isItemModalOpen = ref(false)
const editingItem = ref(null)
const isDeleteModalOpen = ref(false)
const itemToDelete = ref(null)

const getItemTypeLabel = (type) => {
  const labels = {
    Medication: 'Medicamento',
    Procedure: 'Procedimiento',
    DiagnosticAid: 'Ayuda Diagnóstica'
  }
  return labels[type] || type
}

const getItemName = (item) => {
  if (item.itemType === 'Medication' && item.medicationName) {
    return item.medicationName
  }
  if (item.itemType === 'Procedure' && item.procedureName) {
    return item.procedureName
  }
  if (item.itemType === 'DiagnosticAid' && item.diagnosticAidName) {
    return item.diagnosticAidName
  }
  return 'Sin nombre'
}

const openAddItemModal = () => {
  editingItem.value = null
  isItemModalOpen.value = true
}

const openEditItemModal = (item) => {
  editingItem.value = {
    itemNumber: item.itemNumber,
    itemType: item.itemType,
    cost: item.cost,
    medicationId: item.medicationId,
    dose: item.dose,
    treatmentDuration: item.treatmentDuration,
    procedureId: item.procedureId,
    frequency: item.frequency,
    requiresSpecialist: item.requiresSpecialist,
    specialistTypeId: item.specialistTypeId,
    diagnosticAidId: item.diagnosticAidId,
    quantity: item.quantity
  }
  isItemModalOpen.value = true
}

const closeItemModal = () => {
  // Usar nextTick para asegurar que la transición se complete antes de limpiar
  isItemModalOpen.value = false
  setTimeout(() => {
    editingItem.value = null
  }, 300) // Esperar a que termine la transición del modal
}

const handleItemSubmit = async (itemData) => {
  try {
    if (editingItem.value && editingItem.value.itemNumber) {
      // Editar item existente
      await doctorService.updateOrderItem(
        props.order.orderNumber,
        editingItem.value.itemNumber,
        itemData
      )
      toast.success('Item actualizado exitosamente')
    } else {
      // Agregar nuevo item
      await doctorService.addOrderItem(props.order.orderNumber, itemData)
      toast.success('Item agregado exitosamente')
    }
    
    closeItemModal()
    // Recargar la orden
    emit('updated')
  } catch (error) {
    // Error handled by interceptor
  }
}

const handleDeleteItem = (item) => {
  itemToDelete.value = item
  isDeleteModalOpen.value = true
}

const closeDeleteModal = () => {
  isDeleteModalOpen.value = false
  itemToDelete.value = null
}

const confirmDeleteItem = async () => {
  try {
    await doctorService.deleteOrderItem(
      props.order.orderNumber,
      itemToDelete.value.itemNumber
    )
    toast.success('Item eliminado exitosamente')
    closeDeleteModal()
    // Recargar la orden
    emit('updated')
  } catch (error) {
    // Error handled by interceptor
  }
}
</script>

