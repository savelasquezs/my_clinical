

using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Ports;

// Vamos a crear en el controlador una transacción que englobe la creación de la orden y sus ítems

namespace Clinica_Herramientas_2.Domain.Services
{
    public class CreateOrderItem(IOrderPort orderPort, IInventoryPort inventoryPort)
    {
        private readonly IOrderPort orderPort = orderPort;
        private readonly IInventoryPort inventoryPort = inventoryPort;

        public OrderItem Create(CreateOrderItemDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // Validar máximo 6 dígitos para número de orden
            if (dto.OrderNumber > 999999)
            {
                throw new Exception("El número de orden no puede tener más de 6 dígitos.");
            }

            // Validar que la orden existe
            _ = orderPort.FindByNumber(dto.OrderNumber) ?? throw new Exception("La orden no existe.");

            // Generar itemNumber automáticamente (siguiente número disponible)
            int nextItemNumber = GetNextItemNumber(dto.OrderNumber);

            // Crear el OrderItem según el tipo
            return dto.ItemType switch
            {
                OrderItemType.Medication => CreateMedicationOrderItem(dto, nextItemNumber),
                OrderItemType.Procedure => CreateProcedureOrderItem(dto, nextItemNumber),
                OrderItemType.DiagnosticAid => CreateDiagnosticAidOrderItem(dto, nextItemNumber),
                _ => throw new ArgumentException($"Tipo de ítem no válido: {dto.ItemType}")
            };
        }

        private int GetNextItemNumber(int orderNumber)
        {
            // Buscar el siguiente itemNumber disponible
            int itemNumber = 1;
            while (orderPort.ItemExists(orderNumber, itemNumber))
            {
                itemNumber++;
            }
            return itemNumber;
        }

        private MedicationOrderItem CreateMedicationOrderItem(CreateOrderItemDTO dto, int itemNumber)
        {
            if (!dto.MedicationId.HasValue)
            {
                throw new ArgumentException("MedicationId es requerido para medicamentos.");
            }

            var medication = inventoryPort.FindMedicationById(dto.MedicationId.Value);
            return medication == null
                ? throw new Exception("El medicamento no existe en el inventario.")
                : new MedicationOrderItem(
                dto.OrderNumber,
                itemNumber,
                dto.Cost,
                medication,
                dto.Dose ?? medication.Dose,
                dto.TreatmentDuration ?? medication.TreatmentDuration
            );
        }

        private ProcedureOrderItem CreateProcedureOrderItem(CreateOrderItemDTO dto, int itemNumber)
        {
            if (!dto.ProcedureId.HasValue)
            {
                throw new ArgumentException("ProcedureId es requerido para procedimientos.");
            }

            var procedure = inventoryPort.FindProcedureById(dto.ProcedureId.Value);
            return procedure == null
                ? throw new Exception("El procedimiento no existe en el inventario.")
                : new ProcedureOrderItem(
                dto.OrderNumber,
                itemNumber,
                dto.Cost,
                procedure,
                dto.Frequency ?? procedure.Frequency,
                dto.RequiresSpecialist ?? procedure.RequiresSpecialist,
                dto.SpecialistTypeId
            );
        }

        private DiagnosticAidOrderItem CreateDiagnosticAidOrderItem(CreateOrderItemDTO dto, int itemNumber)
        {
            if (!dto.DiagnosticAidId.HasValue)
            {
                throw new ArgumentException("DiagnosticAidId es requerido para ayudas diagnósticas.");
            }

            var diagnosticAid = inventoryPort.FindDiagnosticAidById(dto.DiagnosticAidId.Value);
            return diagnosticAid == null
                ? throw new Exception("La ayuda diagnóstica no existe en el inventario.")
                : new DiagnosticAidOrderItem(
                dto.OrderNumber,
                itemNumber,
                dto.Cost,
                diagnosticAid,
                dto.Quantity ?? diagnosticAid.Quantity,
                dto.RequiresSpecialist ?? diagnosticAid.RequiresSpecialist,
                dto.SpecialistTypeId
            );
        }
    }
}
