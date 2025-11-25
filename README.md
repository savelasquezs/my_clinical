# Sistema de Gestión de Clínica

Sistema de gestión integral para clínicas médicas desarrollado con arquitectura hexagonal, que permite administrar pacientes, citas médicas, órdenes, facturación y recursos clínicos.

## Introducción al Proyecto

Este sistema está diseñado para facilitar la gestión diaria de una clínica médica, proporcionando diferentes interfaces según el rol del usuario:

- **Admin**: Gestión de pacientes, citas y facturación
- **Doctor**: Órdenes médicas, historiales clínicos, recetas y citas disponibles
- **Nurse**: Visitas de enfermería, registro de atención
- **RRHH**: Gestión de usuarios del sistema
- **Support**: Gestión de inventario de recursos clínicos

---

## 🚀 Guía de Instalación Paso a Paso

Sigue estos pasos para poner en marcha el proyecto desde cero:

### Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:

- **.NET 8.0 SDK** - [Descargar aquí](https://dotnet.microsoft.com/download/dotnet/8.0)
- **PostgreSQL 12 o superior** - [Descargar aquí](https://www.postgresql.org/download/)
- **Node.js 18 o superior** - [Descargar aquí](https://nodejs.org/)
- **Git** - [Descargar aquí](https://git-scm.com/downloads)

### Paso 1: Clonar el Repositorio

1. Copia la URL del repositorio desde GitHub
2. Abre una terminal (PowerShell, CMD, o Git Bash)
3. Navega a la carpeta donde quieres clonar el proyecto
4. Ejecuta el siguiente comando:

```bash
git clone https://github.com/savelasquezs/my_clinical.git
```

5. Navega a la carpeta del proyecto:

```bash
cd "Clinica Herramientas 2"
```

### Paso 2: Configurar PostgreSQL

1. **Inicia PostgreSQL** (si no está corriendo como servicio)

2. **Abre pgAdmin o psql** y crea la base de datos:

```sql
CREATE DATABASE clinica_herramientas_2;
```

O desde la terminal:
```bash
psql -U postgres -c "CREATE DATABASE clinica_herramientas_2;"
```

3. **Configura la cadena de conexión** en el archivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ClinicaDb": "Host=localhost;Port=5432;Database=clinica_herramientas_2;Username=postgres;Password=TU_PASSWORD"
  }
}
```

> **Importante**: Reemplaza `TU_PASSWORD` con la contraseña de tu usuario de PostgreSQL (por defecto suele ser la que configuraste durante la instalación).

### Paso 3: Aplicar Migraciones de Base de Datos

1. Desde la raíz del proyecto, ejecuta:

```bash
dotnet ef database update
```

Este comando:
- Crea todas las tablas necesarias en la base de datos
- Ejecuta la migración `SeedInitialRRHHUser` que crea el usuario inicial

2. **Verifica que se haya creado el usuario inicial** ejecutando en PostgreSQL:

```sql
SELECT username, role FROM app_user WHERE username = 'santiago';
```

Deberías ver:
- **Username**: `santiago`
- **Rol**: `RRHH`

### Paso 4: Instalar Dependencias del Frontend

1. Navega a la carpeta `frontend`:

```bash
cd frontend
```

2. Instala las dependencias de Node.js:

```bash
npm install
```

Esto puede tardar unos minutos la primera vez.

3. Regresa a la raíz del proyecto:

```bash
cd ..
```

### Paso 5: Ejecutar el Backend

1. Abre una **primera terminal** en la raíz del proyecto

2. Ejecuta el backend:

```bash
dotnet run
```

3. **Espera a ver este mensaje**:
```
Now listening on: http://localhost:5000
```

4. **Verifica que el backend esté funcionando**:
   - Abre tu navegador y ve a: `http://localhost:5000/swagger`
   - Deberías ver la documentación de la API (Swagger UI)

> **Nota**: Mantén esta terminal abierta mientras trabajas con el proyecto.

### Paso 6: Ejecutar el Frontend

1. Abre una **segunda terminal** en la raíz del proyecto

2. Navega a la carpeta `frontend`:

```bash
cd frontend
```

3. Ejecuta el servidor de desarrollo:

```bash
npm run dev
```

4. **Espera a ver este mensaje**:
```
  VITE v5.x.x  ready in xxx ms

  ➜  Local:   http://localhost:5173/
  ➜  Network: use --host to expose
```

> **Nota**: Mantén esta terminal abierta mientras trabajas con el proyecto.

### Paso 7: Acceder a la Aplicación

1. Abre tu navegador y ve a: `http://localhost:5173`

2. Deberías ver la **pantalla de login**

3. **Inicia sesión con las credenciales del usuario inicial**:
   - **Username**: `santiago`
   - **Password**: `admin123`

4. Una vez autenticado, serás redirigido al dashboard de **RRHH**

### ✅ Verificación Final

Para verificar que todo está funcionando correctamente:

- ✅ Backend corriendo en `http://localhost:5000`
- ✅ Swagger disponible en `http://localhost:5000/swagger`
- ✅ Frontend corriendo en `http://localhost:5173`
- ✅ Puedes iniciar sesión con las credenciales del usuario inicial
- ✅ El dashboard se carga correctamente después del login

### 🔐 Credenciales del Usuario Inicial

El sistema crea automáticamente un usuario de RRHH con las siguientes credenciales:

| Campo | Valor |
|-------|-------|
| **Username** | `santiago` |
| **Password** | `admin123` |
| **DNI** | `00000000` |
| **Rol** | `RRHH` |
| **Nombre completo** | `Santiago Admin` |
| **Email** | `santiago@clinica.com` |

> **⚠️ Importante**: Este usuario tiene permisos de RRHH para gestionar usuarios del sistema. Es **altamente recomendable** cambiar la contraseña después del primer inicio de sesión o crear usuarios adicionales según sea necesario.

---

## 📚 Documentación Técnica

## Arquitectura del Proyecto

El sistema está organizado en 3 capas principales siguiendo el patrón de **Arquitectura Hexagonal (Ports & Adapters)**:

- **Domain**: Modelos de negocio, puertos (interfaces) y servicios de dominio
- **Application**: Casos de uso específicos del sistema y adaptadores de entrada
- **Infrastructure**: Conexión a PostgreSQL, adaptadores de salida y configuración

### Stack Tecnológico

**Backend:**
- **.NET 8.0** - Framework principal
- **C# 12** - Lenguaje de programación
- **ASP.NET Core** - Framework web para API REST
- **Entity Framework Core 9.0.10** - ORM para acceso a datos
- **PostgreSQL** - Base de datos relacional
- **Npgsql 9.0.4** - Proveedor de PostgreSQL para .NET
- **Swagger/OpenAPI** - Documentación de API

**Frontend:**
- **Vue.js 3** - Framework JavaScript (Composition API)
- **Pinia** - State management
- **Vue Router** - Enrutamiento
- **Axios** - Cliente HTTP
- **Tailwind CSS** - Framework de estilos
- **Vite** - Build tool y dev server
- **Vue Toastification** - Notificaciones toast
- **Day.js** - Manejo de fechas
- **Heroicons** - Iconos

## Estructura del Proyecto

```
Clinica Herramientas 2/
├── Domain/                          # Capa de Dominio
│   ├── Model/                      # Entidades de negocio
│   │   ├── Patient.cs
│   │   ├── User.cs
│   │   ├── Order.cs
│   │   ├── Appointment.cs
│   │   ├── MedicalRecord.cs
│   │   └── ...
│   ├── Ports/                      # Interfaces (contratos)
│   │   ├── IPatientPort.cs
│   │   ├── IOrderPort.cs
│   │   ├── IAppointmentPort.cs
│   │   └── ...
│   └── Services/                    # Lógica de negocio
│       ├── CreatePatient.cs
│       ├── CreateOrder.cs
│       ├── CreateMedicalRecord.cs
│       └── ...
│
├── Application/                     # Capa de Aplicación
│   ├── Adapters/Input/              # Adaptadores de entrada
│   │   ├── AdminInputs.cs
│   │   ├── DoctorInputs.cs
│   │   ├── Builders/                # Patrón Builder
│   │   │   ├── PatientBuilder.cs
│   │   │   ├── OrderBuilder.cs
│   │   │   └── ...
│   │   └── Validators/              # Validadores
│   │       ├── PersonValidator.cs
│   │       ├── OrderValidator.cs
│   │       └── ...
│   └── UseCases/                    # Casos de uso
│       ├── AdminUseCase.cs
│       ├── DoctorUseCase.cs
│       ├── NurseUseCase.cs
│       └── ...
│
├── Infrastructure/                  # Capa de Infraestructura
│   ├── Adapters/
│   │   ├── Input/
│   │   │   └── Controllers/        # Controladores API REST
│   │   │       ├── Admin/
│   │   │       ├── Doctor/
│   │   │       ├── Nurse/
│   │   │       └── ...
│   │   └── Output/
│   │       └── Persistence/        # Persistencia con EF Core
│   │           ├── ClinicaDbContext.cs
│   │           ├── PostgresPatientPort.cs
│   │           ├── PostgresOrderPort.cs
│   │           └── ...
│   └── Config/                     # Configuración e inyección de dependencias
│       ├── ConfigFactory.cs
│       ├── AdminConfig.cs
│       ├── DoctorConfig.cs
│       └── ...
│
├── Migrations/                      # Migraciones de Entity Framework
│   ├── 20251021010542_InitialCreate.cs
│   └── ...
│
├── frontend/                        # Aplicación Frontend Vue.js
│   ├── src/
│   │   ├── api/                    # Configuración Axios y endpoints
│   │   ├── components/             # Componentes Vue
│   │   │   ├── shared/            # Componentes compartidos
│   │   │   ├── forms/             # Formularios
│   │   │   └── layout/           # Layout (Navbar, Sidebar, etc.)
│   │   ├── views/                 # Vistas por rol
│   │   │   ├── admin/
│   │   │   ├── doctor/
│   │   │   ├── nurse/
│   │   │   └── ...
│   │   ├── services/              # Servicios de negocio
│   │   ├── stores/                # Stores de Pinia
│   │   ├── router/                # Configuración de rutas
│   │   ├── composables/           # Composables reutilizables
│   │   └── utils/                 # Utilidades
│   ├── package.json
│   ├── vite.config.js
│   └── tailwind.config.js
│
├── Program.cs                       # Punto de entrada de la aplicación
├── appsettings.json                # Configuración (cadena de conexión)
├── Properties/
│   └── launchSettings.json         # Configuración de ejecución
└── Clinica Herramientas 2.csproj   # Archivo de proyecto
```

## Flujo de Datos

### Flujo Completo (Frontend → Backend → Base de Datos)

1. **Frontend (Vue.js)**: Usuario interactúa con la interfaz
2. **Servicio Frontend**: `doctorService.js`, `adminService.js`, etc. hacen peticiones HTTP
3. **Axios Interceptor**: Agrega headers de autenticación (`X-User-Dni`, `X-Username`)
4. **Vite Proxy**: Redirige `/api/*` a `http://localhost:5000`
5. **Backend Controller**: Recibe la petición HTTP (ej: `OrdersController.cs`)
6. **Input Adapter**: Valida y construye objetos usando Builders y Validators
7. **Use Case**: Coordina la operación y valida permisos
8. **Domain Service**: Ejecuta reglas de negocio
9. **Port (Interface)**: Define qué operaciones hacer (ej: `IOrderPort`)
10. **Output Adapter**: Implementa persistencia con EF Core (ej: `PostgresOrderPort`)
11. **Entity Framework Core**: Genera SQL y ejecuta en PostgreSQL
12. **PostgreSQL**: Almacena/recupera datos
13. **Respuesta**: Regresa por el mismo camino hasta el frontend
14. **Frontend**: Actualiza la UI con los datos recibidos

### Flujo con Error

- Si hay error de validación → Exception en Input Adapter/Validator
- Si hay error de reglas de negocio → Exception en Domain Service
- Si hay error de BD → Exception en Postgres Adapter
- Exception se propaga hacia arriba
- Axios interceptor captura el error y muestra toast notification
- Frontend maneja el error específicamente según el caso

## Base de Datos

### PostgreSQL

El sistema utiliza **PostgreSQL** como base de datos relacional. La configuración se encuentra en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ClinicaDb": "Host=localhost;Port=5432;Database=clinica_herramientas_2;Username=postgres;Password=tu_password"
  }
}
```

### Configuración de la Base de Datos

**Parámetros de conexión:**
- **Host**: `localhost` (o la IP del servidor PostgreSQL)
- **Port**: `5432` (puerto por defecto de PostgreSQL)
- **Database**: `clinica_herramientas_2`
- **Username**: Usuario de PostgreSQL (ej: `postgres`)
- **Password**: Contraseña del usuario

### Migraciones

El proyecto utiliza **Entity Framework Core Migrations** para gestionar el esquema de la base de datos.

**Comandos útiles:**

```bash
# Crear una nueva migración
dotnet ef migrations add NombreMigracion

# Aplicar migraciones pendientes
dotnet ef database update

# Revertir última migración
dotnet ef database update NombreMigracionAnterior

# Ver estado de migraciones
dotnet ef migrations list
```

**Migraciones existentes:**
- `20251021010542_InitialCreate` - Creación inicial del esquema
- `20251023002033_SeedInitialRRHHUser` - Usuario inicial de RRHH

### Estructura de Tablas Principales

- **person** - Información personal (base para Patient y User)
- **patient** - Pacientes con contacto de emergencia y seguro médico
- **app_user** - Usuarios del sistema con roles
- **appointment** - Citas médicas
- **order** - Órdenes médicas
- **medication_order_item** - Items de medicamentos en órdenes
- **procedure_order_item** - Items de procedimientos en órdenes
- **diagnostic_aid_order_item** - Items de ayudas diagnósticas en órdenes
- **medical_record** - Historiales clínicos
- **invoice** - Facturas
- **nurse_visit** - Visitas de enfermería
- **medication** - Inventario de medicamentos
- **procedure** - Inventario de procedimientos
- **diagnostic_aid** - Inventario de ayudas diagnósticas

## Frontend

### Vue.js 3 Application

El frontend está construido con **Vue.js 3** usando la **Composition API** y **Pinia** para el manejo de estado.

### Configuración del Frontend

**Instalación de dependencias:**
```bash
cd frontend
npm install
```

**Desarrollo:**
```bash
npm run dev
```
La aplicación estará disponible en `http://localhost:5173`

**Build para producción:**
```bash
npm run build
```

### Configuración de Vite

El archivo `vite.config.js` configura:
- **Puerto**: `5173` (puerto por defecto de Vite)
- **Proxy**: Redirige `/api/*` a `http://localhost:5000` (backend)

```javascript
server: {
  port: 5173,
  proxy: {
    '/api': {
      target: 'http://localhost:5000',
      changeOrigin: true
    }
  }
}
```

### Autenticación en el Frontend

El sistema utiliza autenticación basada en headers:

1. **Login**: El usuario se autentica y se almacena en Pinia store (`auth.js`) y `localStorage`
2. **Headers automáticos**: En cada petición HTTP, Axios interceptor agrega:
   - `X-User-Dni`: DNI del usuario autenticado
   - `X-Username`: Username del usuario autenticado
3. **Validación en backend**: Los controladores leen estos headers para identificar al usuario
4. **Manejo de 401**: Si la respuesta es 401, se limpia la autenticación y se redirige a login

### Estructura del Frontend

- **`src/api/`**: Configuración de Axios y definición de endpoints
- **`src/services/`**: Servicios de negocio que encapsulan llamadas API
- **`src/stores/`**: Stores de Pinia para manejo de estado global
- **`src/views/`**: Vistas principales organizadas por rol
- **`src/components/`**: Componentes reutilizables
  - **`shared/`**: Componentes compartidos (tablas, modales, etc.)
  - **`forms/`**: Formularios específicos
  - **`layout/`**: Componentes de layout (Navbar, Sidebar, Footer)
- **`src/router/`**: Configuración de rutas con Vue Router
- **`src/composables/`**: Composables reutilizables (useToast, useDate, etc.)
- **`src/utils/`**: Utilidades y helpers

### Variables de Entorno (Opcional)

Puedes crear un archivo `.env` en la carpeta `frontend/`:

```env
VITE_API_BASE_URL=http://localhost:5000/api
```

Si no se define, el frontend usa `/api` y el proxy de Vite redirige al backend.

## Comandos Útiles

### Backend

```bash
# Compilar el proyecto
dotnet build

# Ejecutar el proyecto
dotnet run

# Ejecutar con URL específica
dotnet run --urls "http://localhost:5000"

# Limpiar build
dotnet clean

# Ver migraciones aplicadas
dotnet ef migrations list

# Crear nueva migración
dotnet ef migrations add NombreMigracion

# Aplicar migraciones
dotnet ef database update
```

### Frontend

```bash
# Instalar dependencias
npm install

# Modo desarrollo
npm run dev

# Build para producción
npm run build

# Preview del build
npm run preview
```

## API REST y Swagger

El backend expone una **API REST** documentada con **Swagger/OpenAPI**.

### Acceso a Swagger

Una vez que el backend esté corriendo, accede a:
```
http://localhost:5000/swagger
```

### Endpoints Principales

**Autenticación:**
- `POST /api/auth/login` - Iniciar sesión

**Admin:**
- `GET /api/admin/patients` - Listar pacientes
- `POST /api/admin/patients` - Crear paciente
- `GET /api/admin/appointments` - Listar citas
- `POST /api/admin/appointments` - Crear cita
- `GET /api/admin/invoices` - Listar facturas
- `POST /api/admin/invoices` - Crear factura

**Doctor:**
- `GET /api/doctor/orders` - Listar órdenes
- `POST /api/doctor/orders` - Crear orden
- `GET /api/doctor/medical-records` - Listar registros médicos
- `POST /api/doctor/medical-records` - Crear registro médico
- `GET /api/doctor/appointments/available` - Citas disponibles

**Nurse:**
- `GET /api/nurse/visits` - Listar visitas
- `POST /api/nurse/visits` - Crear visita

**RRHH:**
- `GET /api/rrhh/users` - Listar usuarios
- `POST /api/rrhh/users` - Crear usuario

**Support:**
- `GET /api/support/inventory/medications` - Listar medicamentos
- `POST /api/support/inventory/medications` - Crear medicamento

### CORS

El backend está configurado para aceptar peticiones desde cualquier origen en desarrollo:
```csharp
policy.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();
```

## Patrones de Diseño Implementados

- **Hexagonal Architecture (Ports & Adapters)** - Separación de capas y dependencias
- **Factory Pattern** - `PortsFactory`, `ConfigFactory`
- **Builder Pattern** - `PatientBuilder`, `OrderBuilder`, `MedicalRecordBuilder`, etc.
- **Repository Pattern** - Ports actúan como repositorios
- **Dependency Injection** - Configuración completa en `ServiceCollectionExtensions.cs`
- **Strategy Pattern** - Diferentes estrategias de validación en Validators

## Modelos Principales

- **Patient**: Información del paciente, contacto de emergencia, seguro médico
- **User**: Usuarios del sistema con diferentes roles (Admin, Doctor, Nurse, RRHH, Support)
- **Appointment**: Citas médicas programadas con estado de aceptación
- **Order**: Órdenes médicas con items específicos (medicamentos, procedimientos, ayudas diagnósticas)
- **MedicalRecord**: Historial clínico de pacientes con diagnóstico y síntomas
- **Invoice**: Facturación de servicios con cálculos de copago y seguro
- **ClinicalResource**: Recursos clínicos (medicamentos, procedimientos, ayudas diagnósticas)
- **NurseVisit**: Visitas de enfermería con signos vitales y medicamentos administrados

## Configuración de Roles

El sistema maneja 5 tipos de usuarios con permisos específicos:

1. **Admin**: Acceso completo a gestión de pacientes, citas y facturación
2. **Doctor**: Creación de órdenes médicas, historiales clínicos y aceptación de citas
3. **Nurse**: Registro de visitas de enfermería y atención a pacientes
4. **RRHH**: Gestión de usuarios del sistema (crear, actualizar, eliminar)
5. **Support**: Administración de inventario de recursos clínicos

Cada rol tiene su propia configuración (`AdminConfig`, `DoctorConfig`, etc.) que incluye:
- Servicios de dominio específicos
- Casos de uso
- Builders y validadores
- Adaptadores de entrada

## Desarrollo

### Comandos Útiles

**Backend:**
```bash
# Compilar
dotnet build

# Ejecutar
dotnet run

# Ejecutar con URL específica
dotnet run --urls "http://localhost:5000"

# Limpiar build
dotnet clean
```

**Frontend:**
```bash
# Instalar dependencias
npm install

# Desarrollo
npm run dev

# Build para producción
npm run build

# Preview del build
npm run preview
```

### Debugging

**Backend:**
- Usa Visual Studio o VS Code con extensión de C#
- Los breakpoints funcionan normalmente
- Swagger permite probar endpoints directamente

**Frontend:**
- Usa las DevTools del navegador
- Vue DevTools extension para inspeccionar componentes y estado
- Network tab para ver peticiones HTTP

## Troubleshooting

### Error: Puerto 5000 ya en uso
```bash
# Windows - Encontrar proceso
netstat -ano | findstr :5000

# Matar proceso (reemplazar PID)
taskkill /PID <PID> /F
```

### Error: Base de datos no encontrada
- Verifica que PostgreSQL esté corriendo
- Verifica la cadena de conexión en `appsettings.json`
- Asegúrate de haber creado la base de datos

### Error: Migraciones pendientes
```bash
dotnet ef database update
```

### Frontend no se conecta al backend
- Verifica que el backend esté corriendo en `http://localhost:5000`
- Verifica la configuración del proxy en `vite.config.js`
- Revisa la consola del navegador para errores de CORS

## Licencia

Este proyecto está bajo la Licencia MIT. Ver `LICENSE.txt` para más detalles.
