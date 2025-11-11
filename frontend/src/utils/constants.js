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
  [ROLES.DOCTOR]: '/doctor/medical-records',
  [ROLES.NURSE]: '/nurse/visits',
  [ROLES.RRHH]: '/rrhh/users',
  [ROLES.SUPPORT]: '/support/inventory'
}

// Catálogo de compañías de seguros válidas (debe coincidir con el backend)
export const VALID_INSURANCE_COMPANIES = [
  'Sura',
  'EPS Sura',
  'Nueva EPS',
  'Sanitas',
  'Coomeva',
  'Salud Total',
  'Cruz Blanca',
  'Famisanar',
  'Compensar',
  'Cafesalud',
  'Medimas',
  'Aliansalud',
  'Asmet Salud',
  'Capital Salud',
  'Comfenalco',
  'Comfandi',
  'Comfama',
  'Comfasucre',
  'Comparta',
  'Convida',
  'Coosalud',
  'Dusakawi',
  'Ecoopsos',
  'Emdisalud',
  'Emssanar',
  'Eps Familiar',
  'Fundación Salud Mía',
  'Gold Cross',
  'Humana Vivir',
  'IPS Universitaria',
  'Mallamas',
  'Medimás',
  'Mutual Ser',
  'Pijaos Salud',
  'Policía Nacional',
  'Protección Social',
  'Salud Vida',
  'Savia Salud',
  'Sisben',
  'SOS',
  'Sura EPS',
  'Total Salud',
  'Unimec',
  'Vida y Salud'
]

