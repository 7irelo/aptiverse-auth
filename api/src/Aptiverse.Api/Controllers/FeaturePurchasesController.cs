using Aptiverse.Api.Application.FeaturePurchases.Dtos;
using Aptiverse.Api.Application.FeaturePurchases.Services;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/feature-purchases")]
    public class FeaturePurchasesController(
        IFeaturePurchaseService purchaseService,
        ILogger<FeaturePurchasesController> logger) : ControllerBase
    {
        private readonly IFeaturePurchaseService _purchaseService = purchaseService;
        private readonly ILogger<FeaturePurchasesController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<FeaturePurchaseDto>> CreatePurchase([FromBody] CreateFeaturePurchaseDto createPurchaseDto)
        {
            try
            {
                var createdPurchase = await _purchaseService.CreatePurchaseAsync(createPurchaseDto);
                return CreatedAtAction(nameof(GetPurchase), new { id = createdPurchase.Id }, createdPurchase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating feature purchase");
                return BadRequest(new { message = "Error creating feature purchase", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FeaturePurchaseDto>> GetPurchase(long id)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseByIdAsync(id);

                if (purchase == null)
                    return NotFound(new { message = $"Feature purchase with ID {id} not found" });

                return Ok(purchase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feature purchase with ID {PurchaseId}", id);
                return StatusCode(500, new { message = "Error retrieving feature purchase", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<FeaturePurchaseDto>>> GetPurchases(
            [FromQuery] string? userId = null,
            [FromQuery] long? featureId = null,
            [FromQuery] string? paymentStatus = null,
            [FromQuery] string? billingCycle = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? sortBy = "PurchaseDate",
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

                var result = await _purchaseService.GetPurchasesAsync(
                    currentUser: User,
                    userId: userId,
                    featureId: featureId,
                    paymentStatus: paymentStatus,
                    billingCycle: billingCycle,
                    fromDate: fromDate,
                    toDate: toDate,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feature purchases");
                return StatusCode(500, new { message = "Error retrieving feature purchases", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FeaturePurchaseDto>> UpdatePurchase(long id, [FromBody] UpdateFeaturePurchaseDto updatePurchaseDto)
        {
            try
            {
                var updatedPurchase = await _purchaseService.UpdatePurchaseAsync(id, updatePurchaseDto);
                return Ok(updatedPurchase);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Feature purchase with ID {PurchaseId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating feature purchase with ID {PurchaseId}", id);
                return BadRequest(new { message = "Error updating feature purchase", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchase(long id)
        {
            try
            {
                var result = await _purchaseService.DeletePurchaseAsync(id);

                if (!result)
                    return NotFound(new { message = $"Feature purchase with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting feature purchase with ID {PurchaseId}", id);
                return StatusCode(500, new { message = "Error deleting feature purchase", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountPurchases(
            [FromQuery] string? userId = null,
            [FromQuery] long? featureId = null,
            [FromQuery] string? paymentStatus = null)
        {
            try
            {
                var count = await _purchaseService.CountPurchasesAsync(User, userId, featureId, paymentStatus);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting feature purchases");
                return StatusCode(500, new { message = "Error counting feature purchases", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<FeaturePurchaseDto>>> GetPurchasesByUser(string userId)
        {
            try
            {
                var purchases = await _purchaseService.GetPurchasesByUserAsync(userId);
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feature purchases for user {UserId}", userId);
                return StatusCode(500, new { message = "Error retrieving feature purchases", error = ex.Message });
            }
        }

        [HttpGet("feature/{featureId}")]
        public async Task<ActionResult<IEnumerable<FeaturePurchaseDto>>> GetPurchasesByFeature(long featureId)
        {
            try
            {
                var purchases = await _purchaseService.GetPurchasesByFeatureAsync(featureId);
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feature purchases for feature {FeatureId}", featureId);
                return StatusCode(500, new { message = "Error retrieving feature purchases", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}/active")]
        public async Task<ActionResult<IEnumerable<FeaturePurchaseDto>>> GetActivePurchases(string userId)
        {
            try
            {
                var purchases = await _purchaseService.GetActivePurchasesAsync(userId);
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active feature purchases for user {UserId}", userId);
                return StatusCode(500, new { message = "Error retrieving active feature purchases", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}/feature/{featureId}/active")]
        public async Task<ActionResult<bool>> HasActivePurchase(string userId, long featureId)
        {
            try
            {
                var hasActivePurchase = await _purchaseService.HasActivePurchaseAsync(userId, featureId);
                return Ok(new { hasActivePurchase });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking active purchase for user {UserId} and feature {FeatureId}", userId, featureId);
                return StatusCode(500, new { message = "Error checking active purchase", error = ex.Message });
            }
        }

        [HttpGet("revenue")]
        public async Task<ActionResult<decimal>> GetTotalRevenue(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var currentUser = User;
                if (!UserContextHelper.IsSuperUser(currentUser) && !UserContextHelper.IsAdmin(currentUser))
                {
                    return Forbid();
                }

                if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                {
                    return BadRequest(new { message = "fromDate cannot be greater than toDate" });
                }

                var totalRevenue = await _purchaseService.GetTotalRevenueAsync(fromDate, toDate);
                return Ok(new { totalRevenue });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total revenue");
                return StatusCode(500, new { message = "Error calculating total revenue", error = ex.Message });
            }
        }

        [HttpGet("my-purchases")]
        public async Task<ActionResult<IEnumerable<FeaturePurchaseDto>>> GetMyPurchases()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var purchases = await _purchaseService.GetPurchasesByUserAsync(userId);
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user's feature purchases");
                return StatusCode(500, new { message = "Error retrieving feature purchases", error = ex.Message });
            }
        }

        [HttpGet("my-active-purchases")]
        public async Task<ActionResult<IEnumerable<FeaturePurchaseDto>>> GetMyActivePurchases()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var purchases = await _purchaseService.GetActivePurchasesAsync(userId);
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user's active feature purchases");
                return StatusCode(500, new { message = "Error retrieving active feature purchases", error = ex.Message });
            }
        }
    }
}