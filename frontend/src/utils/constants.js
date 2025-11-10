export const ROLES = {
  ADMIN: 'Admin',
  DOCTOR: 'Doctor',
  NURSE: 'Nurse',
  RRHH: 'RRHH',
  SUPPORT: 'Support'
}

export const GENDERS = {
  MALE: 'Male',
  FEMALE: 'Female',
  OTHER: 'Other'
}

export const ORDER_ITEM_TYPES = {
  MEDICATION: 'Medication',
  PROCEDURE: 'Procedure',
  DIAGNOSTIC_AID: 'DiagnosticAid'
}

export const ROUTE_BY_ROLE = {
  [ROLES.ADMIN]: '/admin/patients',
  [ROLES.DOCTOR]: '/doctor/orders',
  [ROLES.NURSE]: '/nurse/visits',
  [ROLES.RRHH]: '/rrhh/users',
  [ROLES.SUPPORT]: '/support/inventory'
}

