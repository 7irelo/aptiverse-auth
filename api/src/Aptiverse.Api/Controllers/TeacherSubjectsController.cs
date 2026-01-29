using Aptiverse.Api.Application.TeacherSubjects.Dtos;
using Aptiverse.Api.Application.TeacherSubjects.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/teacher-subjects")]
    public class TeacherSubjectsController(
        ITeacherSubjectService teacherSubjectService,
        ILogger<TeacherSubjectsController> logger) : ControllerBase
    {
        private readonly ITeacherSubjectService _teacherSubjectService = teacherSubjectService;
        private readonly ILogger<TeacherSubjectsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<TeacherSubjectDto>> CreateTeacherSubject([FromBody] CreateTeacherSubjectDto createTeacherSubjectDto)
        {
            try
            {
                var createdTeacherSubject = await _teacherSubjectService.CreateTeacherSubjectAsync(createTeacherSubjectDto);
                return CreatedAtAction(nameof(GetTeacherSubject), new { id = createdTeacherSubject.Id }, createdTeacherSubject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating teacher subject");
                return BadRequest(new { message = "Error creating teacher subject", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherSubjectDto>> GetTeacherSubject(long id)
        {
            try
            {
                var teacherSubject = await _teacherSubjectService.GetTeacherSubjectByIdAsync(id);

                if (teacherSubject == null)
                    return NotFound(new { message = $"TeacherSubject with ID {id} not found" });

                return Ok(teacherSubject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teacher subject with ID {TeacherSubjectId}", id);
                return StatusCode(500, new { message = "Error retrieving teacher subject", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<TeacherSubjectDto>>> GetTeacherSubjects(
            [FromQuery] long? teacherId = null,
            [FromQuery] string? subjectId = null,
            [FromQuery] int? minProficiencyLevel = null,
            [FromQuery] int? maxProficiencyLevel = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _teacherSubjectService.GetTeacherSubjectsAsync(
                    teacherId: teacherId,
                    subjectId: subjectId,
                    minProficiencyLevel: minProficiencyLevel,
                    maxProficiencyLevel: maxProficiencyLevel,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teacher subjects");
                return StatusCode(500, new { message = "Error retrieving teacher subjects", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TeacherSubjectDto>> UpdateTeacherSubject(long id, [FromBody] UpdateTeacherSubjectDto updateTeacherSubjectDto)
        {
            try
            {
                var updatedTeacherSubject = await _teacherSubjectService.UpdateTeacherSubjectAsync(id, updateTeacherSubjectDto);
                return Ok(updatedTeacherSubject);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "TeacherSubject with ID {TeacherSubjectId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating teacher subject with ID {TeacherSubjectId}", id);
                return BadRequest(new { message = "Error updating teacher subject", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacherSubject(long id)
        {
            try
            {
                var result = await _teacherSubjectService.DeleteTeacherSubjectAsync(id);

                if (!result)
                    return NotFound(new { message = $"TeacherSubject with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting teacher subject with ID {TeacherSubjectId}", id);
                return StatusCode(500, new { message = "Error deleting teacher subject", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountTeacherSubjects(
            [FromQuery] long? teacherId = null,
            [FromQuery] string? subjectId = null)
        {
            try
            {
                var count = await _teacherSubjectService.CountTeacherSubjectsAsync(teacherId, subjectId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting teacher subjects");
                return StatusCode(500, new { message = "Error counting teacher subjects", error = ex.Message });
            }
        }
    }
}