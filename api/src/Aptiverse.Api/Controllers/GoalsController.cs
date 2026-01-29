using Aptiverse.Api.Application.Goals.Dtos;
using Aptiverse.Api.Application.Goals.Services;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/goals")]
    public class GoalsController(
        IGoalService goalService,
        ILogger<GoalsController> logger) : ControllerBase
    {
        private readonly IGoalService _goalService = goalService;
        private readonly ILogger<GoalsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<GoalDto>> CreateGoal([FromBody] CreateGoalDto createGoalDto)
        {
            try
            {
                var createdGoal = await _goalService.CreateGoalAsync(createGoalDto);
                return CreatedAtAction(nameof(GetGoal), new { id = createdGoal.Id }, createdGoal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating goal");
                return BadRequest(new { message = "Error creating goal", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GoalDto>> GetGoal(long id)
        {
            try
            {
                var goal = await _goalService.GetGoalByIdAsync(id);

                if (goal == null)
                    return NotFound(new { message = $"Goal with ID {id} not found" });

                return Ok(goal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving goal with ID {GoalId}", id);
                return StatusCode(500, new { message = "Error retrieving goal", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GoalDto>>> GetGoals(
            [FromQuery] long? studentId = null,
            [FromQuery] string? goalType = null,
            [FromQuery] string? subjectId = null,
            [FromQuery] string? status = null,
            [FromQuery] int? priority = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? sortBy = "TargetDate",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                {
                    return BadRequest(new { message = "startDate cannot be greater than endDate" });
                }

                var result = await _goalService.GetGoalsAsync(
                    currentUser: User,
                    studentId: studentId,
                    goalType: goalType,
                    subjectId: subjectId,
                    status: status,
                    priority: priority,
                    startDate: startDate,
                    endDate: endDate,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving goals");
                return StatusCode(500, new { message = "Error retrieving goals", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GoalDto>> UpdateGoal(long id, [FromBody] UpdateGoalDto updateGoalDto)
        {
            try
            {
                var updatedGoal = await _goalService.UpdateGoalAsync(id, updateGoalDto);
                return Ok(updatedGoal);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Goal with ID {GoalId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating goal with ID {GoalId}", id);
                return BadRequest(new { message = "Error updating goal", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(long id)
        {
            try
            {
                var result = await _goalService.DeleteGoalAsync(id);

                if (!result)
                    return NotFound(new { message = $"Goal with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting goal with ID {GoalId}", id);
                return StatusCode(500, new { message = "Error deleting goal", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountGoals(
            [FromQuery] long? studentId = null,
            [FromQuery] string? goalType = null,
            [FromQuery] string? status = null)
        {
            try
            {
                var count = await _goalService.CountGoalsAsync(User, studentId, goalType, status);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting goals");
                return StatusCode(500, new { message = "Error counting goals", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<GoalDto>>> GetGoalsByStudent(long studentId)
        {
            try
            {
                var goals = await _goalService.GetGoalsByStudentAsync(studentId);
                return Ok(goals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving goals for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving goals", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/active")]
        public async Task<ActionResult<IEnumerable<GoalDto>>> GetActiveGoals(long studentId)
        {
            try
            {
                var goals = await _goalService.GetActiveGoalsAsync(studentId);
                return Ok(goals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active goals for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving active goals", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/completed")]
        public async Task<ActionResult<IEnumerable<GoalDto>>> GetCompletedGoals(long studentId)
        {
            try
            {
                var goals = await _goalService.GetCompletedGoalsAsync(studentId);
                return Ok(goals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving completed goals for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving completed goals", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/overdue")]
        public async Task<ActionResult<IEnumerable<GoalDto>>> GetOverdueGoals(long studentId)
        {
            try
            {
                var goals = await _goalService.GetOverdueGoalsAsync(studentId);
                return Ok(goals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving overdue goals for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving overdue goals", error = ex.Message });
            }
        }

        [HttpPut("{id}/progress")]
        public async Task<ActionResult<GoalDto>> UpdateGoalProgress(long id, [FromBody] decimal currentValue)
        {
            try
            {
                if (currentValue < 0)
                {
                    return BadRequest(new { message = "Current value cannot be negative" });
                }

                var updatedGoal = await _goalService.UpdateGoalProgressAsync(id, currentValue);
                return Ok(updatedGoal);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Goal with ID {GoalId} not found for progress update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating goal progress with ID {GoalId}", id);
                return BadRequest(new { message = "Error updating goal progress", error = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<GoalDto>> UpdateGoalStatus(long id, [FromBody] string status)
        {
            try
            {
                var validStatuses = new[] { "NotStarted", "InProgress", "Completed", "Failed" };
                if (!validStatuses.Contains(status))
                {
                    return BadRequest(new { message = "Invalid status value" });
                }

                var updatedGoal = await _goalService.UpdateGoalStatusAsync(id, status);
                return Ok(updatedGoal);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Goal with ID {GoalId} not found for status update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating goal status with ID {GoalId}", id);
                return BadRequest(new { message = "Error updating goal status", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/overall-progress")]
        public async Task<ActionResult<decimal>> GetStudentOverallProgress(long studentId)
        {
            try
            {
                var overallProgress = await _goalService.GetStudentOverallProgressAsync(studentId);
                return Ok(new { overallProgress });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating overall progress for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error calculating overall progress", error = ex.Message });
            }
        }

        [HttpGet("my-goals")]
        public async Task<ActionResult<IEnumerable<GoalDto>>> GetMyGoals()
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

                var studentId = await GetStudentIdForUser(userId);
                if (!studentId.HasValue)
                {
                    return NotFound(new { message = "Student profile not found for current user" });
                }

                var goals = await _goalService.GetGoalsByStudentAsync(studentId.Value);
                return Ok(goals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user's goals");
                return StatusCode(500, new { message = "Error retrieving goals", error = ex.Message });
            }
        }

        [HttpGet("my-active-goals")]
        public async Task<ActionResult<IEnumerable<GoalDto>>> GetMyActiveGoals()
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

                var studentId = await GetStudentIdForUser(userId);
                if (!studentId.HasValue)
                {
                    return NotFound(new { message = "Student profile not found for current user" });
                }

                var goals = await _goalService.GetActiveGoalsAsync(studentId.Value);
                return Ok(goals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user's active goals");
                return StatusCode(500, new { message = "Error retrieving active goals", error = ex.Message });
            }
        }

        private async Task<long?> GetStudentIdForUser(string userId)
        {
            return null;
        }
    }
}