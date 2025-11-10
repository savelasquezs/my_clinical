using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Infrastructure.Config;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AdminInputs _adminInputs;
        private readonly AdminConfig _adminConfig;

        public AppointmentsController(AdminInputs adminInputs, AdminConfig adminConfig)
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

        [HttpPost]
        public IActionResult CreateAppointment([FromBody] CreateAppointmentRequest request)
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

                _adminInputs.CreateAppointment(
                    request.Id,
                    request.PatientDni,
                    DateTime.SpecifyKind(DateTime.Parse(request.Date), DateTimeKind.Utc)
                );

                return Ok(new { message = "Cita creada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("patient/{patientDni}")]
        public IActionResult GetPatientAppointments(string patientDni)
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

                var appointments = _adminInputs.GetPatientAppointments(patientDni);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CreateAppointmentRequest
    {
        public int Id { get; set; }
        public string PatientDni { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }
}

