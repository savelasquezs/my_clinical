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
    APPOINTMENT_BY_ID: (id) => `/admin/appointments/${id}`,
    PATIENT_APPOINTMENTS: (dni) => `/admin/appointments/patient/${dni}`,
    INVOICES: '/admin/invoices',
    INVOICE_BY_NUMBER: (invoiceNumber) => `/admin/invoices/${invoiceNumber}`
  },
  
  // Doctor
  DOCTOR: {
    ORDERS: '/doctor/orders',
    ORDER_BY_NUMBER: (orderNumber) => `/doctor/orders/${orderNumber}`,
    PATIENT_ORDERS: (dni) => `/doctor/orders/patient/${dni}`,
    ADD_ORDER_ITEM: (orderNumber) => `/doctor/orders/${orderNumber}/items`,
    UPDATE_ORDER_ITEM: (orderNumber, itemNumber) => `/doctor/orders/${orderNumber}/items/${itemNumber}`,
    DELETE_ORDER_ITEM: (orderNumber, itemNumber) => `/doctor/orders/${orderNumber}/items/${itemNumber}`,
    NURSE_VISITS_BY_ORDER: (orderNumber) => `/doctor/orders/${orderNumber}/nurse-visits`,
    MEDICAL_RECORDS: '/doctor/medical-records',
    ALL_MEDICAL_RECORDS: '/doctor/medical-records',
    UPDATE_MEDICAL_RECORD: (id) => `/doctor/medical-records/${id}`,
    MEDICAL_HISTORY: (dni) => `/doctor/medical-records/patient/${dni}`
  },
  
  // Nurse
  NURSE: {
    VISITS: '/nurse/visits',
    PATIENT_INFO: (dni) => `/nurse/visits/patient/${dni}`,
    AVAILABLE_ORDERS: '/nurse/visits/available-orders',
    ORDER_DETAILS: (orderNumber) => `/nurse/visits/orders/${orderNumber}`
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

