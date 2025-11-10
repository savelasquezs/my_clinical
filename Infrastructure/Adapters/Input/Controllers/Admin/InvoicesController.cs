using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/invoices")]
    public class InvoicesController : ControllerBase
    {
        private readonly AdminInputs _adminInputs;

        public InvoicesController(AdminInputs adminInputs)
        {
            _adminInputs = adminInputs;
        }

        [HttpPost]
        public IActionResult CreateInvoice([FromBody] CreateInvoiceRequest request)
        {
            try
            {
                var invoice = _adminInputs.CreateInvoice(
                    request.InvoiceNumber,
                    request.PatientDni,
                    request.DoctorDni,
                    request.OrderNumbers,
                    DateTime.Parse(request.InvoiceDate)
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

