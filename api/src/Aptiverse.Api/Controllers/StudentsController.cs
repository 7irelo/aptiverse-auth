using Aptiverse.Api.Application.Students.Dtos;
using Aptiverse.Api.Application.Students.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController(
        IStudentService studentService,
        ILogger<StudentsController> logger) : ControllerBase
    {
        private readonly IStudentService _studentService = studentService;
        private readonly ILogger<StudentsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] CreateStudentDto createStudentDto)
        {
            try
            {
                var createdStudent = await _studentService.CreateStudentAsync(createStudentDto);
                return CreatedAtAction(nameof(GetStudent), new { id = createdStudent.Id }, createdStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student");
                return BadRequest(new { message = "Error creating student", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(long id)
        {
            try
            {
                var student = await _studentService.GetStudentByIdAsync(id);

                if (student == null)
                    return NotFound(new { message = $"Student with ID {id} not found" });

                return Ok(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student with ID {StudentId}", id);
                return StatusCode(500, new { message = "Error retrieving student", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<StudentDto>>> GetStudents(
            [FromQuery] string? search = null,
            [FromQuery] string? grade = null,
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

                // Pass the current user from the controller
                var result = await _studentService.GetStudentsAsync(
                    currentUser: User,  // This is the ClaimsPrincipal
                    search: search,
                    grade: grade,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving students");
                return StatusCode(500, new { message = "Error retrieving students", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StudentDto>> UpdateStudent(long id, [FromBody] UpdateStudentDto updateStudentDto)
        {
            try
            {
                var updatedStudent = await _studentService.UpdateStudentAsync(id, updateStudentDto);
                return Ok(updatedStudent);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Student with ID {StudentId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student with ID {StudentId}", id);
                return BadRequest(new { message = "Error updating student", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(long id)
        {
            try
            {
                var result = await _studentService.DeleteStudentAsync(id);

                if (!result)
                    return NotFound(new { message = $"Student with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student with ID {StudentId}", id);
                return StatusCode(500, new { message = "Error deleting student", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountStudents([FromQuery] string? grade = null)
        {
            try
            {
                // Pass the current user from the controller
                var count = await _studentService.CountStudentsAsync(User, grade);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting students");
                return StatusCode(500, new { message = "Error counting students", error = ex.Message });
            }
        }
    }
}