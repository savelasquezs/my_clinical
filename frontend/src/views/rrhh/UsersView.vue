<template>
  <div>
    <PageHeader title="Usuarios">
      <template #actions>
        <button @click="openCreateModal" class="btn btn-primary">
          Crear Usuario
        </button>
      </template>
    </PageHeader>

    <div class="card">
      <CommonTable
        :columns="columns"
        :data="rrhhStore.users"
        :loading="rrhhStore.loading"
        @edit="handleEdit"
        @delete="handleDelete"
      />
    </div>

    <CommonModal
      :is-open="isModalOpen"
      :title="modalTitle"
      size="lg"
      @close="closeModal"
    >
      <UserForm
        :mode="formMode"
        :initial-data="selectedUser"
        @submit="handleSubmit"
        @cancel="closeModal"
      />
    </CommonModal>

    <DeleteModal
      :is-open="isDeleteModalOpen"
      :item-name="selectedUser?.fullname"
      item-type="usuario"
      @confirm="confirmDelete"
      @cancel="closeDeleteModal"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRRHHStore } from '@/stores/rrhh'
import { rrhhService } from '@/services/rrhhService'
import { useToast } from '@/composables/useToast'
import PageHeader from '@/components/shared/PageHeader.vue'
import CommonTable from '@/components/shared/CommonTable.vue'
import CommonModal from '@/components/shared/CommonModal.vue'
import DeleteModal from '@/components/shared/DeleteModal.vue'
import UserForm from '@/components/forms/UserForm.vue'

const rrhhStore = useRRHHStore()
const toast = useToast()

const isModalOpen = ref(false)
const isDeleteModalOpen = ref(false)
const formMode = ref('create')
const selectedUser = ref(null)

const modalTitle = computed(() => 
  formMode.value === 'create' ? 'Crear Usuario' : 'Editar Usuario'
)

const columns = [
  { key: 'dni', label: 'DNI' },
  { key: 'fullname', label: 'Nombre Completo' },
  { key: 'email', label: 'Email' },
  { key: 'username', label: 'Usuario' },
  { key: 'role', label: 'Rol' }
]

const openCreateModal = () => {
  formMode.value = 'create'
  selectedUser.value = null
  isModalOpen.value = true
}

const handleEdit = (user) => {
  formMode.value = 'edit'
  selectedUser.value = user
  isModalOpen.value = true
}

const handleDelete = (user) => {
  selectedUser.value = user
  isDeleteModalOpen.value = true
}

const handleSubmit = async (data) => {
  try {
    if (formMode.value === 'create') {
      await rrhhService.createUser(data)
      toast.success('Usuario creado exitosamente')
      await loadUsers()
    } else {
      await rrhhService.updateUser(selectedUser.value.dni, data)
      toast.success('Usuario actualizado exitosamente')
      await loadUsers()
    }
    closeModal()
  } catch (error) {
    // Error handled by interceptor
  }
}

const confirmDelete = async () => {
  try {
    await rrhhService.deleteUser(selectedUser.value.dni)
    toast.success('Usuario eliminado exitosamente')
    await loadUsers()
    closeDeleteModal()
  } catch (error) {
    // Error handled by interceptor
  }
}

const closeModal = () => {
  isModalOpen.value = false
  selectedUser.value = null
}

const closeDeleteModal = () => {
  isDeleteModalOpen.value = false
  selectedUser.value = null
}

const loadUsers = async () => {
  rrhhStore.setLoading(true)
  try {
    const users = await rrhhService.getAllUsers()
    rrhhStore.setUsers(users)
  } catch (error) {
    // Error handled by interceptor
  } finally {
    rrhhStore.setLoading(false)
  }
}

onMounted(() => {
  loadUsers()
})
</script>

