using Aptiverse.Api.Application.Rewards.Dtos;
using Aptiverse.Api.Application.Rewards.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/rewards")]
    public class RewardsController(
        IRewardService rewardService,
        ILogger<RewardsController> logger) : ControllerBase
    {
        private readonly IRewardService _rewardService = rewardService;
        private readonly ILogger<RewardsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<RewardDto>> CreateReward([FromBody] CreateRewardDto createRewardDto)
        {
            try
            {
                var createdReward = await _rewardService.CreateRewardAsync(createRewardDto);
                return CreatedAtAction(nameof(GetReward), new { id = createdReward.Id }, createdReward);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reward");
                return BadRequest(new { message = "Error creating reward", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RewardDto>> GetReward(long id)
        {
            try
            {
                var reward = await _rewardService.GetRewardByIdAsync(id);

                if (reward == null)
                    return NotFound(new { message = $"Reward with ID {id} not found" });

                return Ok(reward);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reward with ID {RewardId}", id);
                return StatusCode(500, new { message = "Error retrieving reward", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<RewardDto>>> GetRewards(
            [FromQuery] string? search = null,
            [FromQuery] string? rewardType = null,
            [FromQuery] int? difficultyTier = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _rewardService.GetRewardsAsync(
                    search: search,
                    rewardType: rewardType,
                    difficultyTier: difficultyTier,
                    isActive: isActive,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rewards");
                return StatusCode(500, new { message = "Error retrieving rewards", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RewardDto>> UpdateReward(long id, [FromBody] UpdateRewardDto updateRewardDto)
        {
            try
            {
                var updatedReward = await _rewardService.UpdateRewardAsync(id, updateRewardDto);
                return Ok(updatedReward);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Reward with ID {RewardId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reward with ID {RewardId}", id);
                return BadRequest(new { message = "Error updating reward", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReward(long id)
        {
            try
            {
                var result = await _rewardService.DeleteRewardAsync(id);

                if (!result)
                    return NotFound(new { message = $"Reward with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting reward with ID {RewardId}", id);
                return StatusCode(500, new { message = "Error deleting reward", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountRewards(
            [FromQuery] string? rewardType = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var count = await _rewardService.CountRewardsAsync(rewardType, isActive);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting rewards");
                return StatusCode(500, new { message = "Error counting rewards", error = ex.Message });
            }
        }
    }
}