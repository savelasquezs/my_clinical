# Listado de Requisitos del Sistema Clínica

## 1. RECURSOS HUMANOS (RRHH)

### 1.1 Gestión de Usuarios
- [ ] **REQ-RRHH-001**: RRHH puede crear usuarios nuevos
- [ ] **REQ-RRHH-002**: RRHH puede eliminar usuarios
- [ ] **REQ-RRHH-003**: RRHH puede actualizar información de usuarios
- [ ] **REQ-RRHH-004**: RRHH NO puede visualizar información de pacientes
- [ ] **REQ-RRHH-005**: RRHH NO puede visualizar medicamentos
- [ ] **REQ-RRHH-006**: RRHH NO puede visualizar procedimientos

### 1.2 Validaciones de Usuario
- [ ] **REQ-RRHH-007**: Nombre de usuario debe ser único en todo el aplicativo
- [ ] **REQ-RRHH-008**: Nombre de usuario máximo 15 caracteres, solo letras y números
- [ ] **REQ-RRHH-009**: Contraseña debe incluir: mayúscula, número, carácter especial, mínimo 8 caracteres
- [ ] **REQ-RRHH-010**: Número de cédula debe ser único en todo el aplicativo
- [ ] **REQ-RRHH-011**: Correo electrónico debe ser válido (verificar dominio y @)
- [ ] **REQ-RRHH-012**: Número de teléfono debe contener entre 1 y 10 dígitos
- [ ] **REQ-RRHH-013**: Fecha de nacimiento formato DD/MM/YYYY, máximo 150 años
- [ ] **REQ-RRHH-014**: Dirección máximo 30 caracteres
- [ ] **REQ-RRHH-015**: Rol debe ser válido (Médico, Enfermera, Personal administrativo, Recursos Humanos, etc.)

## 2. PERSONAL ADMINISTRATIVO (Admin)

### 2.1 Gestión de Pacientes
- [ ] **REQ-ADMIN-001**: Admin puede registrar pacientes nuevos
- [ ] **REQ-ADMIN-002**: Admin puede actualizar información de pacientes
- [ ] **REQ-ADMIN-003**: Admin puede visualizar lista de pacientes
- [ ] **REQ-ADMIN-004**: Admin puede programar citas
- [ ] **REQ-ADMIN-005**: Admin puede registrar información de facturación
- [ ] **REQ-ADMIN-006**: Admin puede registrar información de seguros médicos

### 2.2 Validaciones de Paciente
- [ ] **REQ-ADMIN-007**: Número de identificación (cédula) debe ser único
- [ ] **REQ-ADMIN-008**: Fecha de nacimiento máximo 150 años
- [ ] **REQ-ADMIN-009**: Número de teléfono debe tener 10 dígitos
- [ ] **REQ-ADMIN-010**: Correo electrónico debe ser válido

### 2.3 Contacto de Emergencia
- [ ] **REQ-ADMIN-011**: Paciente debe tener mínimo y máximo un solo contacto de emergencia
- [ ] **REQ-ADMIN-012**: Nombre del contacto de emergencia (separar nombres y apellidos)
- [ ] **REQ-ADMIN-013**: Relación con el paciente debe estar definida
- [ ] **REQ-ADMIN-014**: Número de teléfono de emergencia: 10 dígitos, solo números

### 2.4 Seguro Médico
- [ ] **REQ-ADMIN-015**: Paciente puede tener solo una póliza
- [ ] **REQ-ADMIN-016**: Nombre de la compañía de seguros debe estar registrado
- [ ] **REQ-ADMIN-017**: Número de póliza debe estar registrado
- [ ] **REQ-ADMIN-018**: Estado de la póliza se maneja con booleano (activa/inactiva)
- [ ] **REQ-ADMIN-019**: Vigencia de la póliza formato DD/MM/YYYY

### 2.5 Facturación
- [ ] **REQ-ADMIN-020**: Factura debe mostrar: nombre del paciente, edad y cédula
- [ ] **REQ-ADMIN-021**: Factura debe mostrar: nombre del médico tratante
- [ ] **REQ-ADMIN-022**: Factura debe mostrar: nombre de la compañía de seguro
- [ ] **REQ-ADMIN-023**: Factura debe mostrar: número de póliza
- [ ] **REQ-ADMIN-024**: Factura debe mostrar: días de vigencia de la póliza
- [ ] **REQ-ADMIN-025**: Factura debe mostrar: fecha de finalización de la póliza
- [ ] **REQ-ADMIN-026**: Factura debe incluir información de órdenes generadas para el paciente

### 2.6 Detalle de Facturación por Diagnóstico
- [ ] **REQ-ADMIN-027**: Si hay medicamentos: mostrar nombre, costo y dosis aplicadas
- [ ] **REQ-ADMIN-028**: Si hay procedimientos: mostrar nombre del procedimiento
- [ ] **REQ-ADMIN-029**: Si hay ayuda diagnóstica: mostrar nombre del examen aplicado

### 2.7 Reglas de Copago
- [ ] **REQ-ADMIN-030**: Si póliza activa: copago de $50.000, resto a aseguradora (mostrar detalle completo)
- [ ] **REQ-ADMIN-031**: Si copago total del año supera $1.000.000: no pagar más copago hasta siguiente año, aseguradora asume totalidad
- [ ] **REQ-ADMIN-032**: Si póliza inactiva o no posee: paciente paga total de servicios prestados

## 3. SOPORTE DE INFORMACIÓN (Support)

### 3.1 Gestión de Inventario
- [ ] **REQ-SUPPORT-001**: Support puede crear medicamentos
- [ ] **REQ-SUPPORT-002**: Support puede actualizar medicamentos
- [ ] **REQ-SUPPORT-003**: Support puede crear procedimientos
- [ ] **REQ-SUPPORT-004**: Support puede actualizar procedimientos
- [ ] **REQ-SUPPORT-005**: Support puede crear ayudas diagnósticas
- [ ] **REQ-SUPPORT-006**: Support puede actualizar ayudas diagnósticas
- [ ] **REQ-SUPPORT-007**: Support puede visualizar inventario completo

### 3.2 Validaciones de Inventario
- [ ] **REQ-SUPPORT-008**: ID de recurso clínico debe ser único entre todos los tipos (medicamentos, procedimientos, ayudas diagnósticas)

## 4. ENFERMERAS (Nurse)

### 4.1 Gestión de Visitas
- [ ] **REQ-NURSE-001**: Enfermera puede registrar visitas de pacientes
- [ ] **REQ-NURSE-002**: Enfermera puede visualizar información de pacientes (solo lectura)
- [ ] **REQ-NURSE-003**: Enfermera puede registrar datos vitales: presión arterial
- [ ] **REQ-NURSE-004**: Enfermera puede registrar datos vitales: temperatura
- [ ] **REQ-NURSE-005**: Enfermera puede registrar datos vitales: pulso
- [ ] **REQ-NURSE-006**: Enfermera puede registrar datos vitales: nivel de oxígeno en la sangre
- [ ] **REQ-NURSE-007**: Enfermera puede registrar medicamentos administrados
- [ ] **REQ-NURSE-008**: Enfermera puede registrar procedimientos realizados
- [ ] **REQ-NURSE-009**: Enfermera debe dejar registro de la orden e ítem asociados a medicamentos/procedimientos
- [ ] **REQ-NURSE-010**: Enfermera puede registrar pruebas realizadas
- [ ] **REQ-NURSE-011**: Enfermera puede registrar observaciones relevantes

## 5. MÉDICOS (Doctor)

### 5.1 Gestión de Historia Clínica
- [ ] **REQ-DOCTOR-001**: Médico puede visualizar toda la información del paciente
- [ ] **REQ-DOCTOR-002**: Médico puede crear registros médicos (historia clínica)
- [ ] **REQ-DOCTOR-003**: Médico puede actualizar registros médicos
- [ ] **REQ-DOCTOR-004**: Toda atención médica genera un registro en la historia clínica

### 5.2 Estructura de Historia Clínica
- [ ] **REQ-DOCTOR-005**: Clave principal: cédula del paciente
- [ ] **REQ-DOCTOR-006**: Subclave: fecha de atención
- [ ] **REQ-DOCTOR-007**: Registro debe incluir: fecha (clave del diccionario)
- [ ] **REQ-DOCTOR-008**: Registro debe incluir: cédula del médico que atendió (máximo 10 dígitos)
- [ ] **REQ-DOCTOR-009**: Registro debe incluir: motivo de la consulta
- [ ] **REQ-DOCTOR-010**: Registro debe incluir: sintomatología
- [ ] **REQ-DOCTOR-011**: Registro debe incluir: diagnóstico

### 5.3 Gestión de Órdenes
- [ ] **REQ-DOCTOR-012**: Médico puede crear órdenes
- [ ] **REQ-DOCTOR-013**: Médico puede agregar items a órdenes
- [ ] **REQ-DOCTOR-014**: Médico puede editar items de órdenes
- [ ] **REQ-DOCTOR-015**: Médico puede eliminar items de órdenes
- [ ] **REQ-DOCTOR-016**: Médico puede visualizar órdenes completas con items

### 5.4 Items de Orden - Medicamentos
- [ ] **REQ-DOCTOR-017**: Orden de medicamento debe incluir: número de orden (máximo 6 dígitos)
- [ ] **REQ-DOCTOR-018**: Orden de medicamento debe incluir: ID del medicamento (del inventario)
- [ ] **REQ-DOCTOR-019**: Orden de medicamento debe incluir: dosis
- [ ] **REQ-DOCTOR-020**: Orden de medicamento debe incluir: duración del tratamiento
- [ ] **REQ-DOCTOR-021**: Orden de medicamento debe incluir: número de ítem dentro de la orden

### 5.5 Items de Orden - Procedimientos
- [ ] **REQ-DOCTOR-022**: Orden de procedimiento debe incluir: número de orden (máximo 6 dígitos)
- [ ] **REQ-DOCTOR-023**: Orden de procedimiento debe incluir: ID del procedimiento (del inventario)
- [ ] **REQ-DOCTOR-024**: Orden de procedimiento debe incluir: frecuencia con la que se repite
- [ ] **REQ-DOCTOR-025**: Orden de procedimiento debe incluir: requiere asistencia de especialista (booleano)
- [ ] **REQ-DOCTOR-026**: Si requiere especialista: habilitar lista con especialidades
- [ ] **REQ-DOCTOR-027**: Orden de procedimiento debe incluir: ID del tipo de especialista (si es necesario)
- [ ] **REQ-DOCTOR-028**: Orden de procedimiento debe incluir: número de ítem dentro de la orden

### 5.6 Items de Orden - Ayudas Diagnósticas
- [ ] **REQ-DOCTOR-029**: Orden de ayuda diagnóstica debe incluir: número de orden (máximo 6 dígitos)
- [ ] **REQ-DOCTOR-030**: Orden de ayuda diagnóstica debe incluir: ID de la ayuda diagnóstica (del inventario)
- [ ] **REQ-DOCTOR-031**: Orden de ayuda diagnóstica debe incluir: cantidad
- [ ] **REQ-DOCTOR-032**: Orden de ayuda diagnóstica debe incluir: requiere asistencia de especialista (booleano)
- [ ] **REQ-DOCTOR-033**: Si requiere especialista: habilitar lista con especialidades
- [ ] **REQ-DOCTOR-034**: Orden de ayuda diagnóstica debe incluir: ID del tipo de especialista (si es necesario)
- [ ] **REQ-DOCTOR-035**: Orden de ayuda diagnóstica debe incluir: número de ítem dentro de la orden

### 5.7 Reglas de Negocio de Órdenes
- [ ] **REQ-DOCTOR-036**: Si se receta ayuda diagnóstica: NO se puede recetar procedimiento ni medicamento (no hay certeza del diagnóstico)
- [ ] **REQ-DOCTOR-037**: Cuando se ven resultados de ayuda diagnóstica: crear nuevo registro con diagnóstico y recetar medicamentos/procedimientos
- [ ] **REQ-DOCTOR-038**: Las órdenes deben ser únicas (no se repite identificador de orden)
- [ ] **REQ-DOCTOR-039**: Varios medicamentos recetados van asociados a la misma orden
- [ ] **REQ-DOCTOR-040**: Varios procedimientos solicitados van asociados a la misma orden
- [ ] **REQ-DOCTOR-041**: No puede existir dos elementos dentro de la misma orden que correspondan al mismo ítem (aunque sean de diferente tipo)

### 5.8 Estructura de Base de Datos - Órdenes
- [ ] **REQ-DOCTOR-042**: Tabla de órdenes debe incluir: número de orden
- [ ] **REQ-DOCTOR-043**: Tabla de órdenes debe incluir: cédula del paciente
- [ ] **REQ-DOCTOR-044**: Tabla de órdenes debe incluir: cédula del médico
- [ ] **REQ-DOCTOR-045**: Tabla de órdenes debe incluir: fecha de creación

### 5.9 Tabla Especializada - Medicamentos
- [ ] **REQ-DOCTOR-046**: Tabla de órdenes de medicamento debe incluir: número de orden
- [ ] **REQ-DOCTOR-047**: Tabla de órdenes de medicamento debe incluir: número ítem
- [ ] **REQ-DOCTOR-048**: Tabla de órdenes de medicamento debe incluir: nombre del medicamento
- [ ] **REQ-DOCTOR-049**: Tabla de órdenes de medicamento debe incluir: dosis
- [ ] **REQ-DOCTOR-050**: Tabla de órdenes de medicamento debe incluir: duración del tratamiento
- [ ] **REQ-DOCTOR-051**: Tabla de órdenes de medicamento debe incluir: costo

### 5.10 Tabla Especializada - Procedimientos
- [ ] **REQ-DOCTOR-052**: Tabla de órdenes de procedimiento debe incluir: número de orden
- [ ] **REQ-DOCTOR-053**: Tabla de órdenes de procedimiento debe incluir: número de ítem
- [ ] **REQ-DOCTOR-054**: Tabla de órdenes de procedimiento debe incluir: nombre del procedimiento
- [ ] **REQ-DOCTOR-055**: Tabla de órdenes de procedimiento debe incluir: número de veces que se repite
- [ ] **REQ-DOCTOR-056**: Tabla de órdenes de procedimiento debe incluir: frecuencia con la que se repite
- [ ] **REQ-DOCTOR-057**: Tabla de órdenes de procedimiento debe incluir: costo
- [ ] **REQ-DOCTOR-058**: Tabla de órdenes de procedimiento debe incluir: requiere asistencia por especialista (booleano)
- [ ] **REQ-DOCTOR-059**: Tabla de órdenes de procedimiento debe incluir: ID del tipo de especialidad

### 5.11 Tabla Especializada - Ayudas Diagnósticas
- [ ] **REQ-DOCTOR-060**: Tabla de órdenes de ayuda diagnóstica debe incluir: número de orden
- [ ] **REQ-DOCTOR-061**: Tabla de órdenes de ayuda diagnóstica debe incluir: número de ítem
- [ ] **REQ-DOCTOR-062**: Tabla de órdenes de ayuda diagnóstica debe incluir: nombre de la ayuda diagnóstica
- [ ] **REQ-DOCTOR-063**: Tabla de órdenes de ayuda diagnóstica debe incluir: cantidad
- [ ] **REQ-DOCTOR-064**: Tabla de órdenes de ayuda diagnóstica debe incluir: costo
- [ ] **REQ-DOCTOR-065**: Tabla de órdenes de ayuda diagnóstica debe incluir: requiere asistencia por especialista (booleano)
- [ ] **REQ-DOCTOR-066**: Tabla de órdenes de ayuda diagnóstica debe incluir: ID del tipo de especialidad

### 5.12 Reglas Adicionales de Items
- [ ] **REQ-DOCTOR-067**: Cuando orden de medicamento tiene varios medicamentos: cada uno es un ítem comenzando desde 1, relación orden-ítem única
- [ ] **REQ-DOCTOR-068**: Cuando orden de procedimiento tiene varios procedimientos: cada uno es un ítem comenzando desde 1, relación orden-ítem única
- [ ] **REQ-DOCTOR-069**: Cuando orden tiene varias ayudas diagnósticas: cada una es un ítem comenzando desde 1, relación orden-ítem única
- [ ] **REQ-DOCTOR-070**: Cuando orden tiene medicamentos y procedimientos: cada elemento es un ítem, relación orden-ítem única
- [ ] **REQ-DOCTOR-071**: Hospitalización se considera como procedimiento
- [ ] **REQ-DOCTOR-072**: Visitas de enfermeras durante hospitalización se detallan como procedimientos
- [ ] **REQ-DOCTOR-073**: Medicamentos aplicados durante hospitalización se incluyen en la orden con forma de aplicación

## 6. ACCESO A INVENTARIO

- [ ] **REQ-INV-001**: Doctor puede visualizar medicamentos (solo lectura)
- [ ] **REQ-INV-002**: Doctor puede visualizar procedimientos (solo lectura)
- [ ] **REQ-INV-003**: Doctor puede visualizar ayudas diagnósticas (solo lectura)
- [ ] **REQ-INV-004**: Support tiene acceso completo (CRUD) a inventario

## 7. VALIDACIONES GENERALES

- [ ] **REQ-VAL-001**: Validación de formato de fechas (DD/MM/YYYY)
- [ ] **REQ-VAL-002**: Validación de rangos de edad (máximo 150 años)
- [ ] **REQ-VAL-003**: Validación de formatos de teléfono (10 dígitos)
- [ ] **REQ-VAL-004**: Validación de formatos de email
- [ ] **REQ-VAL-005**: Validación de unicidad de identificadores (cédulas, números de orden, etc.)
- [ ] **REQ-VAL-006**: Validación de longitud de campos de texto

## 8. INTERFAZ DE USUARIO

- [ ] **REQ-UI-001**: Sistema de autenticación funcional
- [ ] **REQ-UI-002**: Navegación por roles funcional
- [ ] **REQ-UI-003**: Formularios con validación en tiempo real
- [ ] **REQ-UI-004**: Mensajes de error claros y descriptivos
- [ ] **REQ-UI-005**: Sistema de notificaciones (toast) para todas las operaciones
- [ ] **REQ-UI-006**: Modales funcionales para creación/edición
- [ ] **REQ-UI-007**: Tablas con paginación y búsqueda donde sea necesario

