using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Domain.Model;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Doctor
{
    [ApiController]
    [Route("api/doctor/medical-records")]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly DoctorInputs _doctorInputs;

        public MedicalRecordsController(DoctorInputs doctorInputs)
        {
            _doctorInputs = doctorInputs;
        }

        [HttpPost]
        public IActionResult CreateMedicalRecord([FromBody] CreateMedicalRecordRequest request)
        {
            try
            {
                var patient = _doctorInputs.GetPatientByDni(request.PatientDni);
                if (patient == null)
                {
                    return NotFound(new { message = "Paciente no encontrado." });
                }

                Order? order = null;
                if (request.OrderNumber.HasValue)
                {
                    var orders = _doctorInputs.GetPatientOrders(request.PatientDni);
                    order = orders.FirstOrDefault(o => o.OrderNumber == request.OrderNumber.Value);
                }

                _doctorInputs.CreateMedicalRecord(
                    DateTime.Parse(request.Date),
                    patient,
                    request.ConsultationReason,
                    request.Symptoms,
                    request.Diagnosis,
                    order
                );

                return Ok(new { message = "Registro médico creado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("patient/{patientDni}")]
        public IActionResult GetMedicalHistory(string patientDni)
        {
            try
            {
                var medicalHistory = _doctorInputs.GetMedicalHistory(patientDni);
                return Ok(medicalHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CreateMedicalRecordRequest
    {
        public string PatientDni { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string ConsultationReason { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public int? OrderNumber { get; set; }
    }
}

