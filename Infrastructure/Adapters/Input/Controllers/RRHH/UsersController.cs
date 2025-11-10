using Microsoft.AspNetCore.Mvc;
using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Domain.Model;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Input.Controllers.RRHH
{
    [ApiController]
    [Route("api/rrhh/users")]
    public class UsersController : ControllerBase
    {
        private readonly RRHHInputs _rrhhInputs;

        public UsersController(RRHHInputs rrhhInputs)
        {
            _rrhhInputs = rrhhInputs;
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                _rrhhInputs.CreateUser(
                    request.Fullname,
                    request.Dni,
                    request.Email,
                    request.Phonenumber,
                    DateOnly.Parse(request.Birthdate),
                    request.Address,
                    Enum.Parse<Role>(request.Role),
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
                    request.Address
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
                return Ok(users);
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

                return Ok(user);
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
    }
}

