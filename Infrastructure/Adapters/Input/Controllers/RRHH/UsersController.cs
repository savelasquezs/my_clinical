using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Infrastructure.Config;
using System.Linq;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.RRHH
{
    [ApiController]
    [Route("api/rrhh/users")]
    public class UsersController : ControllerBase
    {
        private readonly RRHHInputs _rrhhInputs;
        private readonly RRHHConfig _rrhhConfig;

        public UsersController(RRHHInputs rrhhInputs, RRHHConfig rrhhConfig)
        {
            _rrhhInputs = rrhhInputs;
            _rrhhConfig = rrhhConfig;
        }

        private User? GetCurrentUserFromHeaders()
        {
            // Intentar obtener el usuario desde los headers
            if (Request.Headers.TryGetValue("X-User-Dni", out var dniHeader))
            {
                var dni = dniHeader.ToString().Trim();
                if (!string.IsNullOrEmpty(dni))
                {
                    return _rrhhConfig.UserPort.FindByDocument(dni);
                }
            }
            
            if (Request.Headers.TryGetValue("X-Username", out var usernameHeader))
            {
                var username = usernameHeader.ToString().Trim();
                if (!string.IsNullOrEmpty(username))
                {
                    return _rrhhConfig.UserPort.FindByUsername(username);
                }
            }

            return null;
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
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
                _rrhhInputs.SetCurrentUser(currentUser);

                // Parsear el rol con validación
                if (!Enum.TryParse<Role>(request.Role, ignoreCase: true, out var role))
                {
                    return BadRequest(new { message = $"Rol inválido: {request.Role}. Los roles válidos son: Admin, Doctor, Nurse, RRHH, Support." });
                }

                _rrhhInputs.CreateUser(
                    request.Fullname,
                    request.Dni,
                    request.Email,
                    request.Phonenumber,
                    DateOnly.Parse(request.Birthdate),
                    request.Address,
                    role,
                    request.Username,
                    request.Password
                );

                return Ok(new { message = "Usuario creado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{dni}")]
        public IActionResult UpdateUser(string dni, [FromBody] UpdateUserRequest request)
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
                _rrhhInputs.SetCurrentUser(currentUser);

                // Parsear el rol con validación
                if (!Enum.TryParse<Role>(request.Role, ignoreCase: true, out var role))
                {
                    return BadRequest(new { message = $"Rol inválido: {request.Role}. Los roles válidos son: Admin, Doctor, Nurse, RRHH, Support." });
                }

                var user = _rrhhInputs.FindByUsername(request.Username ?? "");
                if (user == null || user.Dni != dni)
                {
                    return NotFound(new { message = "Usuario no encontrado." });
                }

                _rrhhInputs.UpdateUser(
                    user,
                    request.Fullname,
                    request.Email,
                    request.Phonenumber,
                    request.Address,
                    role
                );

                return Ok(new { message = "Usuario actualizado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{dni}")]
        public IActionResult DeleteUser(string dni)
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
                _rrhhInputs.SetCurrentUser(currentUser);

                var users = _rrhhInputs.GetAllUsers();
                var user = users.FirstOrDefault(u => u.Dni == dni);
                if (user == null)
                {
                    return NotFound(new { message = "Usuario no encontrado." });
                }

                _rrhhInputs.DeleteUser(user);
                return Ok(new { message = "Usuario eliminado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _rrhhInputs.GetAllUsers();
                var userDtos = users.Select(u => new
                {
                    dni = u.Dni,
                    username = u.Username,
                    fullname = u.Fullname,
                    email = u.Email,
                    phonenumber = u.Phonenumber,
                    birthdate = u.Birthdate.ToString("yyyy-MM-dd"),
                    address = u.Address,
                    role = u.Role.ToString()
                }).ToList();
                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("username/{username}")]
        public IActionResult GetUserByUsername(string username)
        {
            try
            {
                var user = _rrhhInputs.FindByUsername(username);
                if (user == null)
                {
                    return NotFound(new { message = "Usuario no encontrado." });
                }

                return Ok(new
                {
                    dni = user.Dni,
                    username = user.Username,
                    fullname = user.Fullname,
                    email = user.Email,
                    phonenumber = user.Phonenumber,
                    birthdate = user.Birthdate.ToString("yyyy-MM-dd"),
                    address = user.Address,
                    role = user.Role.ToString()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CreateUserRequest
    {
        public string Fullname { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phonenumber { get; set; } = string.Empty;
        public string Birthdate { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UpdateUserRequest
    {
        public string? Username { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phonenumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}

