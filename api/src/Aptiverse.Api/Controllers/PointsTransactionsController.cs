using Aptiverse.Api.Application.PointsTransactions.Dtos;
using Aptiverse.Api.Application.PointsTransactions.Services;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/points-transactions")]
    public class PointsTransactionsController(
        IPointsTransactionService pointsTransactionService,
        ILogger<PointsTransactionsController> logger) : ControllerBase
    {
        private readonly IPointsTransactionService _pointsTransactionService = pointsTransactionService;
        private readonly ILogger<PointsTransactionsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<PointsTransactionDto>> CreatePointsTransaction([FromBody] CreatePointsTransactionDto createPointsTransactionDto)
        {
            try
            {
                var createdPointsTransaction = await _pointsTransactionService.CreatePointsTransactionAsync(createPointsTransactionDto);
                return CreatedAtAction(nameof(GetPointsTransaction), new { id = createdPointsTransaction.Id }, createdPointsTransaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating points transaction");
                return BadRequest(new { message = "Error creating points transaction", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PointsTransactionDto>> GetPointsTransaction(long id)
        {
            try
            {
                var pointsTransaction = await _pointsTransactionService.GetPointsTransactionByIdAsync(id);

                if (pointsTransaction == null)
                    return NotFound(new { message = $"Points transaction with ID {id} not found" });

                return Ok(pointsTransaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving points transaction with ID {PointsTransactionId}", id);
                return StatusCode(500, new { message = "Error retrieving points transaction", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<PointsTransactionDto>>> GetPointsTransactions(
            [FromQuery] long? studentPointsId = null,
            [FromQuery] long? studentId = null,
            [FromQuery] string? transactionType = null,
            [FromQuery] string? source = null,
            [FromQuery] long? relatedGoalId = null,
            [FromQuery] long? relatedRewardId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? sortBy = "TransactionDate",
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

                var result = await _pointsTransactionService.GetPointsTransactionsAsync(
                    currentUser: User,
                    studentPointsId: studentPointsId,
                    studentId: studentId,
                    transactionType: transactionType,
                    source: source,
                    relatedGoalId: relatedGoalId,
                    relatedRewardId: relatedRewardId,
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
                _logger.LogError(ex, "Error retrieving points transactions");
                return StatusCode(500, new { message = "Error retrieving points transactions", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PointsTransactionDto>> UpdatePointsTransaction(long id, [FromBody] UpdatePointsTransactionDto updatePointsTransactionDto)
        {
            try
            {
                var updatedPointsTransaction = await _pointsTransactionService.UpdatePointsTransactionAsync(id, updatePointsTransactionDto);
                return Ok(updatedPointsTransaction);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Points transaction with ID {PointsTransactionId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating points transaction with ID {PointsTransactionId}", id);
                return BadRequest(new { message = "Error updating points transaction", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePointsTransaction(long id)
        {
            try
            {
                var result = await _pointsTransactionService.DeletePointsTransactionAsync(id);

                if (!result)
                    return NotFound(new { message = $"Points transaction with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting points transaction with ID {PointsTransactionId}", id);
                return StatusCode(500, new { message = "Error deleting points transaction", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountPointsTransactions(
            [FromQuery] long? studentPointsId = null,
            [FromQuery] long? studentId = null,
            [FromQuery] string? transactionType = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var count = await _pointsTransactionService.CountPointsTransactionsAsync(User, studentPointsId, studentId, transactionType, fromDate, toDate);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting points transactions");
                return StatusCode(500, new { message = "Error counting points transactions", error = ex.Message });
            }
        }

        [HttpGet("student-points/{studentPointsId}")]
        public async Task<ActionResult<IEnumerable<PointsTransactionDto>>> GetTransactionsByStudentPoints(long studentPointsId)
        {
            try
            {
                var transactions = await _pointsTransactionService.GetTransactionsByStudentPointsAsync(studentPointsId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transactions for student points {StudentPointsId}", studentPointsId);
                return StatusCode(500, new { message = "Error retrieving transactions", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<PointsTransactionDto>>> GetTransactionsByStudent(long studentId)
        {
            try
            {
                var transactions = await _pointsTransactionService.GetTransactionsByStudentAsync(studentId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transactions for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving transactions", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/recent")]
        public async Task<ActionResult<IEnumerable<PointsTransactionDto>>> GetRecentTransactionsByStudent(long studentId, [FromQuery] int count = 10)
        {
            try
            {
                if (count < 1 || count > 50) count = 10;

                var transactions = await _pointsTransactionService.GetRecentTransactionsByStudentAsync(studentId, count);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent transactions for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving recent transactions", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/earned")]
        public async Task<ActionResult<IEnumerable<PointsTransactionDto>>> GetEarnedTransactionsByStudent(
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

                var transactions = await _pointsTransactionService.GetEarnedTransactionsByStudentAsync(studentId, fromDate, toDate);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving earned transactions for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving earned transactions", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/spent")]
        public async Task<ActionResult<IEnumerable<PointsTransactionDto>>> GetSpentTransactionsByStudent(
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

                var transactions = await _pointsTransactionService.GetSpentTransactionsByStudentAsync(studentId, fromDate, toDate);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving spent transactions for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving spent transactions", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/summary")]
        public async Task<ActionResult<Dictionary<string, int>>> GetTransactionSummary(
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

                var summary = await _pointsTransactionService.GetTransactionSummaryAsync(studentId, fromDate, toDate);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction summary for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error retrieving transaction summary", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/total-earned")]
        public async Task<ActionResult<int>> GetTotalEarnedPoints(
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

                var totalEarned = await _pointsTransactionService.GetTotalEarnedPointsAsync(studentId, fromDate, toDate);
                return Ok(new { totalEarned });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total earned points for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error calculating total earned points", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/total-spent")]
        public async Task<ActionResult<int>> GetTotalSpentPoints(
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

                var totalSpent = await _pointsTransactionService.GetTotalSpentPointsAsync(studentId, fromDate, toDate);
                return Ok(new { totalSpent });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total spent points for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error calculating total spent points", error = ex.Message });
            }
        }

        [HttpGet("my-transactions")]
        public async Task<ActionResult<IEnumerable<PointsTransactionDto>>> GetMyTransactions()
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

                // Need to get the student ID from the user ID - implement this lookup
                // For now, return empty list
                return Ok(new List<PointsTransactionDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's transactions");
                return StatusCode(500, new { message = "Error retrieving transactions", error = ex.Message });
            }
        }

        [HttpGet("my-recent-transactions")]
        public async Task<ActionResult<IEnumerable<PointsTransactionDto>>> GetMyRecentTransactions([FromQuery] int count = 10)
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

                // Need to get the student ID from the user ID - implement this lookup
                // For now, return empty list
                return Ok(new List<PointsTransactionDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's recent transactions");
                return StatusCode(500, new { message = "Error retrieving recent transactions", error = ex.Message });
            }
        }

        [HttpGet("my-summary")]
        public async Task<ActionResult<Dictionary<string, int>>> GetMyTransactionSummary(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
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

                if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                {
                    return BadRequest(new { message = "fromDate cannot be greater than toDate" });
                }

                // Need to get the student ID from the user ID - implement this lookup
                // For now, return empty summary
                return Ok(new Dictionary<string, int>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current student's transaction summary");
                return StatusCode(500, new { message = "Error retrieving transaction summary", error = ex.Message });
            }
        }
    }
}