using Aptiverse.Api.Application.AdminStudents.Dtos;
using Aptiverse.Api.Application.AdminStudents.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/admin-students")]
    public class AdminStudentsController(
        IAdminStudentService adminStudentService,
        ILogger<AdminStudentsController> logger) : ControllerBase
    {
        private readonly IAdminStudentService _adminStudentService = adminStudentService;
        private readonly ILogger<AdminStudentsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<AdminStudentDto>> CreateAdminStudent([FromBody] CreateAdminStudentDto createDto)
        {
            try
            {
                var createdAdminStudent = await _adminStudentService.CreateAdminStudentAsync(createDto);
                return CreatedAtAction(nameof(GetAdminStudent), new { id = createdAdminStudent.Id }, createdAdminStudent);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Admin or Student not found for enrollment");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Duplicate enrollment attempt");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin student enrollment");
                return BadRequest(new { message = "Error creating enrollment", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdminStudentDto>> GetAdminStudent(long id)
        {
            try
            {
                var adminStudent = await _adminStudentService.GetAdminStudentByIdAsync(id);

                if (adminStudent == null)
                    return NotFound(new { message = $"AdminStudent enrollment with ID {id} not found" });

                return Ok(adminStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin student enrollment with ID {EnrollmentId}", id);
                return StatusCode(500, new { message = "Error retrieving enrollment", error = ex.Message });
            }
        }

        [HttpGet("admin/{adminId}/student/{studentId}")]
        public async Task<ActionResult<AdminStudentDto>> GetAdminStudentByAdminAndStudent(long adminId, long studentId)
        {
            try
            {
                var adminStudent = await _adminStudentService.GetAdminStudentByAdminAndStudentAsync(adminId, studentId);

                if (adminStudent == null)
                    return NotFound(new { message = $"Enrollment not found for admin {adminId} and student {studentId}" });

                return Ok(adminStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollment for admin {AdminId} and student {StudentId}", adminId, studentId);
                return StatusCode(500, new { message = "Error retrieving enrollment", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<AdminStudentDto>>> GetAdminStudents(
            [FromQuery] long? adminId = null,
            [FromQuery] long? studentId = null,
            [FromQuery] string? enrollmentStatus = null,
            [FromQuery] string? sortBy = "EnrolledDate",
            [FromQuery] bool sortDescending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _adminStudentService.GetAdminStudentsAsync(
                    currentUser: User,
                    adminId: adminId,
                    studentId: studentId,
                    enrollmentStatus: enrollmentStatus,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin student enrollments");
                return StatusCode(500, new { message = "Error retrieving enrollments", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/enrollments")]
        public async Task<ActionResult<IEnumerable<AdminStudentEnrollmentDto>>> GetStudentEnrollments(long studentId)
        {
            try
            {
                var enrollments = await _adminStudentService.GetStudentEnrollmentsAsync(studentId);
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollments for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving enrollments", error = ex.Message });
            }
        }

        [HttpGet("admin/{adminId}/student/{studentId}/enrolled")]
        public async Task<ActionResult<bool>> IsStudentEnrolled(long adminId, long studentId)
        {
            try
            {
                var isEnrolled = await _adminStudentService.IsStudentEnrolledAsync(adminId, studentId);
                return Ok(new { enrolled = isEnrolled });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking enrollment for admin {AdminId} and student {StudentId}", adminId, studentId);
                return StatusCode(500, new { message = "Error checking enrollment", error = ex.Message });
            }
        }

        [HttpGet("admin/{adminId}/count")]
        public async Task<ActionResult<int>> CountAdminStudents(long adminId, [FromQuery] string? status = null)
        {
            try
            {
                var count = await _adminStudentService.CountAdminStudentsAsync(adminId, status);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting students for admin {AdminId}", adminId);
                return StatusCode(500, new { message = "Error counting students", error = ex.Message });
            }
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferStudent(
            [FromQuery] long fromAdminId,
            [FromQuery] long toAdminId,
            [FromQuery] long studentId)
        {
            try
            {
                await _adminStudentService.TransferStudentAsync(fromAdminId, toAdminId, studentId);
                return Ok(new { message = "Student transferred successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Admin or student not found for transfer");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transferring student {StudentId} from admin {FromAdminId} to {ToAdminId}",
                    studentId, fromAdminId, toAdminId);
                return StatusCode(500, new { message = "Error transferring student", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AdminStudentDto>> UpdateAdminStudent(long id, [FromBody] UpdateAdminStudentDto updateDto)
        {
            try
            {
                var updatedAdminStudent = await _adminStudentService.UpdateAdminStudentAsync(id, updateDto);
                return Ok(updatedAdminStudent);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "AdminStudent enrollment with ID {EnrollmentId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating admin student enrollment with ID {EnrollmentId}", id);
                return BadRequest(new { message = "Error updating enrollment", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminStudent(long id)
        {
            try
            {
                var result = await _adminStudentService.DeleteAdminStudentAsync(id);

                if (!result)
                    return NotFound(new { message = $"AdminStudent enrollment with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting admin student enrollment with ID {EnrollmentId}", id);
                return StatusCode(500, new { message = "Error deleting enrollment", error = ex.Message });
            }
        }

        [HttpDelete("admin/{adminId}/student/{studentId}")]
        public async Task<IActionResult> DeleteAdminStudentByAdminAndStudent(long adminId, long studentId)
        {
            try
            {
                var result = await _adminStudentService.DeleteAdminStudentByAdminAndStudentAsync(adminId, studentId);

                if (!result)
                    return NotFound(new { message = $"Enrollment not found for admin {adminId} and student {studentId}" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting enrollment for admin {AdminId} and student {StudentId}", adminId, studentId);
                return StatusCode(500, new { message = "Error deleting enrollment", error = ex.Message });
            }
        }
    }
}