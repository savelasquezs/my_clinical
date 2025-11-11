/**
 * Utilidad para filtrar registros médicos facturables
 * @param {Array} medicalRecords - Lista de registros médicos con sus órdenes
 * @param {Array} invoices - Lista de facturas con sus órdenes
 * @returns {Array} Lista de registros médicos cuya orden NO está en ninguna factura
 */
export function getPendingInvoices(medicalRecords, invoices) {
  if (!medicalRecords || !Array.isArray(medicalRecords)) {
    return []
  }
  
  if (!invoices || !Array.isArray(invoices)) {
    // Si no hay facturas, todos los registros médicos con órdenes son facturables
    return medicalRecords.filter(mr => mr.order && mr.order.orderNumber)
  }
  
  // Extraer todas las orderNumbers de las facturas
  const invoicedOrderNumbers = new Set()
  invoices.forEach(invoice => {
    if (invoice.orders && Array.isArray(invoice.orders)) {
      invoice.orders.forEach(order => {
        if (order && order.orderNumber) {
          invoicedOrderNumbers.add(order.orderNumber)
        }
      })
    }
  })
  
  // Filtrar registros médicos cuya orden NO esté en las facturas
  return medicalRecords.filter(medicalRecord => {
    // Solo incluir registros médicos que tengan una orden asociada
    if (!medicalRecord.order || !medicalRecord.order.orderNumber) {
      return false
    }
    
    // Excluir si la orden ya está facturada
    return !invoicedOrderNumbers.has(medicalRecord.order.orderNumber)
  })
}

