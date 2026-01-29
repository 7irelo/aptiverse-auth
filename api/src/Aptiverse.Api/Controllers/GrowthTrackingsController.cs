using Aptiverse.Api.Application.GrowthTrackings.Dtos;
using Aptiverse.Api.Application.GrowthTrackings.Services;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/growth-trackings")]
    public class GrowthTrackingsController(
        IGrowthTrackingService growthTrackingService,
        ILogger<GrowthTrackingsController> logger) : ControllerBase
    {
        private readonly IGrowthTrackingService _growthTrackingService = growthTrackingService;
        private readonly ILogger<GrowthTrackingsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<GrowthTrackingDto>> CreateGrowthTracking([FromBody] CreateGrowthTrackingDto createGrowthTrackingDto)
        {
            try
            {
                var createdGrowthTracking = await _growthTrackingService.CreateGrowthTrackingAsync(createGrowthTrackingDto);
                return CreatedAtAction(nameof(GetGrowthTracking), new { id = createdGrowthTracking.Id }, createdGrowthTracking);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Growth tracking already exists for student {StudentId} on date {Date}",
                    createGrowthTrackingDto.StudentId, createGrowthTrackingDto.TrackingDate.Date);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating growth tracking");
                return BadRequest(new { message = "Error creating growth tracking", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GrowthTrackingDto>> GetGrowthTracking(long id)
        {
            try
            {
                var growthTracking = await _growthTrackingService.GetGrowthTrackingByIdAsync(id);

                if (growthTracking == null)
                    return NotFound(new { message = $"Growth tracking with ID {id} not found" });

                return Ok(growthTracking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving growth tracking with ID {GrowthTrackingId}", id);
                return StatusCode(500, new { message = "Error retrieving growth tracking", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GrowthTrackingDto>>> GetGrowthTrackings(
            [FromQuery] long? studentId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] decimal? minGrowth = null,
            [FromQuery] decimal? maxGrowth = null,
            [FromQuery] string? sortBy = "TrackingDate",
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

                if (minGrowth.HasValue && maxGrowth.HasValue && minGrowth > maxGrowth)
                {
                    return BadRequest(new { message = "minGrowth cannot be greater than maxGrowth" });
                }

                var result = await _growthTrackingService.GetGrowthTrackingsAsync(
                    currentUser: User,
                    studentId: studentId,
                    fromDate: fromDate,
                    toDate: toDate,
                    minGrowth: minGrowth,
                    maxGrowth: maxGrowth,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving growth trackings");
                return StatusCode(500, new { message = "Error retrieving growth trackings", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GrowthTrackingDto>> UpdateGrowthTracking(long id, [FromBody] UpdateGrowthTrackingDto updateGrowthTrackingDto)
        {
            try
            {
                var updatedGrowthTracking = await _growthTrackingService.UpdateGrowthTrackingAsync(id, updateGrowthTrackingDto);
                return Ok(updatedGrowthTracking);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Growth tracking with ID {GrowthTrackingId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating growth tracking with ID {GrowthTrackingId}", id);
                return BadRequest(new { message = "Error updating growth tracking", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrowthTracking(long id)
        {
            try
            {
                var result = await _growthTrackingService.DeleteGrowthTrackingAsync(id);

                if (!result)
                    return NotFound(new { message = $"Growth tracking with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting growth tracking with ID {GrowthTrackingId}", id);
                return StatusCode(500, new { message = "Error deleting growth tracking", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountGrowthTrackings(
            [FromQuery] long? studentId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var count = await _growthTrackingService.CountGrowthTrackingsAsync(User, studentId, fromDate, toDate);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting growth trackings");
                return StatusCode(500, new { message = "Error counting growth trackings", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<GrowthTrackingDto>>> GetGrowthTrackingsByStudent(long studentId)
        {
            try
            {
                var growthTrackings = await _growthTrackingService.GetGrowthTrackingsByStudentAsync(studentId);
                return Ok(growthTrackings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving growth trackings for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving growth trackings", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/date-range")]
        public async Task<ActionResult<IEnumerable<GrowthTrackingDto>>> GetGrowthTrackingsByDateRange(
            long studentId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "startDate cannot be greater than endDate" });
                }

                var growthTrackings = await _growthTrackingService.GetGrowthTrackingsByDateRangeAsync(studentId, startDate, endDate);
                return Ok(growthTrackings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving growth trackings for student {StudentId} from {StartDate} to {EndDate}",
                    studentId, startDate, endDate);
                return StatusCode(500, new { message = "Error retrieving growth trackings", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/latest")]
        public async Task<ActionResult<GrowthTrackingDto>> GetLatestGrowthTracking(long studentId)
        {
            try
            {
                var growthTracking = await _growthTrackingService.GetLatestGrowthTrackingAsync(studentId);

                if (growthTracking == null)
                    return NotFound(new { message = $"No growth tracking found for student {studentId}" });

                return Ok(growthTracking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving latest growth tracking for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving latest growth tracking", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/date/{date}")]
        public async Task<ActionResult<GrowthTrackingDto>> GetGrowthTrackingByDate(long studentId, DateTime date)
        {
            try
            {
                var growthTracking = await _growthTrackingService.GetGrowthTrackingByDateAsync(studentId, date);

                if (growthTracking == null)
                    return NotFound(new { message = $"No growth tracking found for student {studentId} on date {date:yyyy-MM-dd}" });

                return Ok(growthTracking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving growth tracking for student {StudentId} on date {Date}", studentId, date);
                return StatusCode(500, new { message = "Error retrieving growth tracking", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/trends")]
        public async Task<ActionResult<Dictionary<string, decimal>>> GetGrowthTrends(
            long studentId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                {
                    return BadRequest(new { message = "fromDate cannot be greater than toDate" });
                }

                var trends = await _growthTrackingService.GetGrowthTrendsAsync(studentId, fromDate, toDate);
                return Ok(trends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving growth trends for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving growth trends", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/average-overall-growth")]
        public async Task<ActionResult<decimal>> GetAverageOverallGrowth(
            long studentId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                {
                    return BadRequest(new { message = "fromDate cannot be greater than toDate" });
                }

                var average = await _growthTrackingService.GetAverageOverallGrowthAsync(studentId, fromDate, toDate);
                return Ok(new { averageOverallGrowth = average });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average overall growth for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error calculating average overall growth", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/recent")]
        public async Task<ActionResult<IEnumerable<GrowthTrackingDto>>> GetRecentGrowthTrackings(long studentId, [FromQuery] int count = 10)
        {
            try
            {
                if (count < 1 || count > 50) count = 10;

                var growthTrackings = await _growthTrackingService.GetRecentGrowthTrackingsAsync(studentId, count);
                return Ok(growthTrackings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent growth trackings for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving recent growth trackings", error = ex.Message });
            }
        }

        [HttpGet("my-growth-trackings")]
        public async Task<ActionResult<IEnumerable<GrowthTrackingDto>>> GetMyGrowthTrackings()
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

                // Need to get the student ID from the user ID - implement this lookup
                // For now, return empty list
                return Ok(new List<GrowthTrackingDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's growth trackings");
                return StatusCode(500, new { message = "Error retrieving growth trackings", error = ex.Message });
            }
        }

        [HttpGet("my-latest-growth-tracking")]
        public async Task<ActionResult<GrowthTrackingDto>> GetMyLatestGrowthTracking()
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

                // Need to get the student ID from the user ID - implement this lookup
                // For now, return not found
                return NotFound(new { message = "No growth tracking found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's latest growth tracking");
                return StatusCode(500, new { message = "Error retrieving latest growth tracking", error = ex.Message });
            }
        }

        [HttpGet("my-growth-trends")]
        public async Task<ActionResult<Dictionary<string, decimal>>> GetMyGrowthTrends(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
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

                if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                {
                    return BadRequest(new { message = "fromDate cannot be greater than toDate" });
                }

                // Need to get the student ID from the user ID - implement this lookup
                // For now, return empty trends
                return Ok(new Dictionary<string, decimal>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's growth trends");
                return StatusCode(500, new { message = "Error retrieving growth trends", error = ex.Message });
            }
        }
    }
}