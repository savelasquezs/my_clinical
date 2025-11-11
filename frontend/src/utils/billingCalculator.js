/**
 * Calcula el copago y monto del seguro según las reglas de negocio
 * Replica la lógica de BillingRulesService.CalculateBilling
 * 
 * @param {Object} patient - Objeto paciente con información de seguro
 * @param {number} totalAmount - Monto total de la factura
 * @param {number} annualCopaymentAccumulated - Copago acumulado del año
 * @returns {Object} Objeto con copaymentAmount, insuranceAmount, annualCopaymentAccumulated
 */
export function calculateBilling(patient, totalAmount, annualCopaymentAccumulated = 0) {
  // Validar parámetros
  if (!patient) {
    throw new Error('El paciente es requerido')
  }
  
  if (typeof totalAmount !== 'number' || totalAmount < 0) {
    throw new Error('El monto total debe ser un número positivo')
  }
  
  if (typeof annualCopaymentAccumulated !== 'number' || annualCopaymentAccumulated < 0) {
    annualCopaymentAccumulated = 0
  }
  
  const insurance = patient.insurance
  
  // Obtener solo la fecha de hoy (sin hora) para comparar con DateTime.Today del backend
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  const todayDateOnly = new Date(today.getFullYear(), today.getMonth(), today.getDate())
  
  // Verificar si no hay seguro, está inactivo o expirado
  // La comparación debe ser exactamente igual al backend: ExpirationDate < DateTime.Today
  let isExpired = false
  if (insurance && insurance.expirationDate) {
    const expirationDate = new Date(insurance.expirationDate)
    // Comparar solo la parte de fecha (sin hora)
    const expirationDateOnly = new Date(expirationDate.getFullYear(), expirationDate.getMonth(), expirationDate.getDate())
    // Si la fecha de expiración es menor que hoy (ya expiró), se considera sin seguro
    isExpired = expirationDateOnly < todayDateOnly
  }
  
  if (!insurance || !insurance.isActive || isExpired) {
    // Sin seguro: paciente paga todo
    return {
      copaymentAmount: totalAmount,
      insuranceAmount: 0,
      annualCopaymentAccumulated: annualCopaymentAccumulated
    }
  }
  
  // Verificar si ya alcanzó el tope anual (1 millón de pesos)
  if (annualCopaymentAccumulated >= 1000000) {
    // Ya alcanzó el tope anual: aseguradora paga todo
    return {
      copaymentAmount: 0,
      insuranceAmount: totalAmount,
      annualCopaymentAccumulated: annualCopaymentAccumulated
    }
  }
  
  // Copago normal: $50,000 (o el total si es menor)
  const copayment = Math.min(50000, totalAmount)
  
  return {
    copaymentAmount: copayment,
    insuranceAmount: totalAmount - copayment,
    annualCopaymentAccumulated: annualCopaymentAccumulated
  }
}

