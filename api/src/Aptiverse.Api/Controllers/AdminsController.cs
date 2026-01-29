using Aptiverse.Api.Application.Admins.Dtos;
using Aptiverse.Api.Application.Admins.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/admins")]
    public class AdminsController(
        IAdminService adminService,
        ILogger<AdminsController> logger) : ControllerBase
    {
        private readonly IAdminService _adminService = adminService;
        private readonly ILogger<AdminsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<AdminDto>> CreateAdmin([FromBody] CreateAdminDto createDto)
        {
            try
            {
                var createdAdmin = await _adminService.CreateAdminAsync(createDto);
                return CreatedAtAction(nameof(GetAdmin), new { id = createdAdmin.Id }, createdAdmin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin");
                return BadRequest(new { message = "Error creating admin", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdminDto>> GetAdmin(long id)
        {
            try
            {
                var admin = await _adminService.GetAdminByIdAsync(id);

                if (admin == null)
                    return NotFound(new { message = $"Admin with ID {id} not found" });

                return Ok(admin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin with ID {AdminId}", id);
                return StatusCode(500, new { message = "Error retrieving admin", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<AdminDto>> GetAdminByUser(string userId)
        {
            try
            {
                var admin = await _adminService.GetAdminByUserIdAsync(userId);

                if (admin == null)
                    return NotFound(new { message = $"Admin with UserId {userId} not found" });

                return Ok(admin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin with UserId {UserId}", userId);
                return StatusCode(500, new { message = "Error retrieving admin", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<AdminDto>>> GetAdmins(
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = "SchoolName",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _adminService.GetAdminsAsync(
                    currentUser: User,
                    search: search,
                    isActive: isActive,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admins");
                return StatusCode(500, new { message = "Error retrieving admins", error = ex.Message });
            }
        }

        [HttpGet("{id}/summary")]
        public async Task<ActionResult<AdminSummaryDto>> GetAdminSummary(long id)
        {
            try
            {
                var summary = await _adminService.GetAdminSummaryAsync(id);
                return Ok(summary);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Admin with ID {AdminId} not found", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting summary for admin {AdminId}", id);
                return StatusCode(500, new { message = "Error getting admin summary", error = ex.Message });
            }
        }

        [HttpGet("{id}/student-count")]
        public async Task<ActionResult<int>> GetAdminStudentCount(long id)
        {
            try
            {
                var count = await _adminService.GetAdminStudentCountAsync(id);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student count for admin {AdminId}", id);
                return StatusCode(500, new { message = "Error getting student count", error = ex.Message });
            }
        }

        [HttpGet("{id}/teacher-count")]
        public async Task<ActionResult<int>> GetAdminTeacherCount(long id)
        {
            try
            {
                var count = await _adminService.GetAdminTeacherCountAsync(id);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teacher count for admin {AdminId}", id);
                return StatusCode(500, new { message = "Error getting teacher count", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AdminDto>> UpdateAdmin(long id, [FromBody] UpdateAdminDto updateDto)
        {
            try
            {
                var updatedAdmin = await _adminService.UpdateAdminAsync(id, updateDto);
                return Ok(updatedAdmin);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Admin with ID {AdminId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating admin with ID {AdminId}", id);
                return BadRequest(new { message = "Error updating admin", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(long id)
        {
            try
            {
                var result = await _adminService.DeleteAdminAsync(id);

                if (!result)
                    return NotFound(new { message = $"Admin with ID {id} not found" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete admin {AdminId} with assigned students/teachers", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting admin with ID {AdminId}", id);
                return StatusCode(500, new { message = "Error deleting admin", error = ex.Message });
            }
        }
    }
}