<template>
  <aside class="w-64 bg-white border-r border-gray-200 min-h-screen">
    <nav class="p-4">
      <ul class="space-y-2">
        <li v-for="item in menuItems" :key="item.path">
          <router-link
            :to="item.path"
            class="flex items-center gap-3 px-4 py-2 rounded-lg text-gray-700 hover:bg-emerald-50 hover:text-emerald-600 transition-colors"
            :class="{ 'bg-emerald-50 text-emerald-600': $route.path === item.path }"
          >
            <span>{{ item.label }}</span>
          </router-link>
        </li>
      </ul>
    </nav>
  </aside>
</template>

<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { ROLES } from '@/utils/constants'

const authStore = useAuthStore()

const menuItemsByRole = {
  [ROLES.ADMIN]: [
    { path: '/admin/patients', label: 'Pacientes', icon: 'UserIcon' },
    { path: '/admin/appointments', label: 'Citas', icon: 'CalendarIcon' },
    { path: '/admin/invoices', label: 'Facturas', icon: 'DocumentIcon' }
  ],
  [ROLES.DOCTOR]: [
    { path: '/doctor/orders', label: 'Órdenes', icon: 'ClipboardIcon' },
    { path: '/doctor/medical-records', label: 'Registros Médicos', icon: 'DocumentTextIcon' }
  ],
  [ROLES.NURSE]: [
    { path: '/nurse/visits', label: 'Visitas', icon: 'HeartIcon' }
  ],
  [ROLES.RRHH]: [
    { path: '/rrhh/users', label: 'Usuarios', icon: 'UsersIcon' }
  ],
  [ROLES.SUPPORT]: [
    { path: '/support/inventory', label: 'Inventario', icon: 'CubeIcon' }
  ]
}

const menuItems = computed(() => {
  const role = authStore.userRole
  return menuItemsByRole[role] || []
})
</script>

