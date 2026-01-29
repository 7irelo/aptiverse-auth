using Aptiverse.Api.Application.GoalMilestones.Dtos;
using Aptiverse.Api.Application.GoalMilestones.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/goals/{goalId}/milestones")]
    public class GoalMilestonesController(
        IGoalMilestoneService milestoneService,
        ILogger<GoalMilestonesController> logger) : ControllerBase
    {
        private readonly IGoalMilestoneService _milestoneService = milestoneService;
        private readonly ILogger<GoalMilestonesController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<GoalMilestoneDto>> CreateMilestone(
            long goalId,
            [FromBody] CreateGoalMilestoneDto createMilestoneDto)
        {
            try
            {
                if (createMilestoneDto.GoalId != goalId)
                {
                    return BadRequest(new { message = "Goal ID in path does not match Goal ID in request body" });
                }

                var createdMilestone = await _milestoneService.CreateMilestoneAsync(createMilestoneDto);
                return CreatedAtAction(
                    nameof(GetMilestone),
                    new { goalId, id = createdMilestone.Id },
                    createdMilestone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating milestone for goal {GoalId}", goalId);
                return BadRequest(new { message = "Error creating milestone", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GoalMilestoneDto>> GetMilestone(long goalId, long id)
        {
            try
            {
                var milestone = await _milestoneService.GetMilestoneByIdAsync(id);

                if (milestone == null)
                    return NotFound(new { message = $"Goal milestone with ID {id} not found" });

                if (milestone.GoalId != goalId)
                {
                    return NotFound(new { message = $"Goal milestone with ID {id} not found in goal {goalId}" });
                }

                return Ok(milestone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving milestone with ID {MilestoneId} for goal {GoalId}", id, goalId);
                return StatusCode(500, new { message = "Error retrieving milestone", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GoalMilestoneDto>>> GetMilestones(
            long goalId,
            [FromQuery] bool? isCompleted = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _milestoneService.GetMilestonesAsync(
                    currentUser: User,
                    goalId: goalId,
                    isCompleted: isCompleted,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving milestones for goal {GoalId}", goalId);
                return StatusCode(500, new { message = "Error retrieving milestones", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GoalMilestoneDto>> UpdateMilestone(
            long goalId,
            long id,
            [FromBody] UpdateGoalMilestoneDto updateMilestoneDto)
        {
            try
            {
                var milestone = await _milestoneService.GetMilestoneByIdAsync(id);
                if (milestone == null || milestone.GoalId != goalId)
                {
                    return NotFound(new { message = $"Goal milestone with ID {id} not found in goal {goalId}" });
                }

                var updatedMilestone = await _milestoneService.UpdateMilestoneAsync(id, updateMilestoneDto);
                return Ok(updatedMilestone);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Goal milestone with ID {MilestoneId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating milestone with ID {MilestoneId} for goal {GoalId}", id, goalId);
                return BadRequest(new { message = "Error updating milestone", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMilestone(long goalId, long id)
        {
            try
            {
                var milestone = await _milestoneService.GetMilestoneByIdAsync(id);
                if (milestone == null || milestone.GoalId != goalId)
                {
                    return NotFound(new { message = $"Goal milestone with ID {id} not found in goal {goalId}" });
                }

                var result = await _milestoneService.DeleteMilestoneAsync(id);

                if (!result)
                    return NotFound(new { message = $"Goal milestone with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting milestone with ID {MilestoneId} from goal {GoalId}", id, goalId);
                return StatusCode(500, new { message = "Error deleting milestone", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountMilestones(long goalId, [FromQuery] bool? isCompleted = null)
        {
            try
            {
                var count = await _milestoneService.CountMilestonesAsync(User, goalId, isCompleted);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting milestones for goal {GoalId}", goalId);
                return StatusCode(500, new { message = "Error counting milestones", error = ex.Message });
            }
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<GoalMilestoneDto>>> ListMilestones(long goalId)
        {
            try
            {
                var milestones = await _milestoneService.GetMilestonesByGoalAsync(goalId);
                return Ok(milestones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving milestone list for goal {GoalId}", goalId);
                return StatusCode(500, new { message = "Error retrieving milestones", error = ex.Message });
            }
        }

        [HttpGet("completed")]
        public async Task<ActionResult<IEnumerable<GoalMilestoneDto>>> GetCompletedMilestones(long goalId)
        {
            try
            {
                var milestones = await _milestoneService.GetCompletedMilestonesAsync(goalId);
                return Ok(milestones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving completed milestones for goal {GoalId}", goalId);
                return StatusCode(500, new { message = "Error retrieving completed milestones", error = ex.Message });
            }
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<GoalMilestoneDto>>> GetPendingMilestones(long goalId)
        {
            try
            {
                var milestones = await _milestoneService.GetPendingMilestonesAsync(goalId);
                return Ok(milestones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending milestones for goal {GoalId}", goalId);
                return StatusCode(500, new { message = "Error retrieving pending milestones", error = ex.Message });
            }
        }

        [HttpPut("{id}/complete")]
        public async Task<ActionResult<GoalMilestoneDto>> MarkMilestoneComplete(long goalId, long id)
        {
            try
            {
                var milestone = await _milestoneService.GetMilestoneByIdAsync(id);
                if (milestone == null || milestone.GoalId != goalId)
                {
                    return NotFound(new { message = $"Goal milestone with ID {id} not found in goal {goalId}" });
                }

                var updatedMilestone = await _milestoneService.MarkMilestoneCompleteAsync(id);
                return Ok(updatedMilestone);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Goal milestone with ID {MilestoneId} not found for completion", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking milestone with ID {MilestoneId} as complete for goal {GoalId}", id, goalId);
                return BadRequest(new { message = "Error marking milestone as complete", error = ex.Message });
            }
        }

        [HttpPut("{id}/incomplete")]
        public async Task<ActionResult<GoalMilestoneDto>> MarkMilestoneIncomplete(long goalId, long id)
        {
            try
            {
                var milestone = await _milestoneService.GetMilestoneByIdAsync(id);
                if (milestone == null || milestone.GoalId != goalId)
                {
                    return NotFound(new { message = $"Goal milestone with ID {id} not found in goal {goalId}" });
                }

                var updatedMilestone = await _milestoneService.MarkMilestoneIncompleteAsync(id);
                return Ok(updatedMilestone);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Goal milestone with ID {MilestoneId} not found for incompletion", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking milestone with ID {MilestoneId} as incomplete for goal {GoalId}", id, goalId);
                return BadRequest(new { message = "Error marking milestone as incomplete", error = ex.Message });
            }
        }

        [HttpGet("total-reward-points")]
        public async Task<ActionResult<int>> GetTotalRewardPoints(long goalId)
        {
            try
            {
                var totalPoints = await _milestoneService.GetTotalRewardPointsAsync(goalId);
                return Ok(new { totalPoints });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total reward points for goal {GoalId}", goalId);
                return StatusCode(500, new { message = "Error calculating total reward points", error = ex.Message });
            }
        }
    }
}