<template>
  <div>
    <PageHeader title="Inventario" />
    
    <div class="mb-4">
      <div class="border-b border-gray-200">
        <nav class="-mb-px flex space-x-8">
          <button
            v-for="tab in tabs"
            :key="tab.id"
            @click="activeTab = tab.id"
            :class="[
              'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm',
              activeTab === tab.id
                ? 'border-emerald-500 text-emerald-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            ]"
          >
            {{ tab.label }}
          </button>
        </nav>
      </div>
    </div>

    <div class="card">
      <div class="mb-4 flex justify-end">
        <button @click="openCreateModal" class="btn btn-primary">
          Crear {{ getCurrentTabLabel() }}
        </button>
      </div>

      <CommonTable
        v-if="activeTab === 'medications'"
        :columns="medicationColumns"
        :data="supportStore.medications"
        :loading="supportStore.loading"
        @edit="(item) => handleEdit('medication', item)"
        @delete="(item) => handleDelete('medication', item)"
      />

      <CommonTable
        v-if="activeTab === 'procedures'"
        :columns="procedureColumns"
        :data="supportStore.procedures"
        :loading="supportStore.loading"
        @edit="(item) => handleEdit('procedure', item)"
        @delete="(item) => handleDelete('procedure', item)"
      />

      <CommonTable
        v-if="activeTab === 'diagnosticAids'"
        :columns="diagnosticAidColumns"
        :data="supportStore.diagnosticAids"
        :loading="supportStore.loading"
        @edit="(item) => handleEdit('diagnosticAid', item)"
        @delete="(item) => handleDelete('diagnosticAid', item)"
      />
    </div>

    <CommonModal
      :is-open="isModalOpen"
      :title="modalTitle"
      size="lg"
      @close="closeModal"
    >
      <InventoryForm
        :type="activeTab"
        :mode="formMode"
        :initial-data="selectedItem"
        @submit="handleSubmit"
        @cancel="closeModal"
      />
    </CommonModal>

    <DeleteModal
      :is-open="isDeleteModalOpen"
      :item-type="deleteItemType"
      :item-name="itemToDelete?.name"
      @confirm="confirmDelete"
      @cancel="cancelDelete"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useSupportStore } from '@/stores/support'
import { supportService } from '@/services/supportService'
import { useToast } from '@/composables/useToast'
import { useMoney } from '@/composables/useMoney'
import PageHeader from '@/components/shared/PageHeader.vue'
import CommonTable from '@/components/shared/CommonTable.vue'
import CommonModal from '@/components/shared/CommonModal.vue'
import DeleteModal from '@/components/shared/DeleteModal.vue'
import InventoryForm from '@/components/forms/InventoryForm.vue'

const supportStore = useSupportStore()
const toast = useToast()
const { formatMoney } = useMoney()

const activeTab = ref('medications')
const isModalOpen = ref(false)
const isDeleteModalOpen = ref(false)
const formMode = ref('create')
const selectedItem = ref(null)
const itemToDelete = ref(null)
const deleteItemType = ref('')

const tabs = [
  { id: 'medications', label: 'Medicamentos' },
  { id: 'procedures', label: 'Procedimientos' },
  { id: 'diagnosticAids', label: 'Ayudas Diagnósticas' }
]

const medicationColumns = [
  { key: 'id', label: 'ID' },
  { key: 'name', label: 'Nombre' },
  { 
    key: 'cost', 
    label: 'Costo',
    formatter: (value) => formatMoney(value)
  }
]

const procedureColumns = [
  { key: 'id', label: 'ID' },
  { key: 'name', label: 'Nombre' },
  { 
    key: 'cost', 
    label: 'Costo',
    formatter: (value) => formatMoney(value)
  }
]

const diagnosticAidColumns = [
  { key: 'id', label: 'ID' },
  { key: 'name', label: 'Nombre' },
  { 
    key: 'cost', 
    label: 'Costo',
    formatter: (value) => formatMoney(value)
  }
]

const modalTitle = computed(() => {
  const tabLabel = getCurrentTabLabel()
  return formMode.value === 'create' 
    ? `Crear ${tabLabel}` 
    : `Editar ${tabLabel}`
})

const getCurrentTabLabel = () => {
  const tab = tabs.find(t => t.id === activeTab.value)
  return tab?.label || ''
}

const openCreateModal = () => {
  formMode.value = 'create'
  selectedItem.value = null
  isModalOpen.value = true
}

const handleEdit = (type, item) => {
  formMode.value = 'edit'
  selectedItem.value = item
  isModalOpen.value = true
}

const handleSubmit = async (data) => {
  try {
    if (activeTab.value === 'medications') {
      if (formMode.value === 'create') {
        await supportService.createMedication(data)
        toast.success('Medicamento creado exitosamente')
      } else {
        await supportService.updateMedication(data)
        toast.success('Medicamento actualizado exitosamente')
      }
      await loadMedications()
    } else if (activeTab.value === 'procedures') {
      if (formMode.value === 'create') {
        await supportService.createProcedure(data)
        toast.success('Procedimiento creado exitosamente')
      } else {
        await supportService.updateProcedure(data)
        toast.success('Procedimiento actualizado exitosamente')
      }
      await loadProcedures()
    } else if (activeTab.value === 'diagnosticAids') {
      if (formMode.value === 'create') {
        await supportService.createDiagnosticAid(data)
        toast.success('Ayuda diagnóstica creada exitosamente')
      } else {
        await supportService.updateDiagnosticAid(data)
        toast.success('Ayuda diagnóstica actualizada exitosamente')
      }
      await loadDiagnosticAids()
    }
    closeModal()
  } catch (error) {
    // Error handled by interceptor
  }
}

const closeModal = () => {
  isModalOpen.value = false
  selectedItem.value = null
}

const handleDelete = (type, item) => {
  itemToDelete.value = item
  deleteItemType.value = getItemTypeLabel(type)
  isDeleteModalOpen.value = true
}

const getItemTypeLabel = (type) => {
  const labels = {
    medication: 'medicamento',
    procedure: 'procedimiento',
    diagnosticAid: 'ayuda diagnóstica'
  }
  return labels[type] || 'elemento'
}

const confirmDelete = async () => {
  if (!itemToDelete.value) return

  try {
    if (activeTab.value === 'medications') {
      await supportService.deleteMedication(itemToDelete.value.id)
      toast.success('Medicamento eliminado exitosamente')
      await loadMedications()
    } else if (activeTab.value === 'procedures') {
      await supportService.deleteProcedure(itemToDelete.value.id)
      toast.success('Procedimiento eliminado exitosamente')
      await loadProcedures()
    } else if (activeTab.value === 'diagnosticAids') {
      await supportService.deleteDiagnosticAid(itemToDelete.value.id)
      toast.success('Ayuda diagnóstica eliminada exitosamente')
      await loadDiagnosticAids()
    }
    cancelDelete()
  } catch (error) {
    // Error handled by interceptor
  }
}

const cancelDelete = () => {
  isDeleteModalOpen.value = false
  itemToDelete.value = null
  deleteItemType.value = ''
}

const loadMedications = async () => {
  supportStore.setLoading(true)
  try {
    const medications = await supportService.getAllMedications()
    supportStore.setMedications(medications)
  } catch (error) {
    // Error handled by interceptor
  } finally {
    supportStore.setLoading(false)
  }
}

const loadProcedures = async () => {
  supportStore.setLoading(true)
  try {
    const procedures = await supportService.getAllProcedures()
    supportStore.setProcedures(procedures)
  } catch (error) {
    // Error handled by interceptor
  } finally {
    supportStore.setLoading(false)
  }
}

const loadDiagnosticAids = async () => {
  supportStore.setLoading(true)
  try {
    const diagnosticAids = await supportService.getAllDiagnosticAids()
    supportStore.setDiagnosticAids(diagnosticAids)
  } catch (error) {
    // Error handled by interceptor
  } finally {
    supportStore.setLoading(false)
  }
}

onMounted(() => {
  loadMedications()
  loadProcedures()
  loadDiagnosticAids()
})
</script>

