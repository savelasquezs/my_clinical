using Microsoft.EntityFrameworkCore;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Ports;
using System.Linq;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Output.Persistence
{
    internal class PostgresInvoicePort : IInvoicePort
    {
        private readonly ClinicaDbContext context;
        
        public PostgresInvoicePort(ClinicaDbContext context)
        {
            this.context = context;
        }
        
        public Invoice? FindByInvoiceNumber(int invoiceNumber)
        {
            var invoice = context.Invoices
                .Include(i => i.Patient)
                .Include(i => i.Doctor)
                .Include(i => i.Orders)
                .FirstOrDefault(i => i.InvoiceNumber == invoiceNumber);

            if (invoice != null)
            {
                // Cargar los items de cada orden con sus relaciones específicas
                foreach (var order in invoice.Orders)
                {
                    // Cargar MedicationOrderItems con Medication
                    var medicationItems = context.Set<MedicationOrderItem>()
                        .Include(m => m.Medication)
                        .Where(i => i.OrderNumber == order.OrderNumber)
                        .ToList();

                    // Cargar ProcedureOrderItems con Procedure
                    var procedureItems = context.Set<ProcedureOrderItem>()
                        .Include(p => p.Procedure)
                        .Where(i => i.OrderNumber == order.OrderNumber)
                        .ToList();

                    // Cargar DiagnosticAidOrderItems con DiagnosticAid
                    var diagnosticAidItems = context.Set<DiagnosticAidOrderItem>()
                        .Include(d => d.DiagnosticAid)
                        .Where(i => i.OrderNumber == order.OrderNumber)
                        .ToList();

                    // Reemplazar los items base con los items específicos que tienen las relaciones cargadas
                    var allItems = new List<OrderItem>();
                    allItems.AddRange(medicationItems);
                    allItems.AddRange(procedureItems);
                    allItems.AddRange(diagnosticAidItems);
                    order.Items.Clear();
                    foreach (var item in allItems.OrderBy(i => i.ItemNumber))
                    {
                        order.Items.Add(item);
                    }
                }
            }

            return invoice;
        }
        
        public void Save(Invoice invoice)
        {
            context.Invoices.Add(invoice);
            context.SaveChanges();
        }
        
        public void Update(Invoice invoice)
        {
            context.Invoices.Update(invoice);
            context.SaveChanges();
        }
        
        public void Delete(Invoice invoice)
        {
            context.Invoices.Remove(invoice);
            context.SaveChanges();
        }
        
        public List<Invoice> FindAll()
        {
            var invoices = context.Invoices
                .Include(i => i.Patient)
                .Include(i => i.Doctor)
                .Include(i => i.Orders)
                .ToList();

            // Cargar los items de cada orden con sus relaciones específicas
            foreach (var invoice in invoices)
            {
                foreach (var order in invoice.Orders)
                {
                    // Cargar MedicationOrderItems con Medication
                    var medicationItems = context.Set<MedicationOrderItem>()
                        .Include(m => m.Medication)
                        .Where(i => i.OrderNumber == order.OrderNumber)
                        .ToList();

                    // Cargar ProcedureOrderItems con Procedure
                    var procedureItems = context.Set<ProcedureOrderItem>()
                        .Include(p => p.Procedure)
                        .Where(i => i.OrderNumber == order.OrderNumber)
                        .ToList();

                    // Cargar DiagnosticAidOrderItems con DiagnosticAid
                    var diagnosticAidItems = context.Set<DiagnosticAidOrderItem>()
                        .Include(d => d.DiagnosticAid)
                        .Where(i => i.OrderNumber == order.OrderNumber)
                        .ToList();

                    // Reemplazar los items base con los items específicos que tienen las relaciones cargadas
                    var allItems = new List<OrderItem>();
                    allItems.AddRange(medicationItems);
                    allItems.AddRange(procedureItems);
                    allItems.AddRange(diagnosticAidItems);
                    order.Items.Clear();
                    foreach (var item in allItems.OrderBy(i => i.ItemNumber))
                    {
                        order.Items.Add(item);
                    }
                }
            }

            return invoices;
        }
        
        public List<Invoice> FindByPatient(string patientDni)
        {
            return context.Invoices
                .Include(i => i.Patient)
                .Include(i => i.Doctor)
                .Include(i => i.Orders)
                .Where(i => i.Patient.Dni == patientDni)
                .ToList();
        }
        
        public List<Invoice> FindByDateRange(DateTime startDate, DateTime endDate)
        {
            return context.Invoices
                .Include(i => i.Patient)
                .Include(i => i.Doctor)
                .Include(i => i.Orders)
                .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                .ToList();
        }
        
        public Invoice? FindByNumber(int invoiceNumber)
        {
            return FindByInvoiceNumber(invoiceNumber);
        }
        
        public decimal GetAnnualCopaymentAccumulated(string patientDni, int year)
        {
            // Por ahora retornamos 0, esto debería implementarse con una tabla de acumulados anuales
            return 0;
        }
        
        public void UpdateAnnualCopaymentAccumulated(string patientDni, int year, decimal newAmount)
        {
            // Por ahora no implementamos nada, esto debería actualizar una tabla de acumulados anuales
        }
    }
}
