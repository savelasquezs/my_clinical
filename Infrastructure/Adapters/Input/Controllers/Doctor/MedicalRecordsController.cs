using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Infrastructure.Config;
using System.Linq;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Doctor
{
    [ApiController]
    [Route("api/doctor/medical-records")]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly DoctorInputs _doctorInputs;
        private readonly DoctorConfig _doctorConfig;

        public MedicalRecordsController(DoctorInputs doctorInputs, DoctorConfig doctorConfig)
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
        public IActionResult CreateMedicalRecord([FromBody] CreateMedicalRecordRequest request)
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
                    DateTime.SpecifyKind(DateTime.Parse(request.Date), DateTimeKind.Utc),
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

        [HttpGet]
        public IActionResult GetAllMedicalRecords()
        {
            try
            {
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                _doctorInputs.SetCurrentUser(currentUser);

                var medicalRecords = _doctorConfig.MedicalRecordPort.FindAll();
                var medicalRecordDtos = medicalRecords.Select(mr => new
                {
                    id = mr.Id,
                    date = mr.Date,
                    patientDni = mr.Patient.Dni,
                    patientName = mr.Patient.Fullname,
                    doctorDni = mr.Doctor.Dni,
                    doctorName = mr.Doctor.Fullname,
                    consultationReason = mr.ConsultationReason,
                    symptoms = mr.Symptoms,
                    diagnosis = mr.Diagnosis,
                    orderNumber = mr.Order?.OrderNumber
                }).ToList();

                return Ok(medicalRecordDtos);
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
                // Obtener el usuario actual desde los headers
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                // Establecer el usuario actual en el use case
                _doctorInputs.SetCurrentUser(currentUser);

                var medicalHistory = _doctorInputs.GetMedicalHistory(patientDni);
                return Ok(medicalHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMedicalRecord(int id, [FromBody] UpdateMedicalRecordRequest request)
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

                _doctorInputs.UpdateMedicalRecord(
                    id,
                    DateTime.SpecifyKind(DateTime.Parse(request.Date), DateTimeKind.Utc),
                    request.ConsultationReason,
                    request.Symptoms,
                    request.Diagnosis
                );

                // Obtener el registro actualizado para retornarlo
                var updatedRecord = _doctorConfig.MedicalRecordPort.FindById(id);
                if (updatedRecord == null)
                {
                    return NotFound(new { message = "Registro médico no encontrado." });
                }

                var medicalRecordDto = new
                {
                    id = updatedRecord.Id,
                    date = updatedRecord.Date,
                    patientDni = updatedRecord.Patient.Dni,
                    patientName = updatedRecord.Patient.Fullname,
                    doctorDni = updatedRecord.Doctor.Dni,
                    doctorName = updatedRecord.Doctor.Fullname,
                    consultationReason = updatedRecord.ConsultationReason,
                    symptoms = updatedRecord.Symptoms,
                    diagnosis = updatedRecord.Diagnosis,
                    orderNumber = updatedRecord.Order?.OrderNumber
                };

                return Ok(medicalRecordDto);
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

    public class UpdateMedicalRecordRequest
    {
        public string Date { get; set; } = string.Empty;
        public string ConsultationReason { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
    }
}

