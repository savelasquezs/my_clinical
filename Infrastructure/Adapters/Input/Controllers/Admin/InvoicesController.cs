using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Infrastructure.Config;
using System.Linq;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/invoices")]
    public class InvoicesController : ControllerBase
    {
        private readonly AdminInputs _adminInputs;
        private readonly AdminConfig _adminConfig;

        public InvoicesController(AdminInputs adminInputs, AdminConfig adminConfig)
        {
            _adminInputs = adminInputs;
            _adminConfig = adminConfig;
        }

        private User? GetCurrentUserFromHeaders()
        {
            // Intentar obtener el usuario desde los headers
            if (Request.Headers.TryGetValue("X-User-Dni", out var dniHeader))
            {
                var dni = dniHeader.ToString().Trim();
                if (!string.IsNullOrEmpty(dni))
                {
                    return _adminConfig.UserPort.FindByDocument(dni);
                }
            }
            
            if (Request.Headers.TryGetValue("X-Username", out var usernameHeader))
            {
                var username = usernameHeader.ToString().Trim();
                if (!string.IsNullOrEmpty(username))
                {
                    return _adminConfig.UserPort.FindByUsername(username);
                }
            }

            return null;
        }

        [HttpGet]
        public IActionResult GetAllInvoices()
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
                _adminInputs.SetCurrentUser(currentUser);

                var invoices = _adminInputs.GetAllInvoices();
                var invoiceDtos = invoices.Select(i => MapInvoiceToDto(i)).ToList();

                return Ok(invoiceDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{invoiceNumber}")]
        public IActionResult GetInvoiceByNumber(int invoiceNumber)
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
                _adminInputs.SetCurrentUser(currentUser);

                var invoices = _adminInputs.GetAllInvoices();
                var invoice = invoices.FirstOrDefault(i => i.InvoiceNumber == invoiceNumber);

                if (invoice == null)
                {
                    return NotFound(new { message = "Factura no encontrada." });
                }

                return Ok(MapInvoiceToDto(invoice));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CreateInvoice([FromBody] CreateInvoiceRequest request)
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
                _adminInputs.SetCurrentUser(currentUser);

                var invoice = _adminInputs.CreateInvoice(
                    request.InvoiceNumber,
                    request.PatientDni,
                    request.DoctorDni,
                    request.OrderNumbers,
                    DateTime.SpecifyKind(DateTime.Parse(request.InvoiceDate), DateTimeKind.Utc)
                );

                return Ok(MapInvoiceToDto(invoice));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private object MapInvoiceToDto(Invoice invoice)
        {
            if (invoice == null)
            {
                throw new ArgumentNullException(nameof(invoice), "La factura no puede ser null");
            }

            var patient = invoice.Patient;
            var doctor = invoice.Doctor;
            var insurance = patient.Insurance;
            
            // Calcular edad del paciente
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - patient.Birthdate.Year;
            if (patient.Birthdate > today.AddYears(-age)) age--;

            // Calcular días de vigencia de la póliza
            int? daysUntilExpiration = null;
            if (insurance != null)
            {
                var expirationDateOnly = DateOnly.FromDateTime(insurance.ExpirationDate);
                daysUntilExpiration = expirationDateOnly.DayNumber - today.DayNumber;
            }

            return new
            {
                invoiceNumber = invoice.InvoiceNumber,
                invoiceDate = invoice.InvoiceDate,
                // Información del paciente (REQ-ADMIN-020)
                patient = new
                {
                    name = patient.Fullname,
                    age = age,
                    dni = patient.Dni
                },
                // Información del médico (REQ-ADMIN-021)
                doctor = new
                {
                    name = doctor.Fullname,
                    dni = doctor.Dni
                },
                // Información del seguro (REQ-ADMIN-022, REQ-ADMIN-023, REQ-ADMIN-024, REQ-ADMIN-025)
                insurance = insurance != null ? new
                {
                    companyName = insurance.CompanyName,
                    policyNumber = insurance.PolicyNumber,
                    isActive = insurance.IsActive,
                    expirationDate = insurance.ExpirationDate,
                    daysUntilExpiration = daysUntilExpiration
                } : null,
                // Información de órdenes (REQ-ADMIN-026)
                orders = invoice.Orders?.Select(o => MapOrderToDto(o)).ToList() ?? new List<object>(),
                // Montos (REQ-ADMIN-030, REQ-ADMIN-031, REQ-ADMIN-032)
                amounts = new
                {
                    totalAmount = invoice.TotalAmount,
                    copaymentAmount = invoice.CopaymentAmount,
                    insuranceAmount = invoice.InsuranceAmount,
                    annualCopaymentAccumulated = invoice.AnnualCopaymentAccumulated
                }
            };
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
                // REQ-ADMIN-027: Medicamentos con nombre, costo y dosis
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
                // REQ-ADMIN-028: Procedimientos con nombre
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
                // REQ-ADMIN-029: Ayudas diagnósticas con nombre
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

    public class CreateInvoiceRequest
    {
        public int InvoiceNumber { get; set; }
        public string PatientDni { get; set; } = string.Empty;
        public string DoctorDni { get; set; } = string.Empty;
        public List<int> OrderNumbers { get; set; } = new();
        public string InvoiceDate { get; set; } = string.Empty;
    }
}

