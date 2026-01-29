using Aptiverse.Api.Application.UserFeatures.Dtos;
using Aptiverse.Api.Application.UserFeatures.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/user-features")]
    public class UserFeaturesController(
        IUserFeatureService userFeatureService,
        ILogger<UserFeaturesController> logger) : ControllerBase
    {
        private readonly IUserFeatureService _userFeatureService = userFeatureService;
        private readonly ILogger<UserFeaturesController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<UserFeatureDto>> CreateUserFeature([FromBody] CreateUserFeatureDto createUserFeatureDto)
        {
            try
            {
                var createdUserFeature = await _userFeatureService.CreateUserFeatureAsync(createUserFeatureDto);
                return CreatedAtAction(nameof(GetUserFeature), new { id = createdUserFeature.Id }, createdUserFeature);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user feature");
                return BadRequest(new { message = "Error creating user feature", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserFeatureDto>> GetUserFeature(long id)
        {
            try
            {
                var userFeature = await _userFeatureService.GetUserFeatureByIdAsync(id);

                if (userFeature == null)
                    return NotFound(new { message = $"User feature with ID {id} not found" });

                return Ok(userFeature);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user feature with ID {UserFeatureId}", id);
                return StatusCode(500, new { message = "Error retrieving user feature", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<UserFeatureDto>>> GetUserFeatures(
            [FromQuery] string? userId = null,
            [FromQuery] long? featureId = null,
            [FromQuery] string? grantType = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = "GrantedAt",
            [FromQuery] bool sortDescending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _userFeatureService.GetUserFeaturesAsync(
                    currentUser: User,
                    userId: userId,
                    featureId: featureId,
                    grantType: grantType,
                    isActive: isActive,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user features");
                return StatusCode(500, new { message = "Error retrieving user features", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserFeatureDto>> UpdateUserFeature(long id, [FromBody] UpdateUserFeatureDto updateUserFeatureDto)
        {
            try
            {
                var updatedUserFeature = await _userFeatureService.UpdateUserFeatureAsync(id, updateUserFeatureDto);
                return Ok(updatedUserFeature);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User feature with ID {UserFeatureId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user feature with ID {UserFeatureId}", id);
                return BadRequest(new { message = "Error updating user feature", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserFeature(long id)
        {
            try
            {
                var result = await _userFeatureService.DeleteUserFeatureAsync(id);

                if (!result)
                    return NotFound(new { message = $"User feature with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user feature with ID {UserFeatureId}", id);
                return StatusCode(500, new { message = "Error deleting user feature", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountUserFeatures(
            [FromQuery] string? userId = null,
            [FromQuery] long? featureId = null,
            [FromQuery] string? grantType = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var count = await _userFeatureService.CountUserFeaturesAsync(User, userId, featureId, grantType, isActive);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting user features");
                return StatusCode(500, new { message = "Error counting user features", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserFeatureDto>>> GetUserFeaturesByUser(string userId)
        {
            try
            {
                var userFeatures = await _userFeatureService.GetUserFeaturesByUserAsync(userId);
                return Ok(userFeatures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user features for user {UserId}", userId);
                return StatusCode(500, new { message = "Error retrieving user features", error = ex.Message });
            }
        }

        [HttpGet("feature/{featureId}")]
        public async Task<ActionResult<IEnumerable<UserFeatureDto>>> GetUserFeaturesByFeature(long featureId)
        {
            try
            {
                var userFeatures = await _userFeatureService.GetUserFeaturesByFeatureAsync(featureId);
                return Ok(userFeatures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user features for feature {FeatureId}", featureId);
                return StatusCode(500, new { message = "Error retrieving user features", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}/active")]
        public async Task<ActionResult<IEnumerable<UserFeatureDto>>> GetActiveUserFeatures(string userId)
        {
            try
            {
                var userFeatures = await _userFeatureService.GetActiveUserFeaturesAsync(userId);
                return Ok(userFeatures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active user features for user {UserId}", userId);
                return StatusCode(500, new { message = "Error retrieving active user features", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}/feature/{featureId}/active")]
        public async Task<ActionResult<bool>> HasActiveFeature(string userId, long featureId)
        {
            try
            {
                var hasActiveFeature = await _userFeatureService.HasActiveFeatureAsync(userId, featureId);
                return Ok(new { hasActiveFeature });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking active feature for user {UserId} and feature {FeatureId}", userId, featureId);
                return StatusCode(500, new { message = "Error checking active feature", error = ex.Message });
            }
        }

        [HttpPost("user/{userId}/any-active")]
        public async Task<ActionResult<bool>> HasAnyActiveFeature(string userId, [FromBody] List<long> featureIds)
        {
            try
            {
                var hasAnyActiveFeature = await _userFeatureService.HasAnyActiveFeatureAsync(userId, featureIds);
                return Ok(new { hasAnyActiveFeature });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {UserId} has any active features from list", userId);
                return StatusCode(500, new { message = "Error checking active features", error = ex.Message });
            }
        }

        [HttpGet("exists/{userId}/{featureId}")]
        public async Task<ActionResult<bool>> UserFeatureExists(string userId, long featureId)
        {
            try
            {
                var exists = await _userFeatureService.ExistsAsync(userId, featureId);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user feature exists for user {UserId} and feature {FeatureId}", userId, featureId);
                return StatusCode(500, new { message = "Error checking user feature existence", error = ex.Message });
            }
        }

        [HttpGet("my-features")]
        public async Task<ActionResult<IEnumerable<UserFeatureDto>>> GetMyUserFeatures()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var userFeatures = await _userFeatureService.GetUserFeaturesByUserAsync(userId);
                return Ok(userFeatures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user's features");
                return StatusCode(500, new { message = "Error retrieving user features", error = ex.Message });
            }
        }

        [HttpGet("my-active-features")]
        public async Task<ActionResult<IEnumerable<UserFeatureDto>>> GetMyActiveUserFeatures()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var userFeatures = await _userFeatureService.GetActiveUserFeaturesAsync(userId);
                return Ok(userFeatures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user's active features");
                return StatusCode(500, new { message = "Error retrieving active user features", error = ex.Message });
            }
        }

        [HttpGet("my-feature/{featureId}/active")]
        public async Task<ActionResult<bool>> CheckMyActiveFeature(long featureId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var hasActiveFeature = await _userFeatureService.HasActiveFeatureAsync(userId, featureId);
                return Ok(new { hasActiveFeature });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if current user has active feature {FeatureId}", featureId);
                return StatusCode(500, new { message = "Error checking active feature", error = ex.Message });
            }
        }
    }
}