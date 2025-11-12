<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
          Iniciar sesión
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          Clínica Herramientas 2
        </p>
      </div>
      <form class="mt-8 space-y-6" @submit.prevent="handleLogin">
        <div class="rounded-md shadow-sm -space-y-px">
          <div>
            <label for="username" class="sr-only">Usuario</label>
            <input
              id="username"
              v-model="formData.username"
              name="username"
              type="text"
              required
              class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-t-md focus:outline-none focus:ring-emerald-500 focus:border-emerald-500 focus:z-10 sm:text-sm"
              placeholder="Usuario"
            />
          </div>
          <div>
            <label for="password" class="sr-only">Contraseña</label>
            <input
              id="password"
              v-model="formData.password"
              name="password"
              type="password"
              required
              class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-b-md focus:outline-none focus:ring-emerald-500 focus:border-emerald-500 focus:z-10 sm:text-sm"
              placeholder="Contraseña"
            />
          </div>
        </div>

        <div v-if="error" class="text-red-600 text-sm text-center">
          {{ error }}
        </div>

        <div>
          <button
            type="submit"
            :disabled="loading"
            class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-emerald-600 hover:bg-emerald-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-emerald-500 disabled:opacity-50"
          >
            <span v-if="loading">Iniciando sesión...</span>
            <span v-else>Iniciar sesión</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { authService } from '@/services/authService'
import { useToast } from '@/composables/useToast'

const router = useRouter()
const toast = useToast()

const formData = ref({
  username: '',
  password: ''
})
const loading = ref(false)
const error = ref('')

const handleLogin = async () => {
  loading.value = true
  error.value = ''
  
  try {
    const user = await authService.login(formData.value.username, formData.value.password)
    toast.success('Sesión iniciada correctamente')
    const defaultRoute = authService.getDefaultRoute(user.role)
    router.push(defaultRoute)
  } catch (err) {
    // Manejar diferentes tipos de errores
    if (err.response) {
      // Error con respuesta del servidor
      error.value = err.response.data?.message || 'Usuario o contraseña incorrectos'
    } else if (err.request) {
      // Error de conexión (servidor no responde)
      error.value = 'No se pudo conectar con el servidor. Verifica que el backend esté corriendo.'
      console.error('Error de conexión:', err.request)
    } else {
      // Error inesperado
      error.value = 'Error inesperado al iniciar sesión'
      console.error('Error inesperado:', err.message)
    }
    // No mostrar toast aquí porque el interceptor ya no lo hace para login
    // pero sí mostramos el error en el formulario
  } finally {
    loading.value = false
  }
}
</script>

