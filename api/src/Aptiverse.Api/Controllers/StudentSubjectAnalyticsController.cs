using Aptiverse.Api.Application.StudentSubjectAnalyticss.Dtos;
using Aptiverse.Api.Application.StudentSubjectAnalyticss.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/student-subject-analytics")]
    public class StudentSubjectAnalyticsController(
        IStudentSubjectAnalyticsService studentSubjectAnalyticsService,
        ILogger<StudentSubjectAnalyticsController> logger) : ControllerBase
    {
        private readonly IStudentSubjectAnalyticsService _studentSubjectAnalyticsService = studentSubjectAnalyticsService;
        private readonly ILogger<StudentSubjectAnalyticsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<StudentSubjectAnalyticsDto>> CreateStudentSubjectAnalytics(
            [FromBody] CreateStudentSubjectAnalyticsDto createStudentSubjectAnalyticsDto)
        {
            try
            {
                var createdAnalytics = await _studentSubjectAnalyticsService.CreateStudentSubjectAnalyticsAsync(createStudentSubjectAnalyticsDto);
                return CreatedAtAction(nameof(GetStudentSubjectAnalytics), new { id = createdAnalytics.Id }, createdAnalytics);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "CreateStudentSubjectAnalyticsDto is null");
                return BadRequest(new { message = "Student subject analytics data is required" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student subject analytics");
                return BadRequest(new { message = "Error creating student subject analytics", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentSubjectAnalyticsDto>> GetStudentSubjectAnalytics(long id)
        {
            try
            {
                var analytics = await _studentSubjectAnalyticsService.GetStudentSubjectAnalyticsByIdAsync(id);

                if (analytics == null)
                    return NotFound(new { message = $"StudentSubjectAnalytics with ID {id} not found" });

                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student subject analytics with ID {AnalyticsId}", id);
                return StatusCode(500, new { message = "Error retrieving student subject analytics", error = ex.Message });
            }
        }

        [HttpGet("by-student-subject/{studentSubjectId}")]
        public async Task<ActionResult<StudentSubjectAnalyticsDto>> GetStudentSubjectAnalyticsByStudentSubjectId(long studentSubjectId)
        {
            try
            {
                var analytics = await _studentSubjectAnalyticsService.GetStudentSubjectAnalyticsByStudentSubjectIdAsync(studentSubjectId);

                if (analytics == null)
                    return NotFound(new { message = $"StudentSubjectAnalytics for StudentSubject ID {studentSubjectId} not found" });

                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student subject analytics for StudentSubject ID {StudentSubjectId}", studentSubjectId);
                return StatusCode(500, new { message = "Error retrieving student subject analytics", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<StudentSubjectAnalyticsDto>>> GetStudentSubjectAnalytics(
            [FromQuery] long? studentSubjectId = null,
            [FromQuery] int? minConsistency = null,
            [FromQuery] int? maxConsistency = null,
            [FromQuery] double? minAttendanceRate = null,
            [FromQuery] double? maxAttendanceRate = null,
            [FromQuery] double? minMotivationLevel = null,
            [FromQuery] double? maxMotivationLevel = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _studentSubjectAnalyticsService.GetStudentSubjectAnalyticsAsync(
                    studentSubjectId: studentSubjectId,
                    minConsistency: minConsistency,
                    maxConsistency: maxConsistency,
                    minAttendanceRate: minAttendanceRate,
                    maxAttendanceRate: maxAttendanceRate,
                    minMotivationLevel: minMotivationLevel,
                    maxMotivationLevel: maxMotivationLevel,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student subject analytics");
                return StatusCode(500, new { message = "Error retrieving student subject analytics", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StudentSubjectAnalyticsDto>> UpdateStudentSubjectAnalytics(
            long id,
            [FromBody] UpdateStudentSubjectAnalyticsDto updateStudentSubjectAnalyticsDto)
        {
            try
            {
                var updatedAnalytics = await _studentSubjectAnalyticsService.UpdateStudentSubjectAnalyticsAsync(id, updateStudentSubjectAnalyticsDto);
                return Ok(updatedAnalytics);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "StudentSubjectAnalytics with ID {AnalyticsId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student subject analytics with ID {AnalyticsId}", id);
                return BadRequest(new { message = "Error updating student subject analytics", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentSubjectAnalytics(long id)
        {
            try
            {
                var result = await _studentSubjectAnalyticsService.DeleteStudentSubjectAnalyticsAsync(id);

                if (!result)
                    return NotFound(new { message = $"StudentSubjectAnalytics with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student subject analytics with ID {AnalyticsId}", id);
                return StatusCode(500, new { message = "Error deleting student subject analytics", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountStudentSubjectAnalytics([FromQuery] long? studentSubjectId = null)
        {
            try
            {
                var count = await _studentSubjectAnalyticsService.CountStudentSubjectAnalyticsAsync(studentSubjectId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting student subject analytics");
                return StatusCode(500, new { message = "Error counting student subject analytics", error = ex.Message });
            }
        }

        [HttpGet("exists/{id}")]
        public async Task<ActionResult<bool>> StudentSubjectAnalyticsExists(long id)
        {
            try
            {
                var exists = await _studentSubjectAnalyticsService.StudentSubjectAnalyticsExistsAsync(id);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if student subject analytics exists with ID {AnalyticsId}", id);
                return StatusCode(500, new { message = "Error checking if student subject analytics exists", error = ex.Message });
            }
        }

        [HttpGet("exists-for-student-subject/{studentSubjectId}")]
        public async Task<ActionResult<bool>> StudentSubjectAnalyticsExistsForStudentSubject(long studentSubjectId)
        {
            try
            {
                var exists = await _studentSubjectAnalyticsService.StudentSubjectAnalyticsExistsForStudentSubjectAsync(studentSubjectId);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if student subject analytics exists for StudentSubject ID {StudentSubjectId}", studentSubjectId);
                return StatusCode(500, new { message = "Error checking if student subject analytics exists", error = ex.Message });
            }
        }
    }
}