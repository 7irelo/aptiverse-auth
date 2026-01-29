using Aptiverse.Api.Application.Parents.Dtos;
using Aptiverse.Api.Application.Parents.Services;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/parents")]
    public class ParentsController(
        IParentService parentService,
        ILogger<ParentsController> logger) : ControllerBase
    {
        private readonly IParentService _parentService = parentService;
        private readonly ILogger<ParentsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<ParentDto>> CreateParent([FromBody] CreateParentDto createParentDto)
        {
            try
            {
                var createdParent = await _parentService.CreateParentAsync(createParentDto);
                return CreatedAtAction(nameof(GetParent), new { id = createdParent.Id }, createdParent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating parent");
                return BadRequest(new { message = "Error creating parent", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ParentDto>> GetParent(long id)
        {
            try
            {
                var parent = await _parentService.GetParentByIdAsync(id);

                if (parent == null)
                    return NotFound(new { message = $"Parent with ID {id} not found" });

                return Ok(parent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving parent with ID {ParentId}", id);
                return StatusCode(500, new { message = "Error retrieving parent", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ParentDto>> GetParentByUserId(string userId)
        {
            try
            {
                var parent = await _parentService.GetParentByUserIdAsync(userId);

                if (parent == null)
                    return NotFound(new { message = $"Parent with user ID '{userId}' not found" });

                return Ok(parent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving parent with user ID {UserId}", userId);
                return StatusCode(500, new { message = "Error retrieving parent", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ParentDto>>> GetParents(
            [FromQuery] string? search = null,
            [FromQuery] string? occupation = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _parentService.GetParentsAsync(
                    currentUser: User,
                    search: search,
                    occupation: occupation,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving parents");
                return StatusCode(500, new { message = "Error retrieving parents", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ParentDto>> UpdateParent(long id, [FromBody] UpdateParentDto updateParentDto)
        {
            try
            {
                var updatedParent = await _parentService.UpdateParentAsync(id, updateParentDto);
                return Ok(updatedParent);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Parent with ID {ParentId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating parent with ID {ParentId}", id);
                return BadRequest(new { message = "Error updating parent", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParent(long id)
        {
            try
            {
                var result = await _parentService.DeleteParentAsync(id);

                if (!result)
                    return NotFound(new { message = $"Parent with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting parent with ID {ParentId}", id);
                return StatusCode(500, new { message = "Error deleting parent", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountParents([FromQuery] string? occupation = null)
        {
            try
            {
                var count = await _parentService.CountParentsAsync(User, occupation);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting parents");
                return StatusCode(500, new { message = "Error counting parents", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<ParentDto>>> GetParentsByStudent(long studentId)
        {
            try
            {
                var parents = await _parentService.GetParentsByStudentAsync(studentId);
                return Ok(parents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving parents for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving parents", error = ex.Message });
            }
        }

        [HttpGet("{parentId}/student-count")]
        public async Task<ActionResult<int>> GetStudentCount(long parentId)
        {
            try
            {
                var studentCount = await _parentService.GetStudentCountAsync(parentId);
                return Ok(new { studentCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student count for parent {ParentId}", parentId);
                return StatusCode(500, new { message = "Error retrieving student count", error = ex.Message });
            }
        }

        [HttpGet("my-profile")]
        public async Task<ActionResult<ParentDto>> GetMyParentProfile()
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

                var parent = await _parentService.GetParentByUserIdAsync(userId);
                if (parent == null)
                {
                    return NotFound(new { message = "Parent profile not found for current user" });
                }

                return Ok(parent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user's parent profile");
                return StatusCode(500, new { message = "Error retrieving parent profile", error = ex.Message });
            }
        }

        [HttpPut("my-profile")]
        public async Task<ActionResult<ParentDto>> UpdateMyParentProfile([FromBody] UpdateParentDto updateParentDto)
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

                var parent = await _parentService.GetParentByUserIdAsync(userId);
                if (parent == null)
                {
                    return NotFound(new { message = "Parent profile not found for current user" });
                }

                var updatedParent = await _parentService.UpdateParentAsync(parent.Id, updateParentDto);
                return Ok(updatedParent);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Current user's parent profile not found for update");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating current user's parent profile");
                return BadRequest(new { message = "Error updating parent profile", error = ex.Message });
            }
        }
    }
}