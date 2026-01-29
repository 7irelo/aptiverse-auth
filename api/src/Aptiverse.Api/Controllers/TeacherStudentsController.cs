using Aptiverse.Api.Application.TeacherStudents.Dtos;
using Aptiverse.Api.Application.TeacherStudents.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/teacher-students")]
    public class TeacherStudentsController(
        ITeacherStudentService teacherStudentService,
        ILogger<TeacherStudentsController> logger) : ControllerBase
    {
        private readonly ITeacherStudentService _teacherStudentService = teacherStudentService;
        private readonly ILogger<TeacherStudentsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<TeacherStudentDto>> CreateTeacherStudent([FromBody] CreateTeacherStudentDto createTeacherStudentDto)
        {
            try
            {
                var createdTeacherStudent = await _teacherStudentService.CreateTeacherStudentAsync(createTeacherStudentDto);
                return CreatedAtAction(nameof(GetTeacherStudent), new { id = createdTeacherStudent.Id }, createdTeacherStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating teacher student");
                return BadRequest(new { message = "Error creating teacher student", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherStudentDto>> GetTeacherStudent(long id)
        {
            try
            {
                var teacherStudent = await _teacherStudentService.GetTeacherStudentByIdAsync(id);

                if (teacherStudent == null)
                    return NotFound(new { message = $"TeacherStudent with ID {id} not found" });

                return Ok(teacherStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teacher student with ID {TeacherStudentId}", id);
                return StatusCode(500, new { message = "Error retrieving teacher student", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<TeacherStudentDto>>> GetTeacherStudents(
            [FromQuery] long? teacherId = null,
            [FromQuery] long? studentId = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] DateTime? assignedAfter = null,
            [FromQuery] DateTime? assignedBefore = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _teacherStudentService.GetTeacherStudentsAsync(
                    teacherId: teacherId,
                    studentId: studentId,
                    isActive: isActive,
                    assignedAfter: assignedAfter,
                    assignedBefore: assignedBefore,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teacher students");
                return StatusCode(500, new { message = "Error retrieving teacher students", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TeacherStudentDto>> UpdateTeacherStudent(long id, [FromBody] UpdateTeacherStudentDto updateTeacherStudentDto)
        {
            try
            {
                var updatedTeacherStudent = await _teacherStudentService.UpdateTeacherStudentAsync(id, updateTeacherStudentDto);
                return Ok(updatedTeacherStudent);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "TeacherStudent with ID {TeacherStudentId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating teacher student with ID {TeacherStudentId}", id);
                return BadRequest(new { message = "Error updating teacher student", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacherStudent(long id)
        {
            try
            {
                var result = await _teacherStudentService.DeleteTeacherStudentAsync(id);

                if (!result)
                    return NotFound(new { message = $"TeacherStudent with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting teacher student with ID {TeacherStudentId}", id);
                return StatusCode(500, new { message = "Error deleting teacher student", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountTeacherStudents(
            [FromQuery] long? teacherId = null,
            [FromQuery] long? studentId = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var count = await _teacherStudentService.CountTeacherStudentsAsync(teacherId, studentId, isActive);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting teacher students");
                return StatusCode(500, new { message = "Error counting teacher students", error = ex.Message });
            }
        }
    }
}