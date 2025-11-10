using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AdminInputs _adminInputs;

        public AppointmentsController(AdminInputs adminInputs)
        {
            _adminInputs = adminInputs;
        }

        [HttpPost]
        public IActionResult CreateAppointment([FromBody] CreateAppointmentRequest request)
        {
            try
            {
                _adminInputs.CreateAppointment(
                    request.Id,
                    request.PatientDni,
                    DateTime.Parse(request.Date)
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

