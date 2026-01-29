using Aptiverse.Api.Application.TeacherAdmins.Dtos;
using Aptiverse.Api.Application.TeacherAdmins.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/teacher-admins")]
    public class TeacherAdminsController(
        ITeacherAdminService teacherAdminService,
        ILogger<TeacherAdminsController> logger) : ControllerBase
    {
        private readonly ITeacherAdminService _teacherAdminService = teacherAdminService;
        private readonly ILogger<TeacherAdminsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<TeacherAdminDto>> CreateTeacherAdmin([FromBody] CreateTeacherAdminDto createTeacherAdminDto)
        {
            try
            {
                var createdTeacherAdmin = await _teacherAdminService.CreateTeacherAdminAsync(createTeacherAdminDto);
                return CreatedAtAction(nameof(GetTeacherAdmin), new { id = createdTeacherAdmin.Id }, createdTeacherAdmin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating teacher admin");
                return BadRequest(new { message = "Error creating teacher admin", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherAdminDto>> GetTeacherAdmin(long id)
        {
            try
            {
                var teacherAdmin = await _teacherAdminService.GetTeacherAdminByIdAsync(id);

                if (teacherAdmin == null)
                    return NotFound(new { message = $"TeacherAdmin with ID {id} not found" });

                return Ok(teacherAdmin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teacher admin with ID {TeacherAdminId}", id);
                return StatusCode(500, new { message = "Error retrieving teacher admin", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<TeacherAdminDto>>> GetTeacherAdmins(
            [FromQuery] long? teacherId = null,
            [FromQuery] long? adminId = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? role = null,
            [FromQuery] DateTime? associatedAfter = null,
            [FromQuery] DateTime? associatedBefore = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _teacherAdminService.GetTeacherAdminsAsync(
                    teacherId: teacherId,
                    adminId: adminId,
                    isActive: isActive,
                    role: role,
                    associatedAfter: associatedAfter,
                    associatedBefore: associatedBefore,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teacher admins");
                return StatusCode(500, new { message = "Error retrieving teacher admins", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TeacherAdminDto>> UpdateTeacherAdmin(long id, [FromBody] UpdateTeacherAdminDto updateTeacherAdminDto)
        {
            try
            {
                var updatedTeacherAdmin = await _teacherAdminService.UpdateTeacherAdminAsync(id, updateTeacherAdminDto);
                return Ok(updatedTeacherAdmin);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "TeacherAdmin with ID {TeacherAdminId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating teacher admin with ID {TeacherAdminId}", id);
                return BadRequest(new { message = "Error updating teacher admin", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacherAdmin(long id)
        {
            try
            {
                var result = await _teacherAdminService.DeleteTeacherAdminAsync(id);

                if (!result)
                    return NotFound(new { message = $"TeacherAdmin with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting teacher admin with ID {TeacherAdminId}", id);
                return StatusCode(500, new { message = "Error deleting teacher admin", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountTeacherAdmins(
            [FromQuery] long? teacherId = null,
            [FromQuery] long? adminId = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var count = await _teacherAdminService.CountTeacherAdminsAsync(teacherId, adminId, isActive);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting teacher admins");
                return StatusCode(500, new { message = "Error counting teacher admins", error = ex.Message });
            }
        }
    }
}