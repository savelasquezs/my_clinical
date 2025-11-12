using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Infrastructure.Config;
using Clinica_Herramientas_2.Infrastructure.Adapters.Output.Persistence;
using System.Linq;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Nurse
{
    [ApiController]
    [Route("api/nurse/visits")]
    public class NurseVisitsController : ControllerBase
    {
        private readonly NurseInputs _nurseInputs;
        private readonly NurseConfig _nurseConfig;
        private readonly ClinicaDbContext _context;

        public NurseVisitsController(NurseInputs nurseInputs, NurseConfig nurseConfig, ClinicaDbContext context)
        {
            _nurseInputs = nurseInputs;
            _nurseConfig = nurseConfig;
            _context = context;
        }

        private User? GetCurrentUserFromHeaders()
        {
            // Intentar obtener el usuario desde los headers
            if (Request.Headers.TryGetValue("X-User-Dni", out var dniHeader))
            {
                var dni = dniHeader.ToString().Trim();
                if (!string.IsNullOrEmpty(dni))
                {
                    return _nurseConfig.UserPort.FindByDocument(dni);
                }
            }
            
            if (Request.Headers.TryGetValue("X-Username", out var usernameHeader))
            {
                var username = usernameHeader.ToString().Trim();
                if (!string.IsNullOrEmpty(username))
                {
                    return _nurseConfig.UserPort.FindByUsername(username);
                }
            }

            return null;
        }

        [HttpPost]
        public IActionResult CreateNurseVisit([FromBody] CreateNurseVisitRequest request)
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
                _nurseInputs.SetCurrentUser(currentUser);

                var patient = _nurseInputs.GetPatientByDni(request.PatientDni);
                if (patient == null)
                {
                    return NotFound(new { message = "Paciente no encontrado." });
                }

                // Obtener el OrderItem real
                var orderItem = _nurseConfig.OrderPort.FindItemByNumber(request.OrderNumber, request.ItemNumber);
                if (orderItem == null)
                {
                    return NotFound(new { message = $"El item {request.ItemNumber} no existe en la orden {request.OrderNumber}." });
                }

                // Validar que el OrderItem es un ProcedureOrderItem con nombre "visita de enfermeria"
                if (orderItem is not ProcedureOrderItem procedureOrderItem)
                {
                    return BadRequest(new { message = "El item debe ser un procedimiento de tipo 'visita de enfermería'." });
                }

                if (procedureOrderItem.Procedure.Name.ToLower() != "visita de enfermeria")
                {
                    return BadRequest(new { message = "El item debe ser un procedimiento de tipo 'visita de enfermería'." });
                }

                // Validar que la orden pertenece al paciente
                var order = _nurseConfig.OrderPort.FindByNumber(request.OrderNumber);
                if (order == null)
                {
                    return NotFound(new { message = $"La orden {request.OrderNumber} no existe." });
                }

                // Obtener medicamentos administrados si se proporcionan
                var administeredMedications = new List<AdministeredMedication>();
                if (request.AdministeredMedications != null && request.AdministeredMedications.Any())
                {
                    foreach (var amRequest in request.AdministeredMedications)
                    {
                        // Obtener el MedicationOrderItem correspondiente
                        var medicationOrderItem = _nurseConfig.OrderPort.FindItemByNumber(request.OrderNumber, amRequest.MedicationOrderItemNumber);
                        if (medicationOrderItem is MedicationOrderItem medItem)
                        {
                            var administeredMedication = new AdministeredMedication(
                                medItem,
                                request.TestsPerformed,
                                request.Notes,
                                DateTime.SpecifyKind(DateTime.Parse(request.PerformedAt), DateTimeKind.Utc),
                                medItem.Medication,
                                amRequest.Dose,
                                amRequest.AdministrationRoute
                            );
                            administeredMedications.Add(administeredMedication);
                        }
                    }
                }

                _nurseInputs.CreateNurseVisit(
                    orderItem,
                    request.TestsPerformed,
                    request.Notes,
                    DateTime.SpecifyKind(DateTime.Parse(request.PerformedAt), DateTimeKind.Utc),
                    request.BloodPressure,
                    request.Temperature,
                    request.Pulse,
                    request.OxygenLevel,
                    administeredMedications,
                    DateTime.SpecifyKind(DateTime.Parse(request.VisitTime), DateTimeKind.Utc),
                    patient
                );

                // Crear PerformedProcedures para los procedimientos marcados como realizados
                if (request.PerformedProcedures != null && request.PerformedProcedures.Any())
                {
                    foreach (var procItemNumber in request.PerformedProcedures)
                    {
                        var procOrderItem = _nurseConfig.OrderPort.FindItemByNumber(request.OrderNumber, procItemNumber);
                        if (procOrderItem is ProcedureOrderItem procItem)
                        {
                            var performedProcedure = new PerformedProcedure(
                                procOrderItem,
                                request.TestsPerformed,
                                request.Notes,
                                DateTime.SpecifyKind(DateTime.Parse(request.PerformedAt), DateTimeKind.Utc)
                            );
                            _context.PerformedProcedures.Add(performedProcedure);
                        }
                    }
                    _context.SaveChanges();
                }

                return Ok(new { message = "Visita de enfermería creada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("patient/{patientDni}")]
        public IActionResult GetPatientInfo(string patientDni)
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
                _nurseInputs.SetCurrentUser(currentUser);

                var patient = _nurseInputs.GetPatientByDni(patientDni);
                if (patient == null)
                {
                    return NotFound(new { message = "Paciente no encontrado." });
                }

                var appointments = _nurseInputs.GetPatientAppointments(patientDni);
                var orders = _nurseInputs.GetPatientOrders(patientDni);

                return Ok(new
                {
                    patient,
                    appointments,
                    orders
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("available-orders")]
        public IActionResult GetAvailableOrders()
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
                _nurseInputs.SetCurrentUser(currentUser);

                var orders = _nurseInputs.GetAvailableOrders();

                // Obtener todos los medical records para mapear pacientes a órdenes
                var orderNumbers = orders.Select(o => o.OrderNumber).ToList();
                var medicalRecords = _context.MedicalRecords
                    .Include(mr => mr.Patient)
                    .Include(mr => mr.Order)
                    .Where(mr => mr.Order != null && orderNumbers.Contains(mr.Order.OrderNumber))
                    .ToList();

                // Mapear a DTOs con información del paciente
                var orderDtos = orders.Select(order =>
                {
                    var medicalRecord = medicalRecords.FirstOrDefault(mr => mr.Order?.OrderNumber == order.OrderNumber);
                    var patient = medicalRecord?.Patient;

                    return new
                    {
                        orderNumber = order.OrderNumber,
                        creationDate = order.CreationDate,
                        itemsCount = order.Items.Count,
                        patientDni = patient?.Dni ?? "",
                        patientName = patient != null ? patient.Fullname : "Paciente no encontrado"
                    };
                }).ToList();

                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("orders/{orderNumber}")]
        public IActionResult GetOrderDetails(int orderNumber)
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
                _nurseInputs.SetCurrentUser(currentUser);

                var order = _nurseInputs.GetOrderDetails(orderNumber);

                // Obtener PerformedProcedures y AdministeredMedications para cada item
                var itemDetails = new List<object>();
                foreach (var item in order.Items)
                {
                    var performedProcedures = _nurseConfig.NurseVisitPort.GetPerformedProceduresByOrderItem(item.OrderNumber, item.ItemNumber);
                    var administeredMedications = _nurseConfig.NurseVisitPort.GetAdministeredMedicationsByOrderItem(item.OrderNumber, item.ItemNumber);

                    object itemDetail;

                    // Crear objeto según el tipo directamente
                    if (item is MedicationOrderItem medItem)
                    {
                        itemDetail = new
                        {
                            orderNumber = item.OrderNumber,
                            itemNumber = item.ItemNumber,
                            cost = item.Cost,
                            itemType = "Medication",
                            medicationId = medItem.Medication.Id,
                            medicationName = medItem.Medication.Name,
                            dose = medItem.Dose,
                            treatmentDuration = medItem.TreatmentDuration,
                            performedProceduresCount = performedProcedures.Count,
                            administeredMedicationsCount = administeredMedications.Count,
                            administeredMedications = administeredMedications.Select(am => new
                            {
                                dose = am.Dose,
                                administrationRoute = am.AdministrationRoute,
                                performedAt = am.PerformedAt
                            }).ToList()
                        };
                    }
                    else if (item is ProcedureOrderItem procItem)
                    {
                        itemDetail = new
                        {
                            orderNumber = item.OrderNumber,
                            itemNumber = item.ItemNumber,
                            cost = item.Cost,
                            itemType = "Procedure",
                            procedureId = procItem.Procedure.Id,
                            procedureName = procItem.Procedure.Name,
                            frequency = procItem.Frequency,
                            requiresSpecialist = procItem.RequiresSpecialist,
                            specialistTypeId = procItem.SpecialistTypeId,
                            performedProceduresCount = performedProcedures.Count,
                            isPerformed = performedProcedures.Any(),
                            administeredMedicationsCount = administeredMedications.Count
                        };
                    }
                    else if (item is DiagnosticAidOrderItem diagItem)
                    {
                        itemDetail = new
                        {
                            orderNumber = item.OrderNumber,
                            itemNumber = item.ItemNumber,
                            cost = item.Cost,
                            itemType = "DiagnosticAid",
                            diagnosticAidId = diagItem.DiagnosticAid.Id,
                            diagnosticAidName = diagItem.DiagnosticAid.Name,
                            quantity = diagItem.Quantity,
                            requiresSpecialist = diagItem.RequiresSpecialist,
                            specialistTypeId = diagItem.SpecialistTypeId,
                            performedProceduresCount = performedProcedures.Count,
                            administeredMedicationsCount = administeredMedications.Count
                        };
                    }
                    else
                    {
                        itemDetail = new
                        {
                            orderNumber = item.OrderNumber,
                            itemNumber = item.ItemNumber,
                            cost = item.Cost,
                            itemType = "Unknown",
                            performedProceduresCount = performedProcedures.Count,
                            administeredMedicationsCount = administeredMedications.Count
                        };
                    }

                    itemDetails.Add(itemDetail);
                }

                // Obtener todos los performedProcedures y administeredMedications de la orden
                var allPerformedProcedures = new List<object>();
                var allAdministeredMedications = new List<object>();

                foreach (var item in order.Items)
                {
                    var performedProcedures = _nurseConfig.NurseVisitPort.GetPerformedProceduresByOrderItem(item.OrderNumber, item.ItemNumber);
                    var administeredMedications = _nurseConfig.NurseVisitPort.GetAdministeredMedicationsByOrderItem(item.OrderNumber, item.ItemNumber);

                    allPerformedProcedures.AddRange(performedProcedures.Select(pp => new
                    {
                        orderNumber = item.OrderNumber,
                        itemNumber = item.ItemNumber,
                        testsPerformed = pp.TestsPerformed,
                        notes = pp.Notes,
                        performedAt = pp.PerformedAt
                    }));

                    allAdministeredMedications.AddRange(administeredMedications.Select(am => new
                    {
                        orderNumber = item.OrderNumber,
                        itemNumber = item.ItemNumber,
                        medicationId = am.Medication?.Id ?? 0,
                        medicationName = am.Medication?.Name ?? "",
                        dose = am.Dose,
                        administrationRoute = am.AdministrationRoute,
                        performedAt = am.PerformedAt
                    }));
                }

                return Ok(new
                {
                    orderNumber = order.OrderNumber,
                    creationDate = order.CreationDate,
                    items = itemDetails,
                    performedProcedures = allPerformedProcedures,
                    administeredMedications = allAdministeredMedications
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CreateNurseVisitRequest
    {
        public string PatientDni { get; set; } = string.Empty;
        public int OrderNumber { get; set; }
        public int ItemNumber { get; set; }
        public decimal Cost { get; set; }
        public string TestsPerformed { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string PerformedAt { get; set; } = string.Empty;
        public string BloodPressure { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public int Pulse { get; set; }
        public int OxygenLevel { get; set; }
        public List<AdministeredMedicationRequest>? AdministeredMedications { get; set; }
        public List<int>? PerformedProcedures { get; set; }
        public string VisitTime { get; set; } = string.Empty;
    }

    public class AdministeredMedicationRequest
    {
        public int MedicationOrderItemNumber { get; set; }
        public string Dose { get; set; } = string.Empty;
        public string AdministrationRoute { get; set; } = string.Empty;
    }
}

