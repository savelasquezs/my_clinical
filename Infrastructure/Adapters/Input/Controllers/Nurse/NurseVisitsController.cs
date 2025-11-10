using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Domain.Model;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Nurse
{
    [ApiController]
    [Route("api/nurse/visits")]
    public class NurseVisitsController : ControllerBase
    {
        private readonly NurseInputs _nurseInputs;

        public NurseVisitsController(NurseInputs nurseInputs)
        {
            _nurseInputs = nurseInputs;
        }

        [HttpPost]
        public IActionResult CreateNurseVisit([FromBody] CreateNurseVisitRequest request)
        {
            try
            {
                var patient = _nurseInputs.GetPatientByDni(request.PatientDni);
                if (patient == null)
                {
                    return NotFound(new { message = "Paciente no encontrado." });
                }

                // Necesitamos obtener el OrderItem - por ahora creamos uno temporal
                var medication = new Medication(0, "", 0, "", 0);
                var orderItem = new MedicationOrderItem(
                    request.OrderNumber,
                    request.ItemNumber,
                    request.Cost,
                    medication,
                    "",
                    0
                );

                var administeredMedications = request.AdministeredMedications?.Select(am =>
                    new AdministeredMedication(
                        orderItem,
                        request.TestsPerformed,
                        request.Notes,
                        DateTime.Parse(request.PerformedAt),
                        new Medication(am.MedicationId, "", 0, "", 0),
                        am.Dose,
                        am.AdministrationRoute
                    )
                ).ToList() ?? new List<AdministeredMedication>();

                _nurseInputs.CreateNurseVisit(
                    orderItem,
                    request.TestsPerformed,
                    request.Notes,
                    DateTime.Parse(request.PerformedAt),
                    request.BloodPressure,
                    request.Temperature,
                    request.Pulse,
                    request.OxygenLevel,
                    administeredMedications,
                    DateTime.Parse(request.VisitTime),
                    patient
                );

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
        public string VisitTime { get; set; } = string.Empty;
    }

    public class AdministeredMedicationRequest
    {
        public int MedicationId { get; set; }
        public string Dose { get; set; } = string.Empty;
        public string AdministrationRoute { get; set; } = string.Empty;
    }
}

