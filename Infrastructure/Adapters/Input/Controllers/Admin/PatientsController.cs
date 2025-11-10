using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Domain.Model;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/patients")]
    public class PatientsController : ControllerBase
    {
        private readonly AdminInputs _adminInputs;

        public PatientsController(AdminInputs adminInputs)
        {
            _adminInputs = adminInputs;
        }

        [HttpPost]
        public IActionResult CreatePatient([FromBody] CreatePatientRequest request)
        {
            try
            {
                _adminInputs.CreatePatient(
                    request.Fullname,
                    request.Dni,
                    request.Email,
                    request.Phonenumber,
                    DateOnly.Parse(request.Birthdate),
                    request.Address,
                    Enum.Parse<Gender>(request.Gender),
                    request.EmergencyFirstName,
                    request.EmergencyLastName,
                    request.EmergencyRelationship,
                    request.EmergencyPhone,
                    request.InsuranceCompanyName,
                    request.InsurancePolicyNumber,
                    request.InsuranceIsActive,
                    DateTime.Parse(request.InsuranceExpirationDate)
                );

                return Ok(new { message = "Paciente creado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{dni}")]
        public IActionResult UpdatePatient(string dni, [FromBody] UpdatePatientRequest request)
        {
            try
            {
                var patient = _adminInputs.GetPatientByDni(dni);
                if (patient == null)
                {
                    return NotFound(new { message = "Paciente no encontrado." });
                }

                _adminInputs.UpdatePatient(patient, request.Email, request.Phone, request.Address);
                return Ok(new { message = "Paciente actualizado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{dni}")]
        public IActionResult GetPatientByDni(string dni)
        {
            try
            {
                var patient = _adminInputs.GetPatientByDni(dni);
                if (patient == null)
                {
                    return NotFound(new { message = "Paciente no encontrado." });
                }

                return Ok(patient);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAllPatients()
        {
            try
            {
                var patients = _adminInputs.GetAllPatients();
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CreatePatientRequest
    {
        public string Fullname { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phonenumber { get; set; } = string.Empty;
        public string Birthdate { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string EmergencyFirstName { get; set; } = string.Empty;
        public string EmergencyLastName { get; set; } = string.Empty;
        public string EmergencyRelationship { get; set; } = string.Empty;
        public string EmergencyPhone { get; set; } = string.Empty;
        public string InsuranceCompanyName { get; set; } = string.Empty;
        public string InsurancePolicyNumber { get; set; } = string.Empty;
        public bool InsuranceIsActive { get; set; }
        public string InsuranceExpirationDate { get; set; } = string.Empty;
    }

    public class UpdatePatientRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}

