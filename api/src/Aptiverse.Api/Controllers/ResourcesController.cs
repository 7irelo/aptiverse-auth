using Aptiverse.Api.Application.Resources.Dtos;
using Aptiverse.Api.Application.Resources.Services;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/resources")]
    public class ResourcesController(
        IResourceService resourceService,
        ILogger<ResourcesController> logger) : ControllerBase
    {
        private readonly IResourceService _resourceService = resourceService;
        private readonly ILogger<ResourcesController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<ResourceDto>> CreateResource([FromBody] CreateResourceDto createResourceDto)
        {
            try
            {
                var createdResource = await _resourceService.CreateResourceAsync(createResourceDto);
                return CreatedAtAction(nameof(GetResource), new { id = createdResource.Id }, createdResource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating resource");
                return BadRequest(new { message = "Error creating resource", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceDto>> GetResource(long id)
        {
            try
            {
                var resource = await _resourceService.GetResourceByIdAsync(id);

                if (resource == null)
                    return NotFound(new { message = $"Resource with ID {id} not found" });

                return Ok(resource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resource with ID {ResourceId}", id);
                return StatusCode(500, new { message = "Error retrieving resource", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ResourceDto>>> GetResources(
            [FromQuery] string? search = null,
            [FromQuery] string? subjectId = null,
            [FromQuery] string? resourceType = null,
            [FromQuery] string? gradeLevel = null,
            [FromQuery] long? teacherId = null,
            [FromQuery] long? tutorId = null,
            [FromQuery] long? courseId = null,
            [FromQuery] bool? isFree = null,
            [FromQuery] bool? isApproved = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? sortBy = "CreatedAt",
            [FromQuery] bool sortDescending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                {
                    return BadRequest(new { message = "minPrice cannot be greater than maxPrice" });
                }

                var result = await _resourceService.GetResourcesAsync(
                    currentUser: User,
                    search: search,
                    subjectId: subjectId,
                    resourceType: resourceType,
                    gradeLevel: gradeLevel,
                    teacherId: teacherId,
                    tutorId: tutorId,
                    courseId: courseId,
                    isFree: isFree,
                    isApproved: isApproved,
                    minPrice: minPrice,
                    maxPrice: maxPrice,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resources");
                return StatusCode(500, new { message = "Error retrieving resources", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResourceDto>> UpdateResource(long id, [FromBody] UpdateResourceDto updateResourceDto)
        {
            try
            {
                var updatedResource = await _resourceService.UpdateResourceAsync(id, updateResourceDto);
                return Ok(updatedResource);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource with ID {ResourceId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating resource with ID {ResourceId}", id);
                return BadRequest(new { message = "Error updating resource", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResource(long id)
        {
            try
            {
                var result = await _resourceService.DeleteResourceAsync(id);

                if (!result)
                    return NotFound(new { message = $"Resource with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting resource with ID {ResourceId}", id);
                return StatusCode(500, new { message = "Error deleting resource", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountResources(
            [FromQuery] string? subjectId = null,
            [FromQuery] string? resourceType = null,
            [FromQuery] bool? isFree = null,
            [FromQuery] bool? isApproved = null)
        {
            try
            {
                var count = await _resourceService.CountResourcesAsync(User, subjectId, resourceType, isFree, isApproved);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting resources");
                return StatusCode(500, new { message = "Error counting resources", error = ex.Message });
            }
        }

        [HttpGet("subject/{subjectId}")]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetResourcesBySubject(string subjectId)
        {
            try
            {
                var resources = await _resourceService.GetResourcesBySubjectAsync(subjectId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resources for subject {SubjectId}", subjectId);
                return StatusCode(500, new { message = "Error retrieving resources", error = ex.Message });
            }
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetResourcesByTeacher(long teacherId)
        {
            try
            {
                var resources = await _resourceService.GetResourcesByTeacherAsync(teacherId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resources for teacher {TeacherId}", teacherId);
                return StatusCode(500, new { message = "Error retrieving resources", error = ex.Message });
            }
        }

        [HttpGet("tutor/{tutorId}")]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetResourcesByTutor(long tutorId)
        {
            try
            {
                var resources = await _resourceService.GetResourcesByTutorAsync(tutorId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resources for tutor {TutorId}", tutorId);
                return StatusCode(500, new { message = "Error retrieving resources", error = ex.Message });
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetResourcesByCourse(long courseId)
        {
            try
            {
                var resources = await _resourceService.GetResourcesByCourseAsync(courseId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resources for course {CourseId}", courseId);
                return StatusCode(500, new { message = "Error retrieving resources", error = ex.Message });
            }
        }

        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetPopularResources([FromQuery] int count = 10)
        {
            try
            {
                if (count < 1 || count > 50) count = 10;

                var resources = await _resourceService.GetPopularResourcesAsync(count);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving popular resources");
                return StatusCode(500, new { message = "Error retrieving popular resources", error = ex.Message });
            }
        }

        [HttpGet("free")]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetFreeResources()
        {
            try
            {
                var resources = await _resourceService.GetFreeResourcesAsync();
                return Ok(resources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving free resources");
                return StatusCode(500, new { message = "Error retrieving free resources", error = ex.Message });
            }
        }

        [HttpPost("{id}/download")]
        public async Task<ActionResult<ResourceDto>> DownloadResource(long id)
        {
            try
            {
                var resource = await _resourceService.IncrementDownloadCountAsync(id);
                return Ok(resource);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource with ID {ResourceId} not found for download", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading resource with ID {ResourceId}", id);
                return BadRequest(new { message = "Error downloading resource", error = ex.Message });
            }
        }

        [HttpPut("{id}/rating")]
        public async Task<ActionResult<ResourceDto>> UpdateResourceRating(long id, [FromBody] double rating)
        {
            try
            {
                if (rating < 0 || rating > 5)
                {
                    return BadRequest(new { message = "Rating must be between 0 and 5" });
                }

                var resource = await _resourceService.UpdateResourceRatingAsync(id, rating);
                return Ok(resource);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource with ID {ResourceId} not found for rating update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating resource rating for resource with ID {ResourceId}", id);
                return BadRequest(new { message = "Error updating resource rating", error = ex.Message });
            }
        }

        [HttpGet("my-resources")]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetMyResources()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var currentUser = User;
                long? teacherId = null;
                long? tutorId = null;

                if (UserContextHelper.IsTeacher(currentUser))
                {
                    
                }
                else if (UserContextHelper.IsTutor(currentUser))
                {
                    
                }
                else
                {
                    return Forbid();
                }
                return Ok(new List<ResourceDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user's resources");
                return StatusCode(500, new { message = "Error retrieving resources", error = ex.Message });
            }
        }
    }
}