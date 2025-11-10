using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Infrastructure.Config;
using Clinica_Herramientas_2.Domain.Services;
using System.Linq;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/patients")]
    public class PatientsController : ControllerBase
    {
        private readonly AdminInputs _adminInputs;
        private readonly AdminConfig _adminConfig;
        private readonly ViewPatientInformation _viewPatientInformation;

        public PatientsController(AdminInputs adminInputs, AdminConfig adminConfig)
        {
            _adminInputs = adminInputs;
            _adminConfig = adminConfig;
            _viewPatientInformation = adminConfig.ViewPatientInformationService;
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
        public IActionResult CreatePatient([FromBody] CreatePatientRequest request)
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
                    DateTime.SpecifyKind(DateTime.Parse(request.InsuranceExpirationDate), DateTimeKind.Utc)
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
                // Obtener el usuario actual desde los headers
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                // Establecer el usuario actual en el use case
                _adminInputs.SetCurrentUser(currentUser);

                var patient = _adminInputs.GetPatientByDni(dni);
                if (patient == null)
                {
                    return NotFound(new { message = "Paciente no encontrado." });
                }

                _adminInputs.UpdatePatient(
                    patient, 
                    request.Email, 
                    request.Phone, 
                    request.Address,
                    request.EmergencyFirstName,
                    request.EmergencyLastName,
                    request.EmergencyRelationship,
                    request.EmergencyPhone,
                    request.InsuranceCompanyName,
                    request.InsurancePolicyNumber,
                    request.InsuranceIsActive,
                    DateTime.SpecifyKind(DateTime.Parse(request.InsuranceExpirationDate), DateTimeKind.Utc)
                );
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
                // Obtener el usuario actual desde los headers
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                // Permitir acceso a Admin, Doctor y Nurse
                if (currentUser.Role != Role.Admin && currentUser.Role != Role.Doctor && currentUser.Role != Role.Nurse)
                {
                    return Forbid("Solo administradores, doctores y enfermeras pueden acceder a esta información.");
                }

                // Usar ViewPatientInformation directamente (no requiere SetCurrentUser)
                var patient = _viewPatientInformation.GetPatientByDni(dni);

                var patientDto = new
                {
                    dni = patient.Dni,
                    fullname = patient.Fullname,
                    email = patient.Email,
                    phonenumber = patient.Phonenumber,
                    birthdate = patient.Birthdate.ToString("yyyy-MM-dd"),
                    address = patient.Address,
                    gender = patient.Gender.ToString(),
                    emergencyContact = patient.EmergencyContact != null ? new
                    {
                        firstname = patient.EmergencyContact.Firtname,
                        lastname = patient.EmergencyContact.Lastname,
                        relationship = patient.EmergencyContact.Relationship,
                        phoneNumber = patient.EmergencyContact.PhoneNumber
                    } : null,
                    insurance = patient.Insurance != null ? new
                    {
                        companyName = patient.Insurance.CompanyName,
                        policyNumber = patient.Insurance.PolicyNumber,
                        isActive = patient.Insurance.IsActive,
                        expirationDate = patient.Insurance.ExpirationDate
                    } : null,
                    appointments = patient.Appointments,
                    medicalRecords = patient.MedicalRecords,
                    nurseVisits = patient.NurseVisits,
                    invoices = patient.Invoices
                };
                return Ok(patientDto);
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
                // Obtener el usuario actual desde los headers
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                // Permitir acceso a Admin, Doctor y Nurse
                if (currentUser.Role != Role.Admin && currentUser.Role != Role.Doctor && currentUser.Role != Role.Nurse)
                {
                    return Forbid("Solo administradores, doctores y enfermeras pueden acceder a esta información.");
                }

                // Usar ViewPatientInformation directamente (no requiere SetCurrentUser)
                var patients = _viewPatientInformation.GetAllPatients();
                var patientDtos = patients.Select(p => new
                {
                    dni = p.Dni,
                    fullname = p.Fullname,
                    email = p.Email,
                    phonenumber = p.Phonenumber,
                    birthdate = p.Birthdate.ToString("yyyy-MM-dd"),
                    address = p.Address,
                    gender = p.Gender.ToString(),
                    emergencyContact = p.EmergencyContact != null ? new
                    {
                        firstname = p.EmergencyContact.Firtname,
                        lastname = p.EmergencyContact.Lastname,
                        relationship = p.EmergencyContact.Relationship,
                        phoneNumber = p.EmergencyContact.PhoneNumber
                    } : null,
                    insurance = p.Insurance != null ? new
                    {
                        companyName = p.Insurance.CompanyName,
                        policyNumber = p.Insurance.PolicyNumber,
                        isActive = p.Insurance.IsActive,
                        expirationDate = p.Insurance.ExpirationDate
                    } : null,
                    appointments = p.Appointments,
                    medicalRecords = p.MedicalRecords,
                    nurseVisits = p.NurseVisits,
                    invoices = p.Invoices
                }).ToList();
                return Ok(patientDtos);
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
        public string EmergencyFirstName { get; set; } = string.Empty;
        public string EmergencyLastName { get; set; } = string.Empty;
        public string EmergencyRelationship { get; set; } = string.Empty;
        public string EmergencyPhone { get; set; } = string.Empty;
        public string InsuranceCompanyName { get; set; } = string.Empty;
        public string InsurancePolicyNumber { get; set; } = string.Empty;
        public bool InsuranceIsActive { get; set; }
        public string InsuranceExpirationDate { get; set; } = string.Empty;
    }
}

