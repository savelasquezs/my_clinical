using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Ports;

namespace Clinica_Herramientas_2.Domain.Services
{
    public class UpdateOrderItem(IOrderPort orderPort, IInventoryPort inventoryPort, OrderRulesService orderRulesService)
    {
        private readonly IOrderPort orderPort = orderPort;
        private readonly IInventoryPort inventoryPort = inventoryPort;
        private readonly OrderRulesService orderRulesService = orderRulesService;

        public void Update(CreateOrderItemDTO dto, int itemNumber)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // Validar que la orden existe
            var order = orderPort.FindByNumber(dto.OrderNumber) ?? throw new Exception("La orden no existe.");

            // Buscar el item a actualizar
            var existingItem = orderPort.FindItemByNumber(dto.OrderNumber, itemNumber) 
                ?? throw new Exception($"El item {itemNumber} no existe en la orden {dto.OrderNumber}.");

            // Actualizar el item según su tipo
            switch (dto.ItemType)
            {
                case OrderItemType.Medication:
                    if (existingItem is not MedicationOrderItem medicationItem)
                    {
                        throw new Exception("El item no es del tipo Medication.");
                    }
                    if (!dto.MedicationId.HasValue)
                    {
                        throw new ArgumentException("MedicationId es requerido para medicamentos.");
                    }
                    var medication = inventoryPort.FindMedicationById(dto.MedicationId.Value)
                        ?? throw new Exception("El medicamento no existe en el inventario.");
                    medicationItem.Update(
                        dto.Cost,
                        medication,
                        dto.Dose ?? medication.Dose,
                        dto.TreatmentDuration ?? medication.TreatmentDuration
                    );
                    break;

                case OrderItemType.Procedure:
                    if (existingItem is not ProcedureOrderItem procedureItem)
                    {
                        throw new Exception("El item no es del tipo Procedure.");
                    }
                    if (!dto.ProcedureId.HasValue)
                    {
                        throw new ArgumentException("ProcedureId es requerido para procedimientos.");
                    }
                    var procedure = inventoryPort.FindProcedureById(dto.ProcedureId.Value)
                        ?? throw new Exception("El procedimiento no existe en el inventario.");
                    procedureItem.Update(
                        dto.Cost,
                        procedure,
                        dto.Frequency ?? procedure.Frequency,
                        dto.RequiresSpecialist ?? procedure.RequiresSpecialist,
                        dto.SpecialistTypeId
                    );
                    break;

                case OrderItemType.DiagnosticAid:
                    if (existingItem is not DiagnosticAidOrderItem diagnosticAidItem)
                    {
                        throw new Exception("El item no es del tipo DiagnosticAid.");
                    }
                    if (!dto.DiagnosticAidId.HasValue)
                    {
                        throw new ArgumentException("DiagnosticAidId es requerido para ayudas diagnósticas.");
                    }
                    var diagnosticAid = inventoryPort.FindDiagnosticAidById(dto.DiagnosticAidId.Value)
                        ?? throw new Exception("La ayuda diagnóstica no existe en el inventario.");
                    diagnosticAidItem.Update(
                        dto.Cost,
                        diagnosticAid,
                        dto.Quantity ?? diagnosticAid.Quantity,
                        dto.RequiresSpecialist ?? diagnosticAid.RequiresSpecialist,
                        dto.SpecialistTypeId
                    );
                    break;

                default:
                    throw new ArgumentException($"Tipo de ítem no válido: {dto.ItemType}");
            }

            // Validar reglas cross-item después de actualizar
            OrderRulesService.ValidateOrder(order);

            // Actualizar la orden en la base de datos
            orderPort.Update(order);
        }
    }
}

