using Aptiverse.Api.Application.CourseEnrollments.Dtos;
using Aptiverse.Api.Application.CourseEnrollments.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class CourseEnrollmentsController(
        ICourseEnrollmentService enrollmentService,
        ILogger<CourseEnrollmentsController> logger) : ControllerBase
    {
        private readonly ICourseEnrollmentService _enrollmentService = enrollmentService;
        private readonly ILogger<CourseEnrollmentsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<CourseEnrollmentDto>> CreateEnrollment([FromBody] CreateCourseEnrollmentDto createEnrollmentDto)
        {
            try
            {
                var createdEnrollment = await _enrollmentService.CreateEnrollmentAsync(createEnrollmentDto);
                return CreatedAtAction(nameof(GetEnrollment), new { id = createdEnrollment.Id }, createdEnrollment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating enrollment");
                return BadRequest(new { message = "Error creating enrollment", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseEnrollmentDto>> GetEnrollment(long id)
        {
            try
            {
                var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);

                if (enrollment == null)
                    return NotFound(new { message = $"Enrollment with ID {id} not found" });

                return Ok(enrollment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollment with ID {EnrollmentId}", id);
                return StatusCode(500, new { message = "Error retrieving enrollment", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<CourseEnrollmentDto>>> GetEnrollments(
            [FromQuery] long? courseId = null,
            [FromQuery] long? studentId = null,
            [FromQuery] string? paymentStatus = null,
            [FromQuery] decimal? minProgress = null,
            [FromQuery] decimal? maxProgress = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                // Validate pagination parameters
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                // Validate progress range
                if (minProgress.HasValue && maxProgress.HasValue && minProgress > maxProgress)
                {
                    return BadRequest(new { message = "minProgress cannot be greater than maxProgress" });
                }

                var result = await _enrollmentService.GetEnrollmentsAsync(
                    currentUser: User,
                    courseId: courseId,
                    studentId: studentId,
                    paymentStatus: paymentStatus,
                    minProgress: minProgress,
                    maxProgress: maxProgress,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollments");
                return StatusCode(500, new { message = "Error retrieving enrollments", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CourseEnrollmentDto>> UpdateEnrollment(long id, [FromBody] UpdateCourseEnrollmentDto updateEnrollmentDto)
        {
            try
            {
                var updatedEnrollment = await _enrollmentService.UpdateEnrollmentAsync(id, updateEnrollmentDto);
                return Ok(updatedEnrollment);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Enrollment with ID {EnrollmentId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating enrollment with ID {EnrollmentId}", id);
                return BadRequest(new { message = "Error updating enrollment", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(long id)
        {
            try
            {
                var result = await _enrollmentService.DeleteEnrollmentAsync(id);

                if (!result)
                    return NotFound(new { message = $"Enrollment with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting enrollment with ID {EnrollmentId}", id);
                return StatusCode(500, new { message = "Error deleting enrollment", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountEnrollments(
            [FromQuery] long? courseId = null,
            [FromQuery] long? studentId = null,
            [FromQuery] string? paymentStatus = null)
        {
            try
            {
                var count = await _enrollmentService.CountEnrollmentsAsync(User, courseId, studentId, paymentStatus);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting enrollments");
                return StatusCode(500, new { message = "Error counting enrollments", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<CourseEnrollmentDto>>> GetEnrollmentsByStudent(long studentId)
        {
            try
            {
                var enrollments = await _enrollmentService.GetEnrollmentsByStudentAsync(studentId);
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollments for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving enrollments", error = ex.Message });
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<CourseEnrollmentDto>>> GetEnrollmentsByCourse(long courseId)
        {
            try
            {
                var enrollments = await _enrollmentService.GetEnrollmentsByCourseAsync(courseId);
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollments for course {CourseId}", courseId);
                return StatusCode(500, new { message = "Error retrieving enrollments", error = ex.Message });
            }
        }

        [HttpGet("progress/{studentId}/{courseId}")]
        public async Task<ActionResult<decimal>> GetStudentProgress(long studentId, long courseId)
        {
            try
            {
                var progress = await _enrollmentService.GetStudentProgressAsync(studentId, courseId);
                return Ok(new { progress });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving progress for student {StudentId} in course {CourseId}", studentId, courseId);
                return StatusCode(500, new { message = "Error retrieving progress", error = ex.Message });
            }
        }

        [HttpGet("enrolled/{studentId}/{courseId}")]
        public async Task<ActionResult<bool>> IsStudentEnrolled(long studentId, long courseId)
        {
            try
            {
                var isEnrolled = await _enrollmentService.IsStudentEnrolledAsync(studentId, courseId);
                return Ok(new { isEnrolled });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking enrollment for student {StudentId} in course {CourseId}", studentId, courseId);
                return StatusCode(500, new { message = "Error checking enrollment", error = ex.Message });
            }
        }
    }
}