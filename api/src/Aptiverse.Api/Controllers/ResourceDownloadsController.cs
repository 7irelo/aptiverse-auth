using Aptiverse.Api.Application.ResourceDownloads.Dtos;
using Aptiverse.Api.Application.ResourceDownloads.Services;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/resource-downloads")]
    public class ResourceDownloadsController(
        IResourceDownloadService resourceDownloadService,
        ILogger<ResourceDownloadsController> logger) : ControllerBase
    {
        private readonly IResourceDownloadService _resourceDownloadService = resourceDownloadService;
        private readonly ILogger<ResourceDownloadsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<ResourceDownloadDto>> CreateResourceDownload([FromBody] CreateResourceDownloadDto createResourceDownloadDto)
        {
            try
            {
                var createdResourceDownload = await _resourceDownloadService.CreateResourceDownloadAsync(createResourceDownloadDto);
                return CreatedAtAction(nameof(GetResourceDownload), new { id = createdResourceDownload.Id }, createdResourceDownload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating resource download");
                return BadRequest(new { message = "Error creating resource download", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceDownloadDto>> GetResourceDownload(long id)
        {
            try
            {
                var resourceDownload = await _resourceDownloadService.GetResourceDownloadByIdAsync(id);

                if (resourceDownload == null)
                    return NotFound(new { message = $"Resource download with ID {id} not found" });

                return Ok(resourceDownload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resource download with ID {ResourceDownloadId}", id);
                return StatusCode(500, new { message = "Error retrieving resource download", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ResourceDownloadDto>>> GetResourceDownloads(
            [FromQuery] long? resourceId = null,
            [FromQuery] long? studentId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? sortBy = "DownloadedAt",
            [FromQuery] bool sortDescending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                {
                    return BadRequest(new { message = "fromDate cannot be greater than toDate" });
                }

                var result = await _resourceDownloadService.GetResourceDownloadsAsync(
                    currentUser: User,
                    resourceId: resourceId,
                    studentId: studentId,
                    fromDate: fromDate,
                    toDate: toDate,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resource downloads");
                return StatusCode(500, new { message = "Error retrieving resource downloads", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResourceDownload(long id)
        {
            try
            {
                var result = await _resourceDownloadService.DeleteResourceDownloadAsync(id);

                if (!result)
                    return NotFound(new { message = $"Resource download with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting resource download with ID {ResourceDownloadId}", id);
                return StatusCode(500, new { message = "Error deleting resource download", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountResourceDownloads(
            [FromQuery] long? resourceId = null,
            [FromQuery] long? studentId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var count = await _resourceDownloadService.CountResourceDownloadsAsync(User, resourceId, studentId, fromDate, toDate);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting resource downloads");
                return StatusCode(500, new { message = "Error counting resource downloads", error = ex.Message });
            }
        }

        [HttpGet("resource/{resourceId}")]
        public async Task<ActionResult<IEnumerable<ResourceDownloadDto>>> GetDownloadsByResource(long resourceId)
        {
            try
            {
                var downloads = await _resourceDownloadService.GetDownloadsByResourceAsync(resourceId);
                return Ok(downloads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving downloads for resource {ResourceId}", resourceId);
                return StatusCode(500, new { message = "Error retrieving downloads", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<ResourceDownloadDto>>> GetDownloadsByStudent(long studentId)
        {
            try
            {
                var downloads = await _resourceDownloadService.GetDownloadsByStudentAsync(studentId);
                return Ok(downloads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving downloads for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving downloads", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/recent")]
        public async Task<ActionResult<IEnumerable<ResourceDownloadDto>>> GetRecentDownloads(long studentId, [FromQuery] int count = 10)
        {
            try
            {
                if (count < 1 || count > 50) count = 10;

                var downloads = await _resourceDownloadService.GetRecentDownloadsAsync(studentId, count);
                return Ok(downloads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent downloads for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving recent downloads", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/resource/{resourceId}/has-downloaded")]
        public async Task<ActionResult<bool>> HasStudentDownloadedResource(long studentId, long resourceId)
        {
            try
            {
                var hasDownloaded = await _resourceDownloadService.HasStudentDownloadedResourceAsync(studentId, resourceId);
                return Ok(new { hasDownloaded });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if student {StudentId} has downloaded resource {ResourceId}", studentId, resourceId);
                return StatusCode(500, new { message = "Error checking download status", error = ex.Message });
            }
        }

        [HttpGet("resource/{resourceId}/count")]
        public async Task<ActionResult<int>> GetDownloadCountByResource(long resourceId)
        {
            try
            {
                var count = await _resourceDownloadService.GetDownloadCountByResourceAsync(resourceId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving download count for resource {ResourceId}", resourceId);
                return StatusCode(500, new { message = "Error retrieving download count", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/count")]
        public async Task<ActionResult<int>> GetDownloadCountByStudent(long studentId)
        {
            try
            {
                var count = await _resourceDownloadService.GetDownloadCountByStudentAsync(studentId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving download count for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving download count", error = ex.Message });
            }
        }

        [HttpGet("popular")]
        public async Task<ActionResult<Dictionary<long, int>>> GetPopularResources(
            [FromQuery] int count = 10,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                if (count < 1 || count > 50) count = 10;

                if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                {
                    return BadRequest(new { message = "fromDate cannot be greater than toDate" });
                }

                var popularResources = await _resourceDownloadService.GetPopularResourcesAsync(count, fromDate, toDate);
                return Ok(popularResources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving popular resources");
                return StatusCode(500, new { message = "Error retrieving popular resources", error = ex.Message });
            }
        }

        [HttpGet("my-downloads")]
        public async Task<ActionResult<IEnumerable<ResourceDownloadDto>>> GetMyDownloads()
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

                return Ok(new List<ResourceDownloadDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's downloads");
                return StatusCode(500, new { message = "Error retrieving downloads", error = ex.Message });
            }
        }

        [HttpGet("my-recent-downloads")]
        public async Task<ActionResult<IEnumerable<ResourceDownloadDto>>> GetMyRecentDownloads([FromQuery] int count = 10)
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

                if (count < 1 || count > 50) count = 10;

                return Ok(new List<ResourceDownloadDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's recent downloads");
                return StatusCode(500, new { message = "Error retrieving recent downloads", error = ex.Message });
            }
        }

        [HttpGet("my-download-count")]
        public async Task<ActionResult<int>> GetMyDownloadCount()
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

                return Ok(new { count = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's download count");
                return StatusCode(500, new { message = "Error retrieving download count", error = ex.Message });
            }
        }
    }
}