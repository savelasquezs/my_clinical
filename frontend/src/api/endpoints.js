// Los endpoints no incluyen /api porque Axios ya lo agrega con baseURL
export const ENDPOINTS = {
  // Auth
  AUTH: {
    LOGIN: '/auth/login'
  },
  
  // Admin
  ADMIN: {
    PATIENTS: '/admin/patients',
    PATIENT_BY_DNI: (dni) => `/admin/patients/${dni}`,
    APPOINTMENTS: '/admin/appointments',
    PATIENT_APPOINTMENTS: (dni) => `/admin/appointments/patient/${dni}`,
    INVOICES: '/admin/invoices'
  },
  
  // Doctor
  DOCTOR: {
    ORDERS: '/doctor/orders',
    PATIENT_ORDERS: (dni) => `/doctor/orders/patient/${dni}`,
    ADD_MEDICATION_TO_ORDER: (orderNumber) => `/doctor/orders/${orderNumber}/items/medication`,
    ADD_PROCEDURE_TO_ORDER: (orderNumber) => `/doctor/orders/${orderNumber}/items/procedure`,
    ADD_DIAGNOSTIC_AID_TO_ORDER: (orderNumber) => `/doctor/orders/${orderNumber}/items/diagnostic-aid`,
    MEDICAL_RECORDS: '/doctor/medical-records',
    MEDICAL_HISTORY: (dni) => `/doctor/medical-records/patient/${dni}`
  },
  
  // Nurse
  NURSE: {
    VISITS: '/nurse/visits',
    PATIENT_INFO: (dni) => `/nurse/visits/patient/${dni}`
  },
  
  // RRHH
  RRHH: {
    USERS: '/rrhh/users',
    USER_BY_USERNAME: (username) => `/rrhh/users/username/${username}`,
    USER_BY_DNI: (dni) => `/rrhh/users/${dni}`
  },
  
  // Support
  SUPPORT: {
    MEDICATIONS: '/support/inventory/medications',
    PROCEDURES: '/support/inventory/procedures',
    DIAGNOSTIC_AIDS: '/support/inventory/diagnostic-aids'
  }
}

