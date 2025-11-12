using Microsoft.EntityFrameworkCore;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Ports;
using System.Linq;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Output.Persistence
{
    internal class PostgresOrderPort : IOrderPort
    {
        private readonly ClinicaDbContext context;
        
        public PostgresOrderPort(ClinicaDbContext context)
        {
            this.context = context;
        }
        
        public Order? FindByOrderNumber(int orderNumber)
        {
            var order = context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.OrderNumber == orderNumber);
            
            if (order == null) return null;
            
            // Cargar las relaciones específicas de cada tipo de item
            // Cargar MedicationOrderItems con Medication
            var medicationItems = context.Set<MedicationOrderItem>()
                .Include(m => m.Medication)
                .Where(i => i.OrderNumber == orderNumber)
                .ToList();
            
            // Cargar ProcedureOrderItems con Procedure
            var procedureItems = context.Set<ProcedureOrderItem>()
                .Include(p => p.Procedure)
                .Where(i => i.OrderNumber == orderNumber)
                .ToList();
            
            // Cargar DiagnosticAidOrderItems con DiagnosticAid
            var diagnosticAidItems = context.Set<DiagnosticAidOrderItem>()
                .Include(d => d.DiagnosticAid)
                .Where(i => i.OrderNumber == orderNumber)
                .ToList();
            
            // Reemplazar los items base con los items específicos que tienen las relaciones cargadas
            // Esto es necesario porque EF Core no carga automáticamente las relaciones en TPT
            var allItems = new List<OrderItem>();
            allItems.AddRange(medicationItems);
            allItems.AddRange(procedureItems);
            allItems.AddRange(diagnosticAidItems);
            
            // Actualizar la colección de items de la orden
            order.Items.Clear();
            foreach (var item in allItems)
            {
                order.Items.Add(item);
            }
            
            return order;
        }
        
        public void Save(Order order)
        {
            context.Orders.Add(order);
            context.SaveChanges();
        }
        
        public void Update(Order order)
        {
            context.Orders.Update(order);
            context.SaveChanges();
        }
        
        public void Delete(Order order)
        {
            context.Orders.Remove(order);
            context.SaveChanges();
        }
        
        public List<Order> FindAll()
        {
            return context.Orders
                .Include(o => o.Items)
                .ToList();
        }
        
        public List<Order> FindByDateRange(DateTime startDate, DateTime endDate)
        {
            return context.Orders
                .Include(o => o.Items)
                .Where(o => o.CreationDate >= startDate && o.CreationDate <= endDate)
                .ToList();
        }
        
        public List<Order> FindByPatient(string patientDni)
        {
            return context.Orders
                .Include(o => o.Items)
                .Where(o => o.Items.Any(i => i.OrderNumber == o.OrderNumber))
                .ToList();
        }
        
        public Order? FindByNumber(int orderNumber)
        {
            return FindByOrderNumber(orderNumber);
        }
        
        public List<Order> FindByPatientDni(string patientDni)
        {
            return FindByPatient(patientDni);
        }
        
        public bool ItemExists(int orderNumber, int itemNumber)
        {
            return context.OrderItems
                .Any(i => i.OrderNumber == orderNumber && i.ItemNumber == itemNumber);
        }

        public OrderItem? FindItemByNumber(int orderNumber, int itemNumber)
        {
            // Intentar encontrar como MedicationOrderItem
            var medicationItem = context.Set<MedicationOrderItem>()
                .Include(m => m.Medication)
                .FirstOrDefault(i => i.OrderNumber == orderNumber && i.ItemNumber == itemNumber);
            if (medicationItem != null) return medicationItem;

            // Intentar encontrar como ProcedureOrderItem
            var procedureItem = context.Set<ProcedureOrderItem>()
                .Include(p => p.Procedure)
                .FirstOrDefault(i => i.OrderNumber == orderNumber && i.ItemNumber == itemNumber);
            if (procedureItem != null) return procedureItem;

            // Intentar encontrar como DiagnosticAidOrderItem
            var diagnosticAidItem = context.Set<DiagnosticAidOrderItem>()
                .Include(d => d.DiagnosticAid)
                .FirstOrDefault(i => i.OrderNumber == orderNumber && i.ItemNumber == itemNumber);
            if (diagnosticAidItem != null) return diagnosticAidItem;

            return null;
        }

        public void RemoveItem(int orderNumber, int itemNumber)
        {
            // Buscar el item
            var item = FindItemByNumber(orderNumber, itemNumber);
            if (item == null)
            {
                throw new Exception($"El item {itemNumber} no existe en la orden {orderNumber}.");
            }

            // Obtener la orden para remover el item de la lista
            var order = FindByNumber(orderNumber);
            if (order == null)
            {
                throw new Exception($"La orden {orderNumber} no existe.");
            }

            // Remover el item de la orden
            order.Items.Remove(item);

            // Eliminar el item de la base de datos según su tipo
            if (item is MedicationOrderItem medicationItem)
            {
                context.Set<MedicationOrderItem>().Remove(medicationItem);
            }
            else if (item is ProcedureOrderItem procedureItem)
            {
                context.Set<ProcedureOrderItem>().Remove(procedureItem);
            }
            else if (item is DiagnosticAidOrderItem diagnosticAidItem)
            {
                context.Set<DiagnosticAidOrderItem>().Remove(diagnosticAidItem);
            }

            // Actualizar la orden
            context.Orders.Update(order);
            context.SaveChanges();
        }

        public OrderItem Create(CreateOrderItemDTO dto)
        {
            // Esta implementación es básica, debería crear el OrderItem según el tipo
            throw new NotImplementedException("Create method should be implemented based on OrderItem type");
        }

        public List<Order> FindOrdersWithNurseVisitProcedure()
        {
            // Obtener todas las facturas para extraer orderNumbers facturadas
            var invoicedOrderNumbers = context.Invoices
                .Include(i => i.Orders)
                .SelectMany(i => i.Orders)
                .Select(o => o.OrderNumber)
                .Distinct()
                .ToList();

            // Obtener todas las órdenes que tienen ProcedureOrderItem con nombre "visita de enfermeria" (case-insensitive)
            var ordersWithNurseVisit = context.Orders
                .Include(o => o.Items)
                .Where(o => context.Set<ProcedureOrderItem>()
                    .Include(p => p.Procedure)
                    .Any(poi => poi.OrderNumber == o.OrderNumber && 
                               poi.Procedure.Name.ToLower() == "visita de enfermeria"))
                .Where(o => !invoicedOrderNumbers.Contains(o.OrderNumber))
                .ToList();

            // Cargar relaciones completas de items para cada orden
            foreach (var order in ordersWithNurseVisit)
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

            return ordersWithNurseVisit;
        }

        public bool IsMedicationInUse(int medicationId)
        {
            return context.Set<MedicationOrderItem>()
                .Any(m => EF.Property<int>(m, "medication_id") == medicationId);
        }

        public bool IsProcedureInUse(int procedureId)
        {
            return context.Set<ProcedureOrderItem>()
                .Any(p => EF.Property<int>(p, "procedure_id") == procedureId);
        }

        public bool IsDiagnosticAidInUse(int diagnosticAidId)
        {
            return context.Set<DiagnosticAidOrderItem>()
                .Any(d => EF.Property<int>(d, "diagnostic_aid_id") == diagnosticAidId);
        }
    }
}
