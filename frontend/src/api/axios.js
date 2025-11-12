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
    
    // Excluir el endpoint de login del manejo automático de errores
    // para que LoginView pueda manejar el error específicamente
    const isLoginEndpoint = error.config?.url?.includes('/auth/login')
    
    if (error.response) {
      const { status, data } = error.response
      
      // Si es 401, limpiar auth y redirigir a login
      if (status === 401) {
        // No redirigir si ya estamos en login (evitar loop)
        if (!isLoginEndpoint) {
          const authStore = useAuthStore()
          authStore.logout()
          window.location.href = '/login'
          toast.error('Sesión expirada. Por favor, inicia sesión nuevamente.')
        }
        return Promise.reject(error)
      }
      
      // No mostrar toast automático para errores de login
      // LoginView manejará el error y mostrará el mensaje apropiado
      if (!isLoginEndpoint) {
        // Mostrar mensaje de error del servidor
        const message = data?.message || 'Ha ocurrido un error'
        toast.error(message)
      }
    } else if (error.request) {
      // No mostrar toast automático para errores de conexión en login
      if (!isLoginEndpoint) {
        toast.error('No se pudo conectar con el servidor')
      }
    } else {
      // No mostrar toast automático para errores inesperados en login
      if (!isLoginEndpoint) {
        toast.error('Error inesperado')
      }
    }
    
    return Promise.reject(error)
  }
)

export default api

