using Aptiverse.Api.Application.RoleFeatures.Dtos;
using Aptiverse.Api.Application.RoleFeatures.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/role-features")]
    public class RoleFeaturesController(
        IRoleFeatureService roleFeatureService,
        ILogger<RoleFeaturesController> logger) : ControllerBase
    {
        private readonly IRoleFeatureService _roleFeatureService = roleFeatureService;
        private readonly ILogger<RoleFeaturesController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<RoleFeatureDto>> CreateRoleFeature([FromBody] CreateRoleFeatureDto createRoleFeatureDto)
        {
            try
            {
                var createdRoleFeature = await _roleFeatureService.CreateRoleFeatureAsync(createRoleFeatureDto);
                return CreatedAtAction(nameof(GetRoleFeature), new { id = createdRoleFeature.Id }, createdRoleFeature);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role feature");
                return BadRequest(new { message = "Error creating role feature", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleFeatureDto>> GetRoleFeature(long id)
        {
            try
            {
                var roleFeature = await _roleFeatureService.GetRoleFeatureByIdAsync(id);

                if (roleFeature == null)
                    return NotFound(new { message = $"Role feature with ID {id} not found" });

                return Ok(roleFeature);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role feature with ID {RoleFeatureId}", id);
                return StatusCode(500, new { message = "Error retrieving role feature", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<RoleFeatureDto>>> GetRoleFeatures(
            [FromQuery] string? roleName = null,
            [FromQuery] long? featureId = null,
            [FromQuery] bool? isDefault = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _roleFeatureService.GetRoleFeaturesAsync(
                    currentUser: User,
                    roleName: roleName,
                    featureId: featureId,
                    isDefault: isDefault,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role features");
                return StatusCode(500, new { message = "Error retrieving role features", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RoleFeatureDto>> UpdateRoleFeature(long id, [FromBody] UpdateRoleFeatureDto updateRoleFeatureDto)
        {
            try
            {
                var updatedRoleFeature = await _roleFeatureService.UpdateRoleFeatureAsync(id, updateRoleFeatureDto);
                return Ok(updatedRoleFeature);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Role feature with ID {RoleFeatureId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role feature with ID {RoleFeatureId}", id);
                return BadRequest(new { message = "Error updating role feature", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoleFeature(long id)
        {
            try
            {
                var result = await _roleFeatureService.DeleteRoleFeatureAsync(id);

                if (!result)
                    return NotFound(new { message = $"Role feature with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role feature with ID {RoleFeatureId}", id);
                return StatusCode(500, new { message = "Error deleting role feature", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountRoleFeatures(
            [FromQuery] string? roleName = null,
            [FromQuery] long? featureId = null,
            [FromQuery] bool? isDefault = null)
        {
            try
            {
                var count = await _roleFeatureService.CountRoleFeaturesAsync(User, roleName, featureId, isDefault);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting role features");
                return StatusCode(500, new { message = "Error counting role features", error = ex.Message });
            }
        }

        [HttpGet("role/{roleName}")]
        public async Task<ActionResult<IEnumerable<RoleFeatureDto>>> GetRoleFeaturesByRole(string roleName)
        {
            try
            {
                var roleFeatures = await _roleFeatureService.GetRoleFeaturesByRoleAsync(roleName);
                return Ok(roleFeatures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role features for role {RoleName}", roleName);
                return StatusCode(500, new { message = "Error retrieving role features", error = ex.Message });
            }
        }

        [HttpGet("feature/{featureId}")]
        public async Task<ActionResult<IEnumerable<RoleFeatureDto>>> GetRoleFeaturesByFeature(long featureId)
        {
            try
            {
                var roleFeatures = await _roleFeatureService.GetRoleFeaturesByFeatureAsync(featureId);
                return Ok(roleFeatures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role features for feature {FeatureId}", featureId);
                return StatusCode(500, new { message = "Error retrieving role features", error = ex.Message });
            }
        }

        [HttpGet("role/{roleName}/defaults")]
        public async Task<ActionResult<IEnumerable<RoleFeatureDto>>> GetDefaultFeaturesForRole(string roleName)
        {
            try
            {
                var roleFeatures = await _roleFeatureService.GetDefaultFeaturesForRoleAsync(roleName);
                return Ok(roleFeatures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving default features for role {RoleName}", roleName);
                return StatusCode(500, new { message = "Error retrieving default features", error = ex.Message });
            }
        }

        [HttpGet("access/{roleName}/{featureId}")]
        public async Task<ActionResult<bool>> HasFeatureAccess(string roleName, long featureId)
        {
            try
            {
                var hasAccess = await _roleFeatureService.HasFeatureAccessAsync(roleName, featureId);
                return Ok(new { hasAccess });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking feature access for role {RoleName} and feature {FeatureId}", roleName, featureId);
                return StatusCode(500, new { message = "Error checking feature access", error = ex.Message });
            }
        }

        [HttpGet("exists/{roleName}/{featureId}")]
        public async Task<ActionResult<bool>> RoleFeatureExists(string roleName, long featureId)
        {
            try
            {
                var exists = await _roleFeatureService.ExistsAsync(roleName, featureId);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if role feature exists for role {RoleName} and feature {FeatureId}", roleName, featureId);
                return StatusCode(500, new { message = "Error checking role feature existence", error = ex.Message });
            }
        }
    }
}