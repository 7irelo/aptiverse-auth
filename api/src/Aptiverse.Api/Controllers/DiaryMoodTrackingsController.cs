using Aptiverse.Api.Application.DiaryMoodTrackings.Dtos;
using Aptiverse.Api.Application.DiaryMoodTrackings.Services;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/mood-trackings")]
    public class DiaryMoodTrackingsController(
        IDiaryMoodTrackingService moodTrackingService,
        ILogger<DiaryMoodTrackingsController> logger) : ControllerBase
    {
        private readonly IDiaryMoodTrackingService _moodTrackingService = moodTrackingService;
        private readonly ILogger<DiaryMoodTrackingsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<DiaryMoodTrackingDto>> CreateMoodTracking([FromBody] CreateDiaryMoodTrackingDto createMoodTrackingDto)
        {
            try
            {
                var createdMoodTracking = await _moodTrackingService.CreateMoodTrackingAsync(createMoodTrackingDto);
                return CreatedAtAction(nameof(GetMoodTracking), new { id = createdMoodTracking.Id }, createdMoodTracking);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Mood tracking already exists for student {StudentId} on date {Date}",
                    createMoodTrackingDto.StudentId, createMoodTrackingDto.TrackingDate.Date);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating mood tracking");
                return BadRequest(new { message = "Error creating mood tracking", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DiaryMoodTrackingDto>> GetMoodTracking(long id)
        {
            try
            {
                var moodTracking = await _moodTrackingService.GetMoodTrackingByIdAsync(id);

                if (moodTracking == null)
                    return NotFound(new { message = $"Mood tracking with ID {id} not found" });

                return Ok(moodTracking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mood tracking with ID {MoodTrackingId}", id);
                return StatusCode(500, new { message = "Error retrieving mood tracking", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<DiaryMoodTrackingDto>>> GetMoodTrackings(
            [FromQuery] long? studentId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? overallMood = null,
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

                var result = await _moodTrackingService.GetMoodTrackingsAsync(
                    currentUser: User,
                    studentId: studentId,
                    fromDate: fromDate,
                    toDate: toDate,
                    overallMood: overallMood,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mood trackings");
                return StatusCode(500, new { message = "Error retrieving mood trackings", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DiaryMoodTrackingDto>> UpdateMoodTracking(long id, [FromBody] UpdateDiaryMoodTrackingDto updateMoodTrackingDto)
        {
            try
            {
                var updatedMoodTracking = await _moodTrackingService.UpdateMoodTrackingAsync(id, updateMoodTrackingDto);
                return Ok(updatedMoodTracking);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Mood tracking with ID {MoodTrackingId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating mood tracking with ID {MoodTrackingId}", id);
                return BadRequest(new { message = "Error updating mood tracking", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMoodTracking(long id)
        {
            try
            {
                var result = await _moodTrackingService.DeleteMoodTrackingAsync(id);

                if (!result)
                    return NotFound(new { message = $"Mood tracking with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting mood tracking with ID {MoodTrackingId}", id);
                return StatusCode(500, new { message = "Error deleting mood tracking", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountMoodTrackings(
            [FromQuery] long? studentId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var count = await _moodTrackingService.CountMoodTrackingsAsync(User, studentId, fromDate, toDate);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting mood trackings");
                return StatusCode(500, new { message = "Error counting mood trackings", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<DiaryMoodTrackingDto>>> GetMoodTrackingsByStudent(long studentId)
        {
            try
            {
                var moodTrackings = await _moodTrackingService.GetMoodTrackingsByStudentAsync(studentId);
                return Ok(moodTrackings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mood trackings for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving mood trackings", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/date-range")]
        public async Task<ActionResult<IEnumerable<DiaryMoodTrackingDto>>> GetMoodTrackingsByDateRange(
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

                var moodTrackings = await _moodTrackingService.GetMoodTrackingsByDateRangeAsync(studentId, startDate, endDate);
                return Ok(moodTrackings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mood trackings for student {StudentId} from {StartDate} to {EndDate}",
                    studentId, startDate, endDate);
                return StatusCode(500, new { message = "Error retrieving mood trackings", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/date/{date}")]
        public async Task<ActionResult<DiaryMoodTrackingDto>> GetMoodTrackingByDate(long studentId, DateTime date)
        {
            try
            {
                var moodTracking = await _moodTrackingService.GetMoodTrackingByDateAsync(studentId, date);

                if (moodTracking == null)
                    return NotFound(new { message = $"No mood tracking found for student {studentId} on date {date:yyyy-MM-dd}" });

                return Ok(moodTracking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mood tracking for student {StudentId} on date {Date}", studentId, date);
                return StatusCode(500, new { message = "Error retrieving mood tracking", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/mood-statistics")]
        public async Task<ActionResult<Dictionary<string, int>>> GetMoodStatistics(
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

                var statistics = await _moodTrackingService.GetMoodStatisticsAsync(studentId, fromDate, toDate);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mood statistics for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving mood statistics", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/average-levels")]
        public async Task<ActionResult<Dictionary<string, double>>> GetAverageLevels(
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

                var averages = await _moodTrackingService.GetAverageLevelsAsync(studentId, fromDate, toDate);
                return Ok(averages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average levels for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error calculating average levels", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/recent")]
        public async Task<ActionResult<IEnumerable<DiaryMoodTrackingDto>>> GetRecentMoodTrackings(long studentId, [FromQuery] int count = 10)
        {
            try
            {
                if (count < 1 || count > 50) count = 10;

                var moodTrackings = await _moodTrackingService.GetRecentMoodTrackingsAsync(studentId, count);
                return Ok(moodTrackings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent mood trackings for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving recent mood trackings", error = ex.Message });
            }
        }

        [HttpGet("my-mood-trackings")]
        public async Task<ActionResult<IEnumerable<DiaryMoodTrackingDto>>> GetMyMoodTrackings()
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
                return Ok(new List<DiaryMoodTrackingDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's mood trackings");
                return StatusCode(500, new { message = "Error retrieving mood trackings", error = ex.Message });
            }
        }

        [HttpGet("my-mood-tracking/today")]
        public async Task<ActionResult<DiaryMoodTrackingDto>> GetMyMoodTrackingForToday()
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

                var today = DateTime.Today;
                // Need to get the student ID from the user ID - you'll need to implement this lookup
                // For now, return not found
                return NotFound(new { message = $"No mood tracking found for today ({today:yyyy-MM-dd})" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's mood tracking for today");
                return StatusCode(500, new { message = "Error retrieving mood tracking", error = ex.Message });
            }
        }

        [HttpGet("my-mood-statistics")]
        public async Task<ActionResult<Dictionary<string, int>>> GetMyMoodStatistics(
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

                return Ok(new Dictionary<string, int>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's mood statistics");
                return StatusCode(500, new { message = "Error retrieving mood statistics", error = ex.Message });
            }
        }
    }
}