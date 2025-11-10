using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Infrastructure.Config;

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

                var order = _doctorInputs.CreateOrder(
                    request.OrderNumber,
                    DateTime.SpecifyKind(DateTime.Parse(request.CreationDate), DateTimeKind.Utc)
                );

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{orderNumber}/items/medication")]
        public IActionResult AddMedicationToOrder(int orderNumber, [FromBody] AddMedicationRequest request)
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

                // Necesitamos obtener la orden - por ahora asumimos que viene en el request
                // En una implementación real, deberíamos obtenerla del repositorio
                var order = new Order(orderNumber, DateTime.Now, new List<OrderItem>());
                
                // Necesitamos obtener el Medication desde el inventario
                // Por ahora, creamos uno temporal con el ID proporcionado
                var medication = new Medication(request.MedicationId, "", 0, "", 0);

                _doctorInputs.AddMedicationToOrder(
                    order,
                    request.ItemNumber,
                    request.Cost,
                    medication,
                    request.Dose,
                    request.TreatmentDuration
                );

                return Ok(new { message = "Medicamento agregado a la orden exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{orderNumber}/items/procedure")]
        public IActionResult AddProcedureToOrder(int orderNumber, [FromBody] AddProcedureRequest request)
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

                var order = new Order(orderNumber, DateTime.Now, new List<OrderItem>());
                var procedure = new Procedure(request.ProcedureId, "", 0, 0, false, null);

                _doctorInputs.AddProcedureToOrder(
                    order,
                    request.ItemNumber,
                    request.Cost,
                    procedure,
                    request.Frequency,
                    request.RequiresSpecialist,
                    request.SpecialistTypeId
                );

                return Ok(new { message = "Procedimiento agregado a la orden exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{orderNumber}/items/diagnostic-aid")]
        public IActionResult AddDiagnosticAidToOrder(int orderNumber, [FromBody] AddDiagnosticAidRequest request)
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

                var order = new Order(orderNumber, DateTime.Now, new List<OrderItem>());
                var diagnosticAid = new DiagnosticAid(request.DiagnosticAidId, "", 0, 0, false, null);

                _doctorInputs.AddDiagnosticAidToOrder(
                    order,
                    request.ItemNumber,
                    request.Cost,
                    diagnosticAid,
                    request.Quantity,
                    request.RequiresSpecialist,
                    request.SpecialistTypeId
                );

                return Ok(new { message = "Ayuda diagnóstica agregada a la orden exitosamente." });
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
                // Obtener el usuario actual desde los headers
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                // Establecer el usuario actual en el use case
                _doctorInputs.SetCurrentUser(currentUser);

                var orders = _doctorInputs.GetPatientOrders(patientDni);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CreateOrderRequest
    {
        public int OrderNumber { get; set; }
        public string CreationDate { get; set; } = string.Empty;
    }

    public class AddMedicationRequest
    {
        public string PatientDni { get; set; } = string.Empty;
        public int ItemNumber { get; set; }
        public decimal Cost { get; set; }
        public int MedicationId { get; set; }
        public string Dose { get; set; } = string.Empty;
        public int TreatmentDuration { get; set; }
    }

    public class AddProcedureRequest
    {
        public string PatientDni { get; set; } = string.Empty;
        public int ItemNumber { get; set; }
        public decimal Cost { get; set; }
        public int ProcedureId { get; set; }
        public int Frequency { get; set; }
        public bool RequiresSpecialist { get; set; }
        public int? SpecialistTypeId { get; set; }
    }

    public class AddDiagnosticAidRequest
    {
        public string PatientDni { get; set; } = string.Empty;
        public int ItemNumber { get; set; }
        public decimal Cost { get; set; }
        public int DiagnosticAidId { get; set; }
        public int Quantity { get; set; }
        public bool RequiresSpecialist { get; set; }
        public int? SpecialistTypeId { get; set; }
    }
}

