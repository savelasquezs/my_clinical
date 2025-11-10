using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Support
{
    [ApiController]
    [Route("api/support/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly SupportInputs _supportInputs;

        public InventoryController(SupportInputs supportInputs)
        {
            _supportInputs = supportInputs;
        }

        // Medications
        [HttpPost("medications")]
        public IActionResult CreateMedication([FromBody] CreateMedicationRequest request)
        {
            try
            {
                _supportInputs.CreateMedication(
                    request.Id,
                    request.Name,
                    request.Cost,
                    request.DefaultDose,
                    request.TreatmentDurationDays
                );

                return Ok(new { message = "Medicamento creado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("medications")]
        public IActionResult UpdateMedication([FromBody] UpdateMedicationRequest request)
        {
            try
            {
                var medications = _supportInputs.GetAllMedications();
                var medication = medications.FirstOrDefault(m => m.Id == request.Id);
                if (medication == null)
                {
                    return NotFound(new { message = "Medicamento no encontrado." });
                }

                _supportInputs.UpdateMedication(medication);
                return Ok(new { message = "Medicamento actualizado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("medications")]
        public IActionResult GetAllMedications()
        {
            try
            {
                var medications = _supportInputs.GetAllMedications();
                return Ok(medications);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Procedures
        [HttpPost("procedures")]
        public IActionResult CreateProcedure([FromBody] CreateProcedureRequest request)
        {
            try
            {
                _supportInputs.CreateProcedure(
                    request.Id,
                    request.Name,
                    request.Cost,
                    request.Frequency,
                    request.RequiresSpecialist,
                    request.SpecialistTypeId
                );

                return Ok(new { message = "Procedimiento creado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("procedures")]
        public IActionResult UpdateProcedure([FromBody] UpdateProcedureRequest request)
        {
            try
            {
                var procedures = _supportInputs.GetAllProcedures();
                var procedure = procedures.FirstOrDefault(p => p.Id == request.Id);
                if (procedure == null)
                {
                    return NotFound(new { message = "Procedimiento no encontrado." });
                }

                _supportInputs.UpdateProcedure(procedure);
                return Ok(new { message = "Procedimiento actualizado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("procedures")]
        public IActionResult GetAllProcedures()
        {
            try
            {
                var procedures = _supportInputs.GetAllProcedures();
                return Ok(procedures);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Diagnostic Aids
        [HttpPost("diagnostic-aids")]
        public IActionResult CreateDiagnosticAid([FromBody] CreateDiagnosticAidRequest request)
        {
            try
            {
                _supportInputs.CreateDiagnosticAid(
                    request.Id,
                    request.Name,
                    request.Cost,
                    request.Quantity,
                    request.RequiresSpecialist,
                    request.SpecialistTypeId
                );

                return Ok(new { message = "Ayuda diagnóstica creada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("diagnostic-aids")]
        public IActionResult UpdateDiagnosticAid([FromBody] UpdateDiagnosticAidRequest request)
        {
            try
            {
                var diagnosticAids = _supportInputs.GetAllDiagnosticAids();
                var diagnosticAid = diagnosticAids.FirstOrDefault(d => d.Id == request.Id);
                if (diagnosticAid == null)
                {
                    return NotFound(new { message = "Ayuda diagnóstica no encontrada." });
                }

                _supportInputs.UpdateDiagnosticAid(diagnosticAid);
                return Ok(new { message = "Ayuda diagnóstica actualizada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("diagnostic-aids")]
        public IActionResult GetAllDiagnosticAids()
        {
            try
            {
                var diagnosticAids = _supportInputs.GetAllDiagnosticAids();
                return Ok(diagnosticAids);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CreateMedicationRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public string DefaultDose { get; set; } = string.Empty;
        public int TreatmentDurationDays { get; set; }
    }

    public class UpdateMedicationRequest
    {
        public int Id { get; set; }
    }

    public class CreateProcedureRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public int Frequency { get; set; }
        public bool RequiresSpecialist { get; set; }
        public int? SpecialistTypeId { get; set; }
    }

    public class UpdateProcedureRequest
    {
        public int Id { get; set; }
    }

    public class CreateDiagnosticAidRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public int Quantity { get; set; }
        public bool RequiresSpecialist { get; set; }
        public int? SpecialistTypeId { get; set; }
    }

    public class UpdateDiagnosticAidRequest
    {
        public int Id { get; set; }
    }
}

