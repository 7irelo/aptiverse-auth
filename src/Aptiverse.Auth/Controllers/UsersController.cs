using Aptiverse.Application.Users.Dtos;
using Aptiverse.Application.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Auth.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(IUserService userService, ILogger<StudentsController> logger) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly ILogger<StudentsController> _logger = logger;

        [HttpPost]
        [Authorize(Roles = "Admin,Superuser")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto user)
        {
            try
            {
                var result = await _userService.CreateUserAsync(user);
                return CreatedAtAction(nameof(GetOneUser), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex) when (ex.Message.Contains("Invalid password"))
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneUser(string id)
        {
            try
            {
                var result = await _userService.GetOneUserAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Superuser")]
        public async Task<IActionResult> GetManyUsers(
            [FromQuery] string? email = null,
            [FromQuery] string? username = null,
            [FromQuery] string? firstName = null,
            [FromQuery] string? lastName = null,
            [FromQuery] string? phoneNumber = null)
        {
            try
            {
                var result = await _userService.GetManyUsersAsync(email, username, firstName, lastName, phoneNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Superuser")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDto userDto)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(id, userDto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Superuser")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
