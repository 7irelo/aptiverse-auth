using Aptiverse.Api.Application.DiaryGoals.Dtos;
using Aptiverse.Api.Application.DiaryGoals.Services;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/diary-goals")]
    public class DiaryGoalsController(
        IDiaryGoalService diaryGoalService,
        ILogger<DiaryGoalsController> logger) : ControllerBase
    {
        private readonly IDiaryGoalService _diaryGoalService = diaryGoalService;
        private readonly ILogger<DiaryGoalsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<DiaryGoalDto>> CreateDiaryGoal([FromBody] CreateDiaryGoalDto createDiaryGoalDto)
        {
            try
            {
                var createdDiaryGoal = await _diaryGoalService.CreateDiaryGoalAsync(createDiaryGoalDto);
                return CreatedAtAction(nameof(GetDiaryGoal), new { id = createdDiaryGoal.Id }, createdDiaryGoal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating diary goal connection");
                return BadRequest(new { message = "Error creating diary goal connection", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DiaryGoalDto>> GetDiaryGoal(long id)
        {
            try
            {
                var diaryGoal = await _diaryGoalService.GetDiaryGoalByIdAsync(id);

                if (diaryGoal == null)
                    return NotFound(new { message = $"Diary goal connection with ID {id} not found" });

                return Ok(diaryGoal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving diary goal connection with ID {DiaryGoalId}", id);
                return StatusCode(500, new { message = "Error retrieving diary goal connection", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<DiaryGoalDto>>> GetDiaryGoals(
            [FromQuery] long? diaryEntryId = null,
            [FromQuery] long? goalId = null,
            [FromQuery] string? connectionType = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _diaryGoalService.GetDiaryGoalsAsync(
                    currentUser: User,
                    diaryEntryId: diaryEntryId,
                    goalId: goalId,
                    connectionType: connectionType,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving diary goal connections");
                return StatusCode(500, new { message = "Error retrieving diary goal connections", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DiaryGoalDto>> UpdateDiaryGoal(long id, [FromBody] UpdateDiaryGoalDto updateDiaryGoalDto)
        {
            try
            {
                var updatedDiaryGoal = await _diaryGoalService.UpdateDiaryGoalAsync(id, updateDiaryGoalDto);
                return Ok(updatedDiaryGoal);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Diary goal connection with ID {DiaryGoalId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating diary goal connection with ID {DiaryGoalId}", id);
                return BadRequest(new { message = "Error updating diary goal connection", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiaryGoal(long id)
        {
            try
            {
                var result = await _diaryGoalService.DeleteDiaryGoalAsync(id);

                if (!result)
                    return NotFound(new { message = $"Diary goal connection with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting diary goal connection with ID {DiaryGoalId}", id);
                return StatusCode(500, new { message = "Error deleting diary goal connection", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountDiaryGoals(
            [FromQuery] long? diaryEntryId = null,
            [FromQuery] long? goalId = null,
            [FromQuery] string? connectionType = null)
        {
            try
            {
                var count = await _diaryGoalService.CountDiaryGoalsAsync(User, diaryEntryId, goalId, connectionType);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting diary goal connections");
                return StatusCode(500, new { message = "Error counting diary goal connections", error = ex.Message });
            }
        }

        [HttpGet("diary-entry/{diaryEntryId}")]
        public async Task<ActionResult<IEnumerable<DiaryGoalDto>>> GetDiaryGoalsByDiaryEntry(long diaryEntryId)
        {
            try
            {
                var diaryGoals = await _diaryGoalService.GetDiaryGoalsByDiaryEntryAsync(diaryEntryId);
                return Ok(diaryGoals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving diary goal connections for diary entry {DiaryEntryId}", diaryEntryId);
                return StatusCode(500, new { message = "Error retrieving diary goal connections", error = ex.Message });
            }
        }

        [HttpGet("goal/{goalId}")]
        public async Task<ActionResult<IEnumerable<DiaryGoalDto>>> GetDiaryGoalsByGoal(long goalId)
        {
            try
            {
                var diaryGoals = await _diaryGoalService.GetDiaryGoalsByGoalAsync(goalId);
                return Ok(diaryGoals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving diary goal connections for goal {GoalId}", goalId);
                return StatusCode(500, new { message = "Error retrieving diary goal connections", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<DiaryGoalDto>>> GetDiaryGoalsByStudent(long studentId)
        {
            try
            {
                var diaryGoals = await _diaryGoalService.GetDiaryGoalsByStudentAsync(studentId);
                return Ok(diaryGoals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving diary goal connections for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving diary goal connections", error = ex.Message });
            }
        }

        [HttpGet("exists/{diaryEntryId}/{goalId}")]
        public async Task<ActionResult<bool>> DiaryGoalExists(long diaryEntryId, long goalId)
        {
            try
            {
                var exists = await _diaryGoalService.ExistsAsync(diaryEntryId, goalId);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if diary goal connection exists for diary entry {DiaryEntryId} and goal {GoalId}", diaryEntryId, goalId);
                return StatusCode(500, new { message = "Error checking diary goal connection existence", error = ex.Message });
            }
        }

        [HttpGet("my-goal-connections")]
        public async Task<ActionResult<IEnumerable<DiaryGoalDto>>> GetMyDiaryGoalConnections()
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
                return Ok(new List<DiaryGoalDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's diary goal connections");
                return StatusCode(500, new { message = "Error retrieving diary goal connections", error = ex.Message });
            }
        }
    }
}