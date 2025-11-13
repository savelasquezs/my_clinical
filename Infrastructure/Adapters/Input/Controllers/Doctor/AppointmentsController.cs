using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Infrastructure.Config;
using System.Linq;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Doctor
{
    [ApiController]
    [Route("api/doctor/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly DoctorInputs _doctorInputs;
        private readonly DoctorConfig _doctorConfig;

        public AppointmentsController(DoctorInputs doctorInputs, DoctorConfig doctorConfig)
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

        [HttpGet("available")]
        public IActionResult GetAvailableAppointments()
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

                var appointments = _doctorInputs.GetAvailableAppointments();
                var appointmentDtos = appointments.Select(a => new
                {
                    id1 = a.Id1,
                    patientDni = a.Patient1?.Dni ?? string.Empty,
                    patientName = a.Patient1?.Fullname ?? string.Empty,
                    date1 = a.Date1,
                    isAccepted1 = a.IsAccepted1
                }).ToList();
                return Ok(appointmentDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}


