using Aptiverse.Api.Application.DiaryEntries.Dtos;
using Aptiverse.Api.Application.DiaryEntries.Services;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/diary-entries")]
    public class DiaryEntriesController(
        IDiaryEntryService diaryEntryService,
        ILogger<DiaryEntriesController> logger) : ControllerBase
    {
        private readonly IDiaryEntryService _diaryEntryService = diaryEntryService;
        private readonly ILogger<DiaryEntriesController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<DiaryEntryDto>> CreateDiaryEntry([FromBody] CreateDiaryEntryDto createDiaryEntryDto)
        {
            try
            {
                var createdDiaryEntry = await _diaryEntryService.CreateDiaryEntryAsync(createDiaryEntryDto);
                return CreatedAtAction(nameof(GetDiaryEntry), new { id = createdDiaryEntry.Id }, createdDiaryEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating diary entry");
                return BadRequest(new { message = "Error creating diary entry", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DiaryEntryDto>> GetDiaryEntry(long id)
        {
            try
            {
                var diaryEntry = await _diaryEntryService.GetDiaryEntryByIdAsync(id);

                if (diaryEntry == null)
                    return NotFound(new { message = $"Diary entry with ID {id} not found" });

                return Ok(diaryEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving diary entry with ID {DiaryEntryId}", id);
                return StatusCode(500, new { message = "Error retrieving diary entry", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<DiaryEntryDto>>> GetDiaryEntries(
            [FromQuery] long? studentId = null,
            [FromQuery] string? mood = null,
            [FromQuery] string? entryType = null,
            [FromQuery] bool? isPrivate = null,
            [FromQuery] string? sentiment = null,
            [FromQuery] bool? needsFollowUp = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = "EntryDate",
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

                var result = await _diaryEntryService.GetDiaryEntriesAsync(
                    currentUser: User,
                    studentId: studentId,
                    mood: mood,
                    entryType: entryType,
                    isPrivate: isPrivate,
                    sentiment: sentiment,
                    needsFollowUp: needsFollowUp,
                    fromDate: fromDate,
                    toDate: toDate,
                    search: search,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving diary entries");
                return StatusCode(500, new { message = "Error retrieving diary entries", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DiaryEntryDto>> UpdateDiaryEntry(long id, [FromBody] UpdateDiaryEntryDto updateDiaryEntryDto)
        {
            try
            {
                var updatedDiaryEntry = await _diaryEntryService.UpdateDiaryEntryAsync(id, updateDiaryEntryDto);
                return Ok(updatedDiaryEntry);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Diary entry with ID {DiaryEntryId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating diary entry with ID {DiaryEntryId}", id);
                return BadRequest(new { message = "Error updating diary entry", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiaryEntry(long id)
        {
            try
            {
                var result = await _diaryEntryService.DeleteDiaryEntryAsync(id);

                if (!result)
                    return NotFound(new { message = $"Diary entry with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting diary entry with ID {DiaryEntryId}", id);
                return StatusCode(500, new { message = "Error deleting diary entry", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountDiaryEntries(
            [FromQuery] long? studentId = null,
            [FromQuery] string? mood = null,
            [FromQuery] bool? needsFollowUp = null)
        {
            try
            {
                var count = await _diaryEntryService.CountDiaryEntriesAsync(User, studentId, mood, needsFollowUp);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting diary entries");
                return StatusCode(500, new { message = "Error counting diary entries", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<DiaryEntryDto>>> GetDiaryEntriesByStudent(long studentId)
        {
            try
            {
                var diaryEntries = await _diaryEntryService.GetDiaryEntriesByStudentAsync(studentId);
                return Ok(diaryEntries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving diary entries for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving diary entries", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/recent")]
        public async Task<ActionResult<IEnumerable<DiaryEntryDto>>> GetRecentDiaryEntries(long studentId, [FromQuery] int count = 10)
        {
            try
            {
                if (count < 1 || count > 50) count = 10;

                var diaryEntries = await _diaryEntryService.GetRecentDiaryEntriesAsync(studentId, count);
                return Ok(diaryEntries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent diary entries for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving recent diary entries", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/follow-up")]
        public async Task<ActionResult<IEnumerable<DiaryEntryDto>>> GetDiaryEntriesNeedingFollowUp(long studentId)
        {
            try
            {
                var diaryEntries = await _diaryEntryService.GetDiaryEntriesNeedingFollowUpAsync(studentId);
                return Ok(diaryEntries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving diary entries needing follow-up for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving diary entries needing follow-up", error = ex.Message });
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

                var statistics = await _diaryEntryService.GetMoodStatisticsAsync(studentId, fromDate, toDate);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mood statistics for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving mood statistics", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/entry-type-statistics")]
        public async Task<ActionResult<Dictionary<string, int>>> GetEntryTypeStatistics(
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

                var statistics = await _diaryEntryService.GetEntryTypeStatisticsAsync(studentId, fromDate, toDate);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entry type statistics for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving entry type statistics", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/average-mood-intensity")]
        public async Task<ActionResult<double>> GetAverageMoodIntensity(
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

                var average = await _diaryEntryService.GetAverageMoodIntensityAsync(studentId, fromDate, toDate);
                return Ok(new { averageMoodIntensity = average });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average mood intensity for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error calculating average mood intensity", error = ex.Message });
            }
        }

        [HttpGet("my-entries")]
        public async Task<ActionResult<IEnumerable<DiaryEntryDto>>> GetMyDiaryEntries()
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
                return Ok(new List<DiaryEntryDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's diary entries");
                return StatusCode(500, new { message = "Error retrieving diary entries", error = ex.Message });
            }
        }

        [HttpGet("my-recent-entries")]
        public async Task<ActionResult<IEnumerable<DiaryEntryDto>>> GetMyRecentDiaryEntries([FromQuery] int count = 10)
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

                // Need to get the student ID from the user ID - you'll need to implement this lookup
                // For now, return empty list
                return Ok(new List<DiaryEntryDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's recent diary entries");
                return StatusCode(500, new { message = "Error retrieving recent diary entries", error = ex.Message });
            }
        }

        [HttpGet("my-follow-up-entries")]
        public async Task<ActionResult<IEnumerable<DiaryEntryDto>>> GetMyDiaryEntriesNeedingFollowUp()
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

                return Ok(new List<DiaryEntryDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's diary entries needing follow-up");
                return StatusCode(500, new { message = "Error retrieving diary entries needing follow-up", error = ex.Message });
            }
        }
    }
}