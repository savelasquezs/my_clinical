# Clínica Herramientas 2 - Frontend

Frontend de la aplicación Clínica Herramientas 2 construido con Vue 3, Pinia, Tailwind CSS y Axios.

## Tecnologías

- Vue 3 (Composition API)
- Pinia (State Management)
- Vue Router
- Axios (HTTP Client)
- Tailwind CSS
- dayjs (Date handling)
- vue-toastification (Toast notifications)

## Instalación

```bash
npm install
```

## Desarrollo

```bash
npm run dev
```

La aplicación estará disponible en `http://localhost:5173`

## Build

```bash
npm run build
```

## Estructura del Proyecto

```
src/
├── api/              # Configuración de Axios y endpoints
├── stores/           # Stores de Pinia
├── services/         # Servicios de negocio
├── composables/      # Composables reutilizables
├── components/       # Componentes Vue
│   ├── shared/      # Componentes compartidos
│   ├── forms/       # Formularios
│   └── layout/      # Componentes de layout
├── views/           # Vistas por rol
├── router/          # Configuración de rutas
└── utils/           # Utilidades
```

## Autenticación

La aplicación usa autenticación simple (Opción B):
- Al hacer login, el usuario se almacena en Pinia store y localStorage
- En cada request, se envía el DNI y username en headers `X-User-Dni` y `X-Username`
- Si la respuesta es 401, se limpia la autenticación y se redirige a login

## Variables de Entorno

Crear archivo `.env`:

```
VITE_API_BASE_URL=http://localhost:5000/api
```

