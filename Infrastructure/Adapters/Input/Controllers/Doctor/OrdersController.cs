using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Infrastructure.Config;
using System.Linq;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Doctor
{
    [ApiController]
    [Route("api/doctor/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly DoctorInputs _doctorInputs;
        private readonly DoctorConfig _doctorConfig;

        public OrdersController(DoctorInputs doctorInputs, DoctorConfig doctorConfig)
        {
            _doctorInputs = doctorInputs;
            _doctorConfig = doctorConfig;
        }

        private User? GetCurrentUserFromHeaders()
        {
            // Intentar obtener el usuario desde los headers
            if (Request.Headers.TryGetValue("X-User-Dni", out var dniHeader))
            {
                var dni = dniHeader.ToString().Trim();
                if (!string.IsNullOrEmpty(dni))
                {
                    return _doctorConfig.UserPort.FindByDocument(dni);
                }
            }
            
            if (Request.Headers.TryGetValue("X-Username", out var usernameHeader))
            {
                var username = usernameHeader.ToString().Trim();
                if (!string.IsNullOrEmpty(username))
                {
                    return _doctorConfig.UserPort.FindByUsername(username);
                }
            }

            return null;
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                // Obtener el usuario actual desde los headers
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                // Establecer el usuario actual en el use case
                _doctorInputs.SetCurrentUser(currentUser);

                // Validar que haya al menos un item
                if (request.Items == null || request.Items.Count == 0)
                {
                    return BadRequest(new { message = "La orden debe tener al menos un item." });
                }

                // Crear la orden
                var order = _doctorInputs.CreateOrder(
                    request.OrderNumber,
                    DateTime.SpecifyKind(DateTime.Parse(request.CreationDate), DateTimeKind.Utc)
                );

                // Agregar los items usando AddOrderItemService
                foreach (var itemRequest in request.Items)
                {
                    itemRequest.OrderNumber = request.OrderNumber;
                    var itemDto = itemRequest.ToCreateOrderItemDTO();
                    _doctorConfig.AddOrderItemService.AddItem(itemDto);
                }

                // Obtener la orden completa con items
                var completeOrder = _doctorInputs.GetOrderByNumber(request.OrderNumber);
                return Ok(MapOrderToDto(completeOrder));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{orderNumber}")]
        public IActionResult GetOrderByNumber(int orderNumber)
        {
            try
            {
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                _doctorInputs.SetCurrentUser(currentUser);

                var order = _doctorInputs.GetOrderByNumber(orderNumber);
                return Ok(MapOrderToDto(order));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{orderNumber}/items")]
        public IActionResult AddOrderItem(int orderNumber, [FromBody] CreateOrderItemRequest itemRequest)
        {
            try
            {
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                _doctorInputs.SetCurrentUser(currentUser);

                itemRequest.OrderNumber = orderNumber;
                var itemDto = itemRequest.ToCreateOrderItemDTO();
                _doctorConfig.AddOrderItemService.AddItem(itemDto);

                return Ok(new { message = "Item agregado a la orden exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{orderNumber}/items/{itemNumber}")]
        public IActionResult UpdateOrderItem(int orderNumber, int itemNumber, [FromBody] CreateOrderItemRequest itemRequest)
        {
            try
            {
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                _doctorInputs.SetCurrentUser(currentUser);

                itemRequest.OrderNumber = orderNumber;
                var itemDto = itemRequest.ToCreateOrderItemDTO();
                _doctorInputs.UpdateOrderItem(itemDto, itemNumber);

                return Ok(new { message = "Item actualizado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{orderNumber}/items/{itemNumber}")]
        public IActionResult DeleteOrderItem(int orderNumber, int itemNumber)
        {
            try
            {
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                _doctorInputs.SetCurrentUser(currentUser);

                _doctorInputs.RemoveOrderItem(orderNumber, itemNumber);

                return Ok(new { message = "Item eliminado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("patient/{patientDni}")]
        public IActionResult GetPatientOrders(string patientDni)
        {
            try
            {
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                _doctorInputs.SetCurrentUser(currentUser);

                var orders = _doctorInputs.GetPatientOrders(patientDni);
                var orderDtos = orders.Select(o => MapOrderToDto(o)).ToList();
                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{orderNumber}/nurse-visits")]
        public IActionResult GetNurseVisitsByOrder(int orderNumber)
        {
            try
            {
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                _doctorInputs.SetCurrentUser(currentUser);

                var nurseVisits = _doctorConfig.NurseVisitPort.FindByOrderNumber(orderNumber);

                var nurseVisitDtos = nurseVisits.Select(nv => new
                {
                    id = nv.Id,
                    visitTime = nv.VisitTime,
                    nurseName = nv.Nurse?.Fullname ?? "Enfermera no encontrada",
                    patientName = nv.Patient?.Fullname ?? "Paciente no encontrado",
                    vitalData = nv.VitalData != null ? new
                    {
                        bloodPressure = nv.VitalData.BloodPressure,
                        temperature = nv.VitalData.Temperature,
                        pulse = nv.VitalData.Pulse,
                        oxygenLevel = nv.VitalData.OxygenLevel
                    } : null,
                    testsPerformed = nv.TestsPerformed,
                    notes = nv.Notes,
                    administeredMedications = nv.AdministeredMedications?.Select(am => new
                    {
                        medicationName = am.Medication?.Name ?? "Medicamento no encontrado",
                        dose = am.Dose,
                        administrationRoute = am.AdministrationRoute,
                        performedAt = am.PerformedAt
                    }).ToList()
                }).ToList();

                return Ok(nurseVisitDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private object MapOrderToDto(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "La orden no puede ser null");
            }

            return new
            {
                orderNumber = order.OrderNumber,
                creationDate = order.CreationDate,
                items = order.Items?.Select(item => MapOrderItemToDto(item)).ToList() ?? new List<object>()
            };
        }

        private object MapOrderItemToDto(OrderItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El item no puede ser null");
            }

            var baseDto = new
            {
                orderNumber = item.OrderNumber,
                itemNumber = item.ItemNumber,
                cost = item.Cost
            };

            return item switch
            {
                MedicationOrderItem medicationItem => new
                {
                    baseDto.orderNumber,
                    baseDto.itemNumber,
                    baseDto.cost,
                    itemType = "Medication",
                    medicationId = medicationItem.Medication?.Id ?? 0,
                    medicationName = medicationItem.Medication?.Name ?? "Medicamento no encontrado",
                    dose = medicationItem.Dose ?? string.Empty,
                    treatmentDuration = medicationItem.TreatmentDuration
                },
                ProcedureOrderItem procedureItem => new
                {
                    baseDto.orderNumber,
                    baseDto.itemNumber,
                    baseDto.cost,
                    itemType = "Procedure",
                    procedureId = procedureItem.Procedure?.Id ?? 0,
                    procedureName = procedureItem.Procedure?.Name ?? "Procedimiento no encontrado",
                    frequency = procedureItem.Frequency,
                    requiresSpecialist = procedureItem.RequiresSpecialist,
                    specialistTypeId = procedureItem.SpecialistTypeId
                },
                DiagnosticAidOrderItem diagnosticAidItem => new
                {
                    baseDto.orderNumber,
                    baseDto.itemNumber,
                    baseDto.cost,
                    itemType = "DiagnosticAid",
                    diagnosticAidId = diagnosticAidItem.DiagnosticAid?.Id ?? 0,
                    diagnosticAidName = diagnosticAidItem.DiagnosticAid?.Name ?? "Ayuda diagnóstica no encontrada",
                    quantity = diagnosticAidItem.Quantity,
                    requiresSpecialist = diagnosticAidItem.RequiresSpecialist,
                    specialistTypeId = diagnosticAidItem.SpecialistTypeId
                },
                _ => baseDto
            };
        }
    }

    public class CreateOrderRequest
    {
        public int OrderNumber { get; set; }
        public string CreationDate { get; set; } = string.Empty;
        public List<CreateOrderItemRequest> Items { get; set; } = new List<CreateOrderItemRequest>();
    }

    public class CreateOrderItemRequest
    {
        public int OrderNumber { get; set; }
        public decimal Cost { get; set; }
        public string ItemType { get; set; } = string.Empty; // String para facilitar el binding desde JSON
        
        // Medication specific
        public int? MedicationId { get; set; }
        public string? Dose { get; set; }
        public int? TreatmentDuration { get; set; }
        
        // Procedure specific
        public int? ProcedureId { get; set; }
        public int? Frequency { get; set; }
        public bool? RequiresSpecialist { get; set; }
        public int? SpecialistTypeId { get; set; }
        
        // DiagnosticAid specific
        public int? DiagnosticAidId { get; set; }
        public int? Quantity { get; set; }

        public CreateOrderItemDTO ToCreateOrderItemDTO()
        {
            if (!Enum.TryParse<OrderItemType>(ItemType, ignoreCase: true, out var itemType))
            {
                throw new ArgumentException($"Tipo de ítem no válido: {ItemType}. Debe ser uno de: Medication, Procedure, DiagnosticAid");
            }

            return new CreateOrderItemDTO
            {
                OrderNumber = OrderNumber,
                Cost = Cost,
                ItemType = itemType,
                MedicationId = MedicationId,
                Dose = Dose,
                TreatmentDuration = TreatmentDuration,
                ProcedureId = ProcedureId,
                Frequency = Frequency,
                RequiresSpecialist = RequiresSpecialist,
                SpecialistTypeId = SpecialistTypeId,
                DiagnosticAidId = DiagnosticAidId,
                Quantity = Quantity
            };
        }
    }
}
