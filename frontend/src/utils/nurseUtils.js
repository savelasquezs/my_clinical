/**
 * Utilidades para cálculos de enfermería
 */

/**
 * Calcula el progreso de administración de medicamentos
 * @param {Object} medicationOrderItem - El MedicationOrderItem de la orden
 * @param {Array} administeredMedications - Lista de AdministeredMedication
 * @returns {Object} Objeto con administered, pending, total, percentage
 */
export function calculateMedicationProgress(medicationOrderItem, administeredMedications) {
  if (!medicationOrderItem || !administeredMedications) {
    return {
      administered: 0,
      pending: 0,
      total: 0,
      percentage: 0
    }
  }

  // Filtrar solo los medicamentos administrados que corresponden a este MedicationOrderItem
  const relevantMedications = administeredMedications.filter(am => 
    (am.orderItem?.orderNumber === medicationOrderItem.orderNumber || am.orderNumber === medicationOrderItem.orderNumber) &&
    (am.orderItem?.itemNumber === medicationOrderItem.itemNumber || am.itemNumber === medicationOrderItem.itemNumber)
  )

  // Sumar todas las dosis administradas
  // La dosis en AdministeredMedication es la cantidad administrada en esa visita específica
  let totalAdministered = 0
  relevantMedications.forEach(am => {
    const doseValue = parseFloat(am.dose) || 0
    totalAdministered += doseValue
  })

  // El total es el treatmentDuration (días) multiplicado por la dosis por día
  // dose es "dosis por día" (string que puede ser un número)
  const dosePerDay = parseFloat(medicationOrderItem.dose) || 0
  const totalDoses = medicationOrderItem.treatmentDuration * dosePerDay

  const pending = Math.max(0, totalDoses - totalAdministered)
  const percentage = totalDoses > 0 ? (totalAdministered / totalDoses) * 100 : 0

  return {
    administered: totalAdministered,
    pending: pending,
    total: totalDoses,
    percentage: Math.min(100, Math.max(0, percentage))
  }
}

/**
 * Verifica si un procedimiento ha sido realizado
 * @param {Object} procedureOrderItem - El ProcedureOrderItem de la orden
 * @param {Array} performedProcedures - Lista de PerformedProcedure
 * @returns {boolean} true si el procedimiento ha sido realizado
 */
export function isProcedurePerformed(procedureOrderItem, performedProcedures) {
  if (!procedureOrderItem || !performedProcedures || performedProcedures.length === 0) {
    return false
  }

  return performedProcedures.some(pp => 
    pp.orderNumber === procedureOrderItem.orderNumber &&
    pp.itemNumber === procedureOrderItem.itemNumber
  )
}

