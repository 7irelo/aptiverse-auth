using Aptiverse.Api.Application.RewardFeatures.Dtos;
using Aptiverse.Api.Application.RewardFeatures.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/reward-features")]
    public class RewardFeaturesController(
        IRewardFeatureService rewardFeatureService,
        ILogger<RewardFeaturesController> logger) : ControllerBase
    {
        private readonly IRewardFeatureService _rewardFeatureService = rewardFeatureService;
        private readonly ILogger<RewardFeaturesController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<RewardFeatureDto>> CreateRewardFeature([FromBody] CreateRewardFeatureDto createRewardFeatureDto)
        {
            try
            {
                var createdRewardFeature = await _rewardFeatureService.CreateRewardFeatureAsync(createRewardFeatureDto);
                return CreatedAtAction(nameof(GetRewardFeature), new { id = createdRewardFeature.Id }, createdRewardFeature);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reward feature");
                return BadRequest(new { message = "Error creating reward feature", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RewardFeatureDto>> GetRewardFeature(long id)
        {
            try
            {
                var rewardFeature = await _rewardFeatureService.GetRewardFeatureByIdAsync(id);

                if (rewardFeature == null)
                    return NotFound(new { message = $"Reward feature with ID {id} not found" });

                return Ok(rewardFeature);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reward feature with ID {RewardFeatureId}", id);
                return StatusCode(500, new { message = "Error retrieving reward feature", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<RewardFeatureDto>>> GetRewardFeatures(
            [FromQuery] long? rewardId = null,
            [FromQuery] long? featureId = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _rewardFeatureService.GetRewardFeaturesAsync(
                    currentUser: User,
                    rewardId: rewardId,
                    featureId: featureId,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reward features");
                return StatusCode(500, new { message = "Error retrieving reward features", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RewardFeatureDto>> UpdateRewardFeature(long id, [FromBody] UpdateRewardFeatureDto updateRewardFeatureDto)
        {
            try
            {
                var updatedRewardFeature = await _rewardFeatureService.UpdateRewardFeatureAsync(id, updateRewardFeatureDto);
                return Ok(updatedRewardFeature);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Reward feature with ID {RewardFeatureId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reward feature with ID {RewardFeatureId}", id);
                return BadRequest(new { message = "Error updating reward feature", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRewardFeature(long id)
        {
            try
            {
                var result = await _rewardFeatureService.DeleteRewardFeatureAsync(id);

                if (!result)
                    return NotFound(new { message = $"Reward feature with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting reward feature with ID {RewardFeatureId}", id);
                return StatusCode(500, new { message = "Error deleting reward feature", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountRewardFeatures(
            [FromQuery] long? rewardId = null,
            [FromQuery] long? featureId = null)
        {
            try
            {
                var count = await _rewardFeatureService.CountRewardFeaturesAsync(User, rewardId, featureId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting reward features");
                return StatusCode(500, new { message = "Error counting reward features", error = ex.Message });
            }
        }

        [HttpGet("reward/{rewardId}")]
        public async Task<ActionResult<IEnumerable<RewardFeatureDto>>> GetRewardFeaturesByReward(long rewardId)
        {
            try
            {
                var rewardFeatures = await _rewardFeatureService.GetRewardFeaturesByRewardAsync(rewardId);
                return Ok(rewardFeatures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reward features for reward {RewardId}", rewardId);
                return StatusCode(500, new { message = "Error retrieving reward features", error = ex.Message });
            }
        }

        [HttpGet("feature/{featureId}")]
        public async Task<ActionResult<IEnumerable<RewardFeatureDto>>> GetRewardFeaturesByFeature(long featureId)
        {
            try
            {
                var rewardFeatures = await _rewardFeatureService.GetRewardFeaturesByFeatureAsync(featureId);
                return Ok(rewardFeatures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reward features for feature {FeatureId}", featureId);
                return StatusCode(500, new { message = "Error retrieving reward features", error = ex.Message });
            }
        }

        [HttpGet("exists/{rewardId}/{featureId}")]
        public async Task<ActionResult<bool>> RewardFeatureExists(long rewardId, long featureId)
        {
            try
            {
                var exists = await _rewardFeatureService.ExistsAsync(rewardId, featureId);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if reward feature exists for reward {RewardId} and feature {FeatureId}", rewardId, featureId);
                return StatusCode(500, new { message = "Error checking reward feature existence", error = ex.Message });
            }
        }
    }
}