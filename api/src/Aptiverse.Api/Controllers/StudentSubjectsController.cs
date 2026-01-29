using Aptiverse.Api.Application.StudentSubjects.Dtos;
using Aptiverse.Api.Application.StudentSubjects.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/student-subjects")]
    public class StudentSubjectsController(
        IStudentSubjectService studentSubjectService,
        ILogger<StudentSubjectsController> logger) : ControllerBase
    {
        private readonly IStudentSubjectService _studentSubjectService = studentSubjectService;
        private readonly ILogger<StudentSubjectsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<StudentSubjectDto>> CreateStudentSubject([FromBody] CreateStudentSubjectDto createStudentSubjectDto)
        {
            try
            {
                var createdStudentSubject = await _studentSubjectService.CreateStudentSubjectAsync(createStudentSubjectDto);
                return CreatedAtAction(nameof(GetStudentSubject), new { id = createdStudentSubject.Id }, createdStudentSubject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student subject");
                return BadRequest(new { message = "Error creating student subject", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentSubjectDto>> GetStudentSubject(long id)
        {
            try
            {
                var studentSubject = await _studentSubjectService.GetStudentSubjectByIdAsync(id);

                if (studentSubject == null)
                    return NotFound(new { message = $"StudentSubject with ID {id} not found" });

                return Ok(studentSubject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student subject with ID {StudentSubjectId}", id);
                return StatusCode(500, new { message = "Error retrieving student subject", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<StudentSubjectDto>>> GetStudentSubjects(
            [FromQuery] long? studentId = null,
            [FromQuery] string? subjectId = null,
            [FromQuery] int? minProgress = null,
            [FromQuery] int? maxProgress = null,
            [FromQuery] double? minAverageScore = null,
            [FromQuery] double? maxAverageScore = null,
            [FromQuery] string? performanceTrend = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _studentSubjectService.GetStudentSubjectsAsync(
                    studentId: studentId,
                    subjectId: subjectId,
                    minProgress: minProgress,
                    maxProgress: maxProgress,
                    minAverageScore: minAverageScore,
                    maxAverageScore: maxAverageScore,
                    performanceTrend: performanceTrend,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student subjects");
                return StatusCode(500, new { message = "Error retrieving student subjects", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StudentSubjectDto>> UpdateStudentSubject(long id, [FromBody] UpdateStudentSubjectDto updateStudentSubjectDto)
        {
            try
            {
                var updatedStudentSubject = await _studentSubjectService.UpdateStudentSubjectAsync(id, updateStudentSubjectDto);
                return Ok(updatedStudentSubject);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "StudentSubject with ID {StudentSubjectId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student subject with ID {StudentSubjectId}", id);
                return BadRequest(new { message = "Error updating student subject", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentSubject(long id)
        {
            try
            {
                var result = await _studentSubjectService.DeleteStudentSubjectAsync(id);

                if (!result)
                    return NotFound(new { message = $"StudentSubject with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student subject with ID {StudentSubjectId}", id);
                return StatusCode(500, new { message = "Error deleting student subject", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountStudentSubjects(
            [FromQuery] long? studentId = null,
            [FromQuery] string? subjectId = null)
        {
            try
            {
                var count = await _studentSubjectService.CountStudentSubjectsAsync(studentId, subjectId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting student subjects");
                return StatusCode(500, new { message = "Error counting student subjects", error = ex.Message });
            }
        }
    }
}