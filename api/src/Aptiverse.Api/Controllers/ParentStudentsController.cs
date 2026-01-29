using Aptiverse.Api.Application.ParentStudents.Dtos;
using Aptiverse.Api.Application.ParentStudents.Services;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/parent-students")]
    public class ParentStudentsController(
        IParentStudentService parentStudentService,
        ILogger<ParentStudentsController> logger) : ControllerBase
    {
        private readonly IParentStudentService _parentStudentService = parentStudentService;
        private readonly ILogger<ParentStudentsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<ParentStudentDto>> CreateParentStudent([FromBody] CreateParentStudentDto createParentStudentDto)
        {
            try
            {
                var createdParentStudent = await _parentStudentService.CreateParentStudentAsync(createParentStudentDto);
                return CreatedAtAction(nameof(GetParentStudent), new { id = createdParentStudent.Id }, createdParentStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating parent student relationship");
                return BadRequest(new { message = "Error creating parent student relationship", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ParentStudentDto>> GetParentStudent(long id)
        {
            try
            {
                var parentStudent = await _parentStudentService.GetParentStudentByIdAsync(id);

                if (parentStudent == null)
                    return NotFound(new { message = $"Parent student relationship with ID {id} not found" });

                return Ok(parentStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving parent student relationship with ID {ParentStudentId}", id);
                return StatusCode(500, new { message = "Error retrieving parent student relationship", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ParentStudentDto>>> GetParentStudents(
            [FromQuery] long? parentId = null,
            [FromQuery] long? studentId = null,
            [FromQuery] string? relationship = null,
            [FromQuery] bool? isPrimaryContact = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _parentStudentService.GetParentStudentsAsync(
                    currentUser: User,
                    parentId: parentId,
                    studentId: studentId,
                    relationship: relationship,
                    isPrimaryContact: isPrimaryContact,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving parent student relationships");
                return StatusCode(500, new { message = "Error retrieving parent student relationships", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ParentStudentDto>> UpdateParentStudent(long id, [FromBody] UpdateParentStudentDto updateParentStudentDto)
        {
            try
            {
                var updatedParentStudent = await _parentStudentService.UpdateParentStudentAsync(id, updateParentStudentDto);
                return Ok(updatedParentStudent);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Parent student relationship with ID {ParentStudentId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating parent student relationship with ID {ParentStudentId}", id);
                return BadRequest(new { message = "Error updating parent student relationship", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParentStudent(long id)
        {
            try
            {
                var result = await _parentStudentService.DeleteParentStudentAsync(id);

                if (!result)
                    return NotFound(new { message = $"Parent student relationship with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting parent student relationship with ID {ParentStudentId}", id);
                return StatusCode(500, new { message = "Error deleting parent student relationship", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountParentStudents(
            [FromQuery] long? parentId = null,
            [FromQuery] long? studentId = null,
            [FromQuery] string? relationship = null)
        {
            try
            {
                var count = await _parentStudentService.CountParentStudentsAsync(User, parentId, studentId, relationship);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting parent student relationships");
                return StatusCode(500, new { message = "Error counting parent student relationships", error = ex.Message });
            }
        }

        [HttpGet("parent/{parentId}")]
        public async Task<ActionResult<IEnumerable<ParentStudentDto>>> GetParentStudentsByParent(long parentId)
        {
            try
            {
                var parentStudents = await _parentStudentService.GetParentStudentsByParentAsync(parentId);
                return Ok(parentStudents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving parent student relationships for parent {ParentId}", parentId);
                return StatusCode(500, new { message = "Error retrieving parent student relationships", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<ParentStudentDto>>> GetParentStudentsByStudent(long studentId)
        {
            try
            {
                var parentStudents = await _parentStudentService.GetParentStudentsByStudentAsync(studentId);
                return Ok(parentStudents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving parent student relationships for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving parent student relationships", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/primary-contact")]
        public async Task<ActionResult<ParentStudentDto>> GetPrimaryContactForStudent(long studentId)
        {
            try
            {
                var primaryContact = await _parentStudentService.GetPrimaryContactForStudentAsync(studentId);

                if (primaryContact == null)
                    return NotFound(new { message = $"No primary contact found for student {studentId}" });

                return Ok(primaryContact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving primary contact for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving primary contact", error = ex.Message });
            }
        }

        [HttpGet("exists/{parentId}/{studentId}")]
        public async Task<ActionResult<bool>> ParentStudentExists(long parentId, long studentId)
        {
            try
            {
                var exists = await _parentStudentService.ExistsAsync(parentId, studentId);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if parent student relationship exists for parent {ParentId} and student {StudentId}", parentId, studentId);
                return StatusCode(500, new { message = "Error checking parent student relationship existence", error = ex.Message });
            }
        }

        [HttpGet("my-children")]
        public async Task<ActionResult<IEnumerable<ParentStudentDto>>> GetMyChildren()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var currentUser = User;
                if (!UserContextHelper.IsParent(currentUser))
                {
                    return Forbid();
                }

                // Need to get the parent ID from the user ID - you'll need to implement this lookup
                // For now, return empty list
                return Ok(new List<ParentStudentDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current parent's children");
                return StatusCode(500, new { message = "Error retrieving children", error = ex.Message });
            }
        }

        [HttpGet("my-parents")]
        public async Task<ActionResult<IEnumerable<ParentStudentDto>>> GetMyParents()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var currentUser = User;
                if (!UserContextHelper.IsStudent(currentUser))
                {
                    return Forbid();
                }

                // Need to get the student ID from the user ID - you'll need to implement this lookup
                // For now, return empty list
                return Ok(new List<ParentStudentDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's parents");
                return StatusCode(500, new { message = "Error retrieving parents", error = ex.Message });
            }
        }
    }
}