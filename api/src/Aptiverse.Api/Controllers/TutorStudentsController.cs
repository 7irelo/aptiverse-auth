using Aptiverse.Api.Application.TutorStudents.Dtos;
using Aptiverse.Api.Application.TutorStudents.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/tutor-students")]
    public class TutorStudentsController(
        ITutorStudentService tutorStudentService,
        ILogger<TutorStudentsController> logger) : ControllerBase
    {
        private readonly ITutorStudentService _tutorStudentService = tutorStudentService;
        private readonly ILogger<TutorStudentsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<TutorStudentDto>> CreateTutorStudent([FromBody] CreateTutorStudentDto createTutorStudentDto)
        {
            try
            {
                var createdTutorStudent = await _tutorStudentService.CreateTutorStudentAsync(createTutorStudentDto);
                return CreatedAtAction(nameof(GetTutorStudent), new { id = createdTutorStudent.Id }, createdTutorStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tutor student");
                return BadRequest(new { message = "Error creating tutor student", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TutorStudentDto>> GetTutorStudent(long id)
        {
            try
            {
                var tutorStudent = await _tutorStudentService.GetTutorStudentByIdAsync(id);

                if (tutorStudent == null)
                    return NotFound(new { message = $"TutorStudent with ID {id} not found" });

                return Ok(tutorStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tutor student with ID {TutorStudentId}", id);
                return StatusCode(500, new { message = "Error retrieving tutor student", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<TutorStudentDto>>> GetTutorStudents(
            [FromQuery] long? tutorId = null,
            [FromQuery] long? studentId = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int? minSessionsPerWeek = null,
            [FromQuery] int? maxSessionsPerWeek = null,
            [FromQuery] DateTime? startedAfter = null,
            [FromQuery] DateTime? startedBefore = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _tutorStudentService.GetTutorStudentsAsync(
                    tutorId: tutorId,
                    studentId: studentId,
                    isActive: isActive,
                    minSessionsPerWeek: minSessionsPerWeek,
                    maxSessionsPerWeek: maxSessionsPerWeek,
                    startedAfter: startedAfter,
                    startedBefore: startedBefore,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tutor students");
                return StatusCode(500, new { message = "Error retrieving tutor students", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TutorStudentDto>> UpdateTutorStudent(long id, [FromBody] UpdateTutorStudentDto updateTutorStudentDto)
        {
            try
            {
                var updatedTutorStudent = await _tutorStudentService.UpdateTutorStudentAsync(id, updateTutorStudentDto);
                return Ok(updatedTutorStudent);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "TutorStudent with ID {TutorStudentId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tutor student with ID {TutorStudentId}", id);
                return BadRequest(new { message = "Error updating tutor student", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTutorStudent(long id)
        {
            try
            {
                var result = await _tutorStudentService.DeleteTutorStudentAsync(id);

                if (!result)
                    return NotFound(new { message = $"TutorStudent with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tutor student with ID {TutorStudentId}", id);
                return StatusCode(500, new { message = "Error deleting tutor student", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountTutorStudents(
            [FromQuery] long? tutorId = null,
            [FromQuery] long? studentId = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var count = await _tutorStudentService.CountTutorStudentsAsync(tutorId, studentId, isActive);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting tutor students");
                return StatusCode(500, new { message = "Error counting tutor students", error = ex.Message });
            }
        }
    }
}