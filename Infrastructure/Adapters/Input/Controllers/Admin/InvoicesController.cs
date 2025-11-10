using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Infrastructure.Config;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/invoices")]
    public class InvoicesController : ControllerBase
    {
        private readonly AdminInputs _adminInputs;
        private readonly AdminConfig _adminConfig;

        public InvoicesController(AdminInputs adminInputs, AdminConfig adminConfig)
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
        public IActionResult CreateInvoice([FromBody] CreateInvoiceRequest request)
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

                var invoice = _adminInputs.CreateInvoice(
                    request.InvoiceNumber,
                    request.PatientDni,
                    request.DoctorDni,
                    request.OrderNumbers,
                    DateTime.SpecifyKind(DateTime.Parse(request.InvoiceDate), DateTimeKind.Utc)
                );

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CreateInvoiceRequest
    {
        public int InvoiceNumber { get; set; }
        public string PatientDni { get; set; } = string.Empty;
        public string DoctorDni { get; set; } = string.Empty;
        public List<int> OrderNumbers { get; set; } = new();
        public string InvoiceDate { get; set; } = string.Empty;
    }
}

