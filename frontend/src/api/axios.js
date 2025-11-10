import axios from 'axios'
import { useAuthStore } from '@/stores/auth'
import { useToast } from '@/composables/useToast'
import { ENDPOINTS } from './endpoints'

// baseURL debe ser la URL completa o /api si usamos proxy de Vite
const baseURL = import.meta.env.VITE_API_BASE_URL || '/api'

const api = axios.create({
  baseURL: baseURL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor: agregar header de autenticación
api.interceptors.request.use(
  (config) => {
    const authStore = useAuthStore()
    if (authStore.user) {
      // Opción B: enviar DNI o username en header
      if (authStore.user.dni) {
        config.headers['X-User-Dni'] = authStore.user.dni
      }
      if (authStore.user.username) {
        config.headers['X-Username'] = authStore.user.username
      }
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor: manejo de errores global
api.interceptors.response.use(
  (response) => {
    return response
  },
  (error) => {
    const toast = useToast()
    
    if (error.response) {
      const { status, data } = error.response
      
      // Si es 401, limpiar auth y redirigir a login
      if (status === 401) {
        const authStore = useAuthStore()
        authStore.logout()
        window.location.href = '/login'
        toast.error('Sesión expirada. Por favor, inicia sesión nuevamente.')
        return Promise.reject(error)
      }
      
      // Mostrar mensaje de error del servidor
      const message = data?.message || 'Ha ocurrido un error'
      toast.error(message)
    } else if (error.request) {
      toast.error('No se pudo conectar con el servidor')
    } else {
      toast.error('Error inesperado')
    }
    
    return Promise.reject(error)
  }
)

export default api

