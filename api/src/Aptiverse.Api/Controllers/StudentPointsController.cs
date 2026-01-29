using Aptiverse.Api.Application.StudentPointss.Dtos;
using Aptiverse.Api.Application.StudentPointss.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/student-points")]
    public class StudentPointsController(
        IStudentPointsService studentPointsService,
        ILogger<StudentPointsController> logger) : ControllerBase
    {
        private readonly IStudentPointsService _studentPointsService = studentPointsService;
        private readonly ILogger<StudentPointsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<StudentPointsDto>> CreateStudentPoints([FromBody] CreateStudentPointsDto createStudentPointsDto)
        {
            try
            {
                var createdStudentPoints = await _studentPointsService.CreateStudentPointsAsync(createStudentPointsDto);
                return CreatedAtAction(nameof(GetStudentPoints), new { id = createdStudentPoints.Id }, createdStudentPoints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student points");
                return BadRequest(new { message = "Error creating student points", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentPointsDto>> GetStudentPoints(long id)
        {
            try
            {
                var studentPoints = await _studentPointsService.GetStudentPointsByIdAsync(id);

                if (studentPoints == null)
                    return NotFound(new { message = $"StudentPoints with ID {id} not found" });

                return Ok(studentPoints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student points with ID {StudentPointsId}", id);
                return StatusCode(500, new { message = "Error retrieving student points", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<StudentPointsDto>> GetStudentPointsByStudent(long studentId)
        {
            try
            {
                var studentPoints = await _studentPointsService.GetStudentPointsByStudentIdAsync(studentId);

                if (studentPoints == null)
                    return NotFound(new { message = $"StudentPoints for student ID {studentId} not found" });

                return Ok(studentPoints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student points for student ID {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving student points", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<StudentPointsDto>>> GetStudentPoints(
            [FromQuery] long? studentId = null,
            [FromQuery] int? minLevel = null,
            [FromQuery] int? maxLevel = null,
            [FromQuery] string? rank = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _studentPointsService.GetStudentPointsAsync(
                    studentId: studentId,
                    minLevel: minLevel,
                    maxLevel: maxLevel,
                    rank: rank,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student points");
                return StatusCode(500, new { message = "Error retrieving student points", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StudentPointsDto>> UpdateStudentPoints(long id, [FromBody] UpdateStudentPointsDto updateStudentPointsDto)
        {
            try
            {
                var updatedStudentPoints = await _studentPointsService.UpdateStudentPointsAsync(id, updateStudentPointsDto);
                return Ok(updatedStudentPoints);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "StudentPoints with ID {StudentPointsId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student points with ID {StudentPointsId}", id);
                return BadRequest(new { message = "Error updating student points", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentPoints(long id)
        {
            try
            {
                var result = await _studentPointsService.DeleteStudentPointsAsync(id);

                if (!result)
                    return NotFound(new { message = $"StudentPoints with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student points with ID {StudentPointsId}", id);
                return StatusCode(500, new { message = "Error deleting student points", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountStudentPoints(
            [FromQuery] long? studentId = null,
            [FromQuery] int? minLevel = null,
            [FromQuery] string? rank = null)
        {
            try
            {
                var count = await _studentPointsService.CountStudentPointsAsync(studentId, minLevel, rank);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting student points");
                return StatusCode(500, new { message = "Error counting student points", error = ex.Message });
            }
        }
    }
}