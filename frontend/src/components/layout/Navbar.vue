<template>
  <nav class="bg-white border-b border-gray-200">
    <div class="px-6 py-4 flex items-center justify-between">
      <div class="flex items-center">
        <h1 class="text-xl font-bold text-emerald-600">Clínica Herramientas 2</h1>
      </div>
      <div class="flex items-center gap-4">
        <span class="text-sm text-gray-600">{{ userFullname }}</span>
        <span class="text-xs text-gray-400">({{ userRole }})</span>
        <button
          @click="handleLogout"
          class="text-sm text-gray-600 hover:text-gray-900"
        >
          Cerrar sesión
        </button>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { authService } from '@/services/authService'

const router = useRouter()
const authStore = useAuthStore()

const userFullname = computed(() => authStore.userFullname || 'Usuario')
const userRole = computed(() => authStore.userRole || '')

const handleLogout = () => {
  authService.logout()
  router.push('/login')
}
</script>

