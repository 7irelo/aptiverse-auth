using Aptiverse.Api.Application.Features.Dtos;
using Aptiverse.Api.Application.Features.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/features")]
    public class FeaturesController(
        IFeatureService featureService,
        ILogger<FeaturesController> logger) : ControllerBase
    {
        private readonly IFeatureService _featureService = featureService;
        private readonly ILogger<FeaturesController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<FeatureDto>> CreateFeature([FromBody] CreateFeatureDto createFeatureDto)
        {
            try
            {
                var createdFeature = await _featureService.CreateFeatureAsync(createFeatureDto);
                return CreatedAtAction(nameof(GetFeature), new { id = createdFeature.Id }, createdFeature);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating feature");
                return BadRequest(new { message = "Error creating feature", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FeatureDto>> GetFeature(long id)
        {
            try
            {
                var feature = await _featureService.GetFeatureByIdAsync(id);

                if (feature == null)
                    return NotFound(new { message = $"Feature with ID {id} not found" });

                return Ok(feature);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feature with ID {FeatureId}", id);
                return StatusCode(500, new { message = "Error retrieving feature", error = ex.Message });
            }
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<FeatureDto>> GetFeatureByCode(string code)
        {
            try
            {
                var feature = await _featureService.GetFeatureByCodeAsync(code);

                if (feature == null)
                    return NotFound(new { message = $"Feature with code '{code}' not found" });

                return Ok(feature);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feature with code {FeatureCode}", code);
                return StatusCode(500, new { message = "Error retrieving feature", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<FeatureDto>>> GetFeatures(
            [FromQuery] string? search = null,
            [FromQuery] string? category = null,
            [FromQuery] string? billingCycle = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                {
                    return BadRequest(new { message = "minPrice cannot be greater than maxPrice" });
                }

                var result = await _featureService.GetFeaturesAsync(
                    currentUser: User,
                    search: search,
                    category: category,
                    billingCycle: billingCycle,
                    isActive: isActive,
                    minPrice: minPrice,
                    maxPrice: maxPrice,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving features");
                return StatusCode(500, new { message = "Error retrieving features", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FeatureDto>> UpdateFeature(long id, [FromBody] UpdateFeatureDto updateFeatureDto)
        {
            try
            {
                var updatedFeature = await _featureService.UpdateFeatureAsync(id, updateFeatureDto);
                return Ok(updatedFeature);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Feature with ID {FeatureId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating feature with ID {FeatureId}", id);
                return BadRequest(new { message = "Error updating feature", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeature(long id)
        {
            try
            {
                var result = await _featureService.DeleteFeatureAsync(id);

                if (!result)
                    return NotFound(new { message = $"Feature with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting feature with ID {FeatureId}", id);
                return StatusCode(500, new { message = "Error deleting feature", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountFeatures(
            [FromQuery] string? category = null,
            [FromQuery] string? billingCycle = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var count = await _featureService.CountFeaturesAsync(User, category, billingCycle, isActive);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting features");
                return StatusCode(500, new { message = "Error counting features", error = ex.Message });
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<FeatureDto>>> GetActiveFeatures()
        {
            try
            {
                var features = await _featureService.GetActiveFeaturesAsync();
                return Ok(features);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active features");
                return StatusCode(500, new { message = "Error retrieving active features", error = ex.Message });
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<FeatureDto>>> GetFeaturesByCategory(string category)
        {
            try
            {
                var features = await _featureService.GetFeaturesByCategoryAsync(category);
                return Ok(features);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving features for category {Category}", category);
                return StatusCode(500, new { message = "Error retrieving features", error = ex.Message });
            }
        }

        [HttpGet("user/features")]
        public async Task<ActionResult<IEnumerable<FeatureDto>>> GetFeaturesForCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var features = await _featureService.GetFeaturesForUserAsync(userId);
                return Ok(features);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving features for current user");
                return StatusCode(500, new { message = "Error retrieving features", error = ex.Message });
            }
        }
    }
}