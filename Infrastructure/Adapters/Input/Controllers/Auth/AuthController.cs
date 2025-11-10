using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Infrastructure.Config;
using Clinica_Herramientas_2.Domain.Model;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthConfig _authConfig;

        public AuthController(AuthConfig authConfig)
        {
            _authConfig = authConfig;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Usuario y contraseña son requeridos." });
            }

            var user = _authConfig.AuthenticateUserService.Authenticate(request.Username, request.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos." });
            }

            // Devolver usuario sin contraseña (Opción B del plan)
            return Ok(new
            {
                dni = user.Dni,
                username = user.Username,
                fullname = user.Fullname,
                email = user.Email,
                role = user.Role.ToString()
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

