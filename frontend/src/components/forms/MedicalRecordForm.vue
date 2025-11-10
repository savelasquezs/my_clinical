<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <div class="grid grid-cols-2 gap-4">
      <div>
        <PatientSelector 
          v-model="formData.patientDni"
          label="Paciente *"
          :required="true"
        />
      </div>
      <div>
        <label class="label">Fecha *</label>
        <input v-model="formData.date" type="datetime-local" class="input" required />
      </div>
      <div class="col-span-2">
        <label class="label">Razón de Consulta *</label>
        <textarea v-model="formData.consultationReason" class="input" rows="3" required></textarea>
      </div>
      <div class="col-span-2">
        <label class="label">Síntomas *</label>
        <textarea v-model="formData.symptoms" class="input" rows="3" required></textarea>
      </div>
      <div class="col-span-2">
        <label class="label">Diagnóstico *</label>
        <textarea v-model="formData.diagnosis" class="input" rows="3" required></textarea>
      </div>
    </div>

    <!-- Sección opcional para crear orden -->
    <div class="border-t pt-4">
      <label class="flex items-center gap-2 mb-4">
        <input v-model="createOrder" type="checkbox" class="checkbox" />
        <span class="font-medium">Crear orden asociada</span>
      </label>

      <div v-if="createOrder" class="space-y-4 bg-gray-50 p-4 rounded-lg">
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="label">Número de Orden *</label>
            <input v-model.number="orderData.orderNumber" type="number" class="input" required />
          </div>
          <div>
            <label class="label">Fecha de Creación *</label>
            <input v-model="orderData.creationDate" type="datetime-local" class="input" required />
          </div>
        </div>

        <div>
          <div class="flex justify-between items-center mb-2">
            <label class="label">Items de la Orden *</label>
            <button type="button" @click="openAddItemModal" class="btn btn-sm btn-primary">
              Agregar Item
            </button>
          </div>
          
          <div v-if="orderData.items.length === 0" class="text-sm text-gray-500 p-4 bg-white rounded border">
            No hay items agregados. La orden debe tener al menos un item.
          </div>
          
          <div v-else class="space-y-2">
            <div 
              v-for="(item, index) in orderData.items" 
              :key="index"
              class="flex items-center justify-between p-3 bg-white rounded border"
            >
              <div class="flex-1">
                <span class="font-medium">{{ getItemTypeLabel(item.itemType) }}</span>
                <span class="text-gray-600 ml-2">
                  - ${{ item.cost }}
                  <span v-if="item.itemType === 'Medication' && item.medicationName">
                    ({{ item.medicationName }})
                  </span>
                  <span v-else-if="item.itemType === 'Procedure' && item.procedureName">
                    ({{ item.procedureName }})
                  </span>
                  <span v-else-if="item.itemType === 'DiagnosticAid' && item.diagnosticAidName">
                    ({{ item.diagnosticAidName }})
                  </span>
                </span>
              </div>
              <button 
                type="button" 
                @click="removeItem(index)"
                class="btn btn-sm btn-danger ml-2"
              >
                Eliminar
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="flex justify-end gap-3 pt-4 border-t">
      <button type="button" @click="$emit('cancel')" class="btn btn-secondary">
        Cancelar
      </button>
      <button type="submit" class="btn btn-primary" :disabled="createOrder && orderData.items.length === 0">
        Crear
      </button>
    </div>

    <!-- Modal para agregar item -->
    <CommonModal
      v-if="isItemModalOpen"
      :is-open="isItemModalOpen"
      title="Agregar Item a la Orden"
      size="lg"
      @close="closeItemModal"
    >
      <OrderItemForm
        v-if="isItemModalOpen && orderData.orderNumber"
        :key="`create-item-${orderData.orderNumber}`"
        :mode="'create'"
        :order-number="orderData.orderNumber"
        @submit="handleAddItem"
        @cancel="closeItemModal"
      />
    </CommonModal>
  </form>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useToast } from '@/composables/useToast'
import PatientSelector from '@/components/shared/PatientSelector.vue'
import OrderItemForm from '@/components/forms/OrderItemForm.vue'
import CommonModal from '@/components/shared/CommonModal.vue'

const emit = defineEmits(['submit', 'cancel'])
const toast = useToast()

const createOrder = ref(false)
const isItemModalOpen = ref(false)

const formData = ref({
  patientDni: '',
  date: '',
  consultationReason: '',
  symptoms: '',
  diagnosis: '',
  orderNumber: null
})

const orderData = ref({
  orderNumber: null,
  creationDate: '',
  items: []
})

const getItemTypeLabel = (type) => {
  const labels = {
    Medication: 'Medicamento',
    Procedure: 'Procedimiento',
    DiagnosticAid: 'Ayuda Diagnóstica'
  }
  return labels[type] || type
}

const openAddItemModal = () => {
  if (!orderData.value.orderNumber) {
    toast.error('Debe ingresar un número de orden primero')
    return
  }
  isItemModalOpen.value = true
}

const closeItemModal = () => {
  // Usar setTimeout para asegurar que la transición se complete
  isItemModalOpen.value = false
}

const handleAddItem = (itemData) => {
  orderData.value.items.push(itemData)
  closeItemModal()
}

const removeItem = (index) => {
  orderData.value.items.splice(index, 1)
}

const handleSubmit = () => {
  const data = {
    patientDni: formData.value.patientDni,
    date: formData.value.date,
    consultationReason: formData.value.consultationReason,
    symptoms: formData.value.symptoms,
    diagnosis: formData.value.diagnosis,
    orderNumber: null
  }

  // Si se crea orden, incluir los datos de la orden
  if (createOrder.value) {
    if (orderData.value.items.length === 0) {
      toast.error('La orden debe tener al menos un item')
      return
    }
    data.order = {
      orderNumber: orderData.value.orderNumber,
      creationDate: orderData.value.creationDate,
      items: orderData.value.items
    }
  }

  emit('submit', data)
}
</script>
