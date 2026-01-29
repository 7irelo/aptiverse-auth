using Aptiverse.Api.Application.Teachers.Dtos;
using Aptiverse.Api.Application.Teachers.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/teachers")]
    public class TeachersController(
        ITeacherService teacherService,
        ILogger<TeachersController> logger) : ControllerBase
    {
        private readonly ITeacherService _teacherService = teacherService;
        private readonly ILogger<TeachersController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<TeacherDto>> CreateTeacher([FromBody] CreateTeacherDto createTeacherDto)
        {
            try
            {
                var createdTeacher = await _teacherService.CreateTeacherAsync(createTeacherDto);
                return CreatedAtAction(nameof(GetTeacher), new { id = createdTeacher.Id }, createdTeacher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating teacher");
                return BadRequest(new { message = "Error creating teacher", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDto>> GetTeacher(long id)
        {
            try
            {
                var teacher = await _teacherService.GetTeacherByIdAsync(id);

                if (teacher == null)
                    return NotFound(new { message = $"Teacher with ID {id} not found" });

                return Ok(teacher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teacher with ID {TeacherId}", id);
                return StatusCode(500, new { message = "Error retrieving teacher", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<TeacherDto>>> GetTeachers(
            [FromQuery] string? search = null,
            [FromQuery] string? specialization = null,
            [FromQuery] int? minYearsOfExperience = null,
            [FromQuery] bool? isVerified = null,
            [FromQuery] decimal? minHourlyRate = null,
            [FromQuery] decimal? maxHourlyRate = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _teacherService.GetTeachersAsync(
                    search: search,
                    specialization: specialization,
                    minYearsOfExperience: minYearsOfExperience,
                    isVerified: isVerified,
                    minHourlyRate: minHourlyRate,
                    maxHourlyRate: maxHourlyRate,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teachers");
                return StatusCode(500, new { message = "Error retrieving teachers", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TeacherDto>> UpdateTeacher(long id, [FromBody] UpdateTeacherDto updateTeacherDto)
        {
            try
            {
                var updatedTeacher = await _teacherService.UpdateTeacherAsync(id, updateTeacherDto);
                return Ok(updatedTeacher);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Teacher with ID {TeacherId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating teacher with ID {TeacherId}", id);
                return BadRequest(new { message = "Error updating teacher", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(long id)
        {
            try
            {
                var result = await _teacherService.DeleteTeacherAsync(id);

                if (!result)
                    return NotFound(new { message = $"Teacher with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting teacher with ID {TeacherId}", id);
                return StatusCode(500, new { message = "Error deleting teacher", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountTeachers(
            [FromQuery] string? specialization = null,
            [FromQuery] bool? isVerified = null)
        {
            try
            {
                var count = await _teacherService.CountTeachersAsync(specialization, isVerified);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting teachers");
                return StatusCode(500, new { message = "Error counting teachers", error = ex.Message });
            }
        }
    }
}