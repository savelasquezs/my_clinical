import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { ROUTE_BY_ROLE } from '@/utils/constants'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/login',
      name: 'Login',
      component: () => import('@/views/auth/LoginView.vue'),
      meta: { requiresAuth: false }
    },
    {
      path: '/admin',
      component: () => import('@/components/layout/AppLayout.vue'),
      meta: { requiresAuth: true, role: 'Admin' },
      children: [
        {
          path: '',
          redirect: '/admin/patients'
        },
        {
          path: 'patients',
          name: 'Patients',
          component: () => import('@/views/admin/PatientsView.vue')
        },
        {
          path: 'appointments',
          name: 'Appointments',
          component: () => import('@/views/admin/AppointmentsView.vue')
        },
        {
          path: 'invoices',
          name: 'Invoices',
          component: () => import('@/views/admin/InvoicesView.vue')
        }
      ]
    },
    {
      path: '/doctor',
      component: () => import('@/components/layout/AppLayout.vue'),
      meta: { requiresAuth: true, role: 'Doctor' },
      children: [
        {
          path: '',
          redirect: '/doctor/orders'
        },
        {
          path: 'orders',
          name: 'Orders',
          component: () => import('@/views/doctor/OrdersView.vue')
        },
        {
          path: 'medical-records',
          name: 'MedicalRecords',
          component: () => import('@/views/doctor/MedicalRecordsView.vue')
        }
      ]
    },
    {
      path: '/nurse',
      component: () => import('@/components/layout/AppLayout.vue'),
      meta: { requiresAuth: true, role: 'Nurse' },
      children: [
        {
          path: '',
          redirect: '/nurse/visits'
        },
        {
          path: 'visits',
          name: 'Visits',
          component: () => import('@/views/nurse/VisitsView.vue')
        }
      ]
    },
    {
      path: '/rrhh',
      component: () => import('@/components/layout/AppLayout.vue'),
      meta: { requiresAuth: true, role: 'RRHH' },
      children: [
        {
          path: '',
          redirect: '/rrhh/users'
        },
        {
          path: 'users',
          name: 'Users',
          component: () => import('@/views/rrhh/UsersView.vue')
        }
      ]
    },
    {
      path: '/support',
      component: () => import('@/components/layout/AppLayout.vue'),
      meta: { requiresAuth: true, role: 'Support' },
      children: [
        {
          path: '',
          redirect: '/support/inventory'
        },
        {
          path: 'inventory',
          name: 'Inventory',
          component: () => import('@/views/support/InventoryView.vue')
        }
      ]
    },
    {
      path: '/',
      redirect: '/login'
    }
  ]
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  authStore.checkAuth()

  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)

  if (requiresAuth && !authStore.isAuthenticated) {
    next('/login')
  } else if (to.path === '/login' && authStore.isAuthenticated) {
    const defaultRoute = ROUTE_BY_ROLE[authStore.userRole] || '/login'
    next(defaultRoute)
  } else if (requiresAuth && to.meta.role && authStore.userRole !== to.meta.role) {
    const defaultRoute = ROUTE_BY_ROLE[authStore.userRole] || '/login'
    next(defaultRoute)
  } else {
    next()
  }
})

export default router

