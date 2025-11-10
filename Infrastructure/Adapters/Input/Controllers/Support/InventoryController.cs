using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Infrastructure.Config;
using Npgsql;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Support
{
    [ApiController]
    [Route("api/support/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly SupportInputs _supportInputs;
        private readonly SupportConfig _supportConfig;

        public InventoryController(SupportInputs supportInputs, SupportConfig supportConfig)
        {
            _supportInputs = supportInputs;
            _supportConfig = supportConfig;
        }

        private User? GetCurrentUserFromHeaders()
        {
            // Intentar obtener el usuario desde los headers
            if (Request.Headers.TryGetValue("X-User-Dni", out var dniHeader))
            {
                var dni = dniHeader.ToString().Trim();
                if (!string.IsNullOrEmpty(dni))
                {
                    return _supportConfig.UserPort.FindByDocument(dni);
                }
            }
            
            if (Request.Headers.TryGetValue("X-Username", out var usernameHeader))
            {
                var username = usernameHeader.ToString().Trim();
                if (!string.IsNullOrEmpty(username))
                {
                    return _supportConfig.UserPort.FindByUsername(username);
                }
            }

            return null;
        }

        // Medications
        [HttpPost("medications")]
        public IActionResult CreateMedication([FromBody] CreateMedicationRequest request)
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
                _supportInputs.SetCurrentUser(currentUser);

                _supportInputs.CreateMedication(
                    request.Id,
                    request.Name,
                    request.Cost,
                    request.DefaultDose,
                    request.TreatmentDurationDays
                );

                return Ok(new { message = "Medicamento creado exitosamente." });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // Capturar errores de base de datos (claves duplicadas, etc.)
                if (dbEx.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    return BadRequest(new { message = "Ya existe un recurso clínico con este ID. El ID debe ser único entre todos los recursos clínicos (medicamentos, procedimientos y ayudas diagnósticas)." });
                }
                return BadRequest(new { message = $"Error al guardar en la base de datos: {dbEx.Message}" });
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
                // Obtener el usuario actual desde los headers
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                // Establecer el usuario actual en el use case
                _supportInputs.SetCurrentUser(currentUser);

                var medications = _supportInputs.GetAllMedications();
                var medication = medications.FirstOrDefault(m => m.Id == request.Id);
                if (medication == null)
                {
                    return NotFound(new { message = "Medicamento no encontrado." });
                }

                _supportInputs.UpdateMedication(medication);
                return Ok(new { message = "Medicamento actualizado exitosamente." });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // Capturar errores de base de datos (claves duplicadas, etc.)
                if (dbEx.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    return BadRequest(new { message = "Ya existe un recurso clínico con este ID. El ID debe ser único entre todos los recursos clínicos (medicamentos, procedimientos y ayudas diagnósticas)." });
                }
                return BadRequest(new { message = $"Error al guardar en la base de datos: {dbEx.Message}" });
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
                // Obtener el usuario actual desde los headers
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                // Permitir acceso a Support y Doctor para lectura
                if (currentUser.Role != Role.Support && currentUser.Role != Role.Doctor)
                {
                    return Unauthorized(new { message = "No tiene permisos para acceder a este recurso." });
                }

                // Usar ManageInventoryService directamente para lectura (no requiere validación de rol)
                var medications = _supportConfig.ManageInventoryService.GetAllMedications();
                return Ok(medications);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // Capturar errores de base de datos (claves duplicadas, etc.)
                if (dbEx.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    return BadRequest(new { message = "Ya existe un recurso clínico con este ID. El ID debe ser único entre todos los recursos clínicos (medicamentos, procedimientos y ayudas diagnósticas)." });
                }
                return BadRequest(new { message = $"Error al guardar en la base de datos: {dbEx.Message}" });
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
                // Obtener el usuario actual desde los headers
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                // Establecer el usuario actual en el use case
                _supportInputs.SetCurrentUser(currentUser);

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
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // Capturar errores de base de datos (claves duplicadas, etc.)
                if (dbEx.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    return BadRequest(new { message = "Ya existe un recurso clínico con este ID. El ID debe ser único entre todos los recursos clínicos (medicamentos, procedimientos y ayudas diagnósticas)." });
                }
                return BadRequest(new { message = $"Error al guardar en la base de datos: {dbEx.Message}" });
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
                // Obtener el usuario actual desde los headers
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                // Establecer el usuario actual en el use case
                _supportInputs.SetCurrentUser(currentUser);

                var procedures = _supportInputs.GetAllProcedures();
                var procedure = procedures.FirstOrDefault(p => p.Id == request.Id);
                if (procedure == null)
                {
                    return NotFound(new { message = "Procedimiento no encontrado." });
                }

                _supportInputs.UpdateProcedure(procedure);
                return Ok(new { message = "Procedimiento actualizado exitosamente." });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // Capturar errores de base de datos (claves duplicadas, etc.)
                if (dbEx.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    return BadRequest(new { message = "Ya existe un recurso clínico con este ID. El ID debe ser único entre todos los recursos clínicos (medicamentos, procedimientos y ayudas diagnósticas)." });
                }
                return BadRequest(new { message = $"Error al guardar en la base de datos: {dbEx.Message}" });
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
                // Obtener el usuario actual desde los headers
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                // Permitir acceso a Support y Doctor para lectura
                if (currentUser.Role != Role.Support && currentUser.Role != Role.Doctor)
                {
                    return Unauthorized(new { message = "No tiene permisos para acceder a este recurso." });
                }

                // Usar ManageInventoryService directamente para lectura (no requiere validación de rol)
                var procedures = _supportConfig.ManageInventoryService.GetAllProcedures();
                return Ok(procedures);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // Capturar errores de base de datos (claves duplicadas, etc.)
                if (dbEx.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    return BadRequest(new { message = "Ya existe un recurso clínico con este ID. El ID debe ser único entre todos los recursos clínicos (medicamentos, procedimientos y ayudas diagnósticas)." });
                }
                return BadRequest(new { message = $"Error al guardar en la base de datos: {dbEx.Message}" });
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
                // Obtener el usuario actual desde los headers
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                // Establecer el usuario actual en el use case
                _supportInputs.SetCurrentUser(currentUser);

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
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // Capturar errores de base de datos (claves duplicadas, etc.)
                if (dbEx.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    return BadRequest(new { message = "Ya existe un recurso clínico con este ID. El ID debe ser único entre todos los recursos clínicos (medicamentos, procedimientos y ayudas diagnósticas)." });
                }
                return BadRequest(new { message = $"Error al guardar en la base de datos: {dbEx.Message}" });
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
                // Obtener el usuario actual desde los headers
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                // Establecer el usuario actual en el use case
                _supportInputs.SetCurrentUser(currentUser);

                var diagnosticAids = _supportInputs.GetAllDiagnosticAids();
                var diagnosticAid = diagnosticAids.FirstOrDefault(d => d.Id == request.Id);
                if (diagnosticAid == null)
                {
                    return NotFound(new { message = "Ayuda diagnóstica no encontrada." });
                }

                _supportInputs.UpdateDiagnosticAid(diagnosticAid);
                return Ok(new { message = "Ayuda diagnóstica actualizada exitosamente." });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // Capturar errores de base de datos (claves duplicadas, etc.)
                if (dbEx.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    return BadRequest(new { message = "Ya existe un recurso clínico con este ID. El ID debe ser único entre todos los recursos clínicos (medicamentos, procedimientos y ayudas diagnósticas)." });
                }
                return BadRequest(new { message = $"Error al guardar en la base de datos: {dbEx.Message}" });
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
                // Obtener el usuario actual desde los headers
                var currentUser = GetCurrentUserFromHeaders();
                if (currentUser == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                // Permitir acceso a Support y Doctor para lectura
                if (currentUser.Role != Role.Support && currentUser.Role != Role.Doctor)
                {
                    return Unauthorized(new { message = "No tiene permisos para acceder a este recurso." });
                }

                // Usar ManageInventoryService directamente para lectura (no requiere validación de rol)
                var diagnosticAids = _supportConfig.ManageInventoryService.GetAllDiagnosticAids();
                return Ok(diagnosticAids);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // Capturar errores de base de datos (claves duplicadas, etc.)
                if (dbEx.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    return BadRequest(new { message = "Ya existe un recurso clínico con este ID. El ID debe ser único entre todos los recursos clínicos (medicamentos, procedimientos y ayudas diagnósticas)." });
                }
                return BadRequest(new { message = $"Error al guardar en la base de datos: {dbEx.Message}" });
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

