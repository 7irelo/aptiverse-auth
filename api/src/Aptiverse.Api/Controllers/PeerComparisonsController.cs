using Aptiverse.Api.Application.PeerComparisons.Dtos;
using Aptiverse.Api.Application.PeerComparisons.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/peer-comparisons")]
    public class PeerComparisonsController(
        IPeerComparisonService peerComparisonService,
        ILogger<PeerComparisonsController> logger) : ControllerBase
    {
        private readonly IPeerComparisonService _peerComparisonService = peerComparisonService;
        private readonly ILogger<PeerComparisonsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<PeerComparisonDto>> CreatePeerComparison([FromBody] CreatePeerComparisonDto createPeerComparisonDto)
        {
            try
            {
                var createdPeerComparison = await _peerComparisonService.CreatePeerComparisonAsync(createPeerComparisonDto);
                return CreatedAtAction(nameof(GetPeerComparison), new { id = createdPeerComparison.Id }, createdPeerComparison);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating peer comparison");
                return BadRequest(new { message = "Error creating peer comparison", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PeerComparisonDto>> GetPeerComparison(long id)
        {
            try
            {
                var peerComparison = await _peerComparisonService.GetPeerComparisonByIdAsync(id);

                if (peerComparison == null)
                    return NotFound(new { message = $"PeerComparison with ID {id} not found" });

                return Ok(peerComparison);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving peer comparison with ID {PeerComparisonId}", id);
                return StatusCode(500, new { message = "Error retrieving peer comparison", error = ex.Message });
            }
        }

        [HttpGet("student-subject/{studentSubjectId}")]
        public async Task<ActionResult<PeerComparisonDto>> GetPeerComparisonByStudentSubject(long studentSubjectId)
        {
            try
            {
                var peerComparison = await _peerComparisonService.GetPeerComparisonByStudentSubjectIdAsync(studentSubjectId);

                if (peerComparison == null)
                    return NotFound(new { message = $"PeerComparison for student subject ID {studentSubjectId} not found" });

                return Ok(peerComparison);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving peer comparison for student subject ID {StudentSubjectId}", studentSubjectId);
                return StatusCode(500, new { message = "Error retrieving peer comparison", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<PeerComparisonDto>>> GetPeerComparisons(
            [FromQuery] long? studentSubjectId = null,
            [FromQuery] string? trendComparison = null,
            [FromQuery] int? minPercentile = null,
            [FromQuery] int? maxPercentile = null,
            [FromQuery] int? minRanking = null,
            [FromQuery] int? maxRanking = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _peerComparisonService.GetPeerComparisonsAsync(
                    studentSubjectId: studentSubjectId,
                    trendComparison: trendComparison,
                    minPercentile: minPercentile,
                    maxPercentile: maxPercentile,
                    minRanking: minRanking,
                    maxRanking: maxRanking,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving peer comparisons");
                return StatusCode(500, new { message = "Error retrieving peer comparisons", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PeerComparisonDto>> UpdatePeerComparison(long id, [FromBody] UpdatePeerComparisonDto updatePeerComparisonDto)
        {
            try
            {
                var updatedPeerComparison = await _peerComparisonService.UpdatePeerComparisonAsync(id, updatePeerComparisonDto);
                return Ok(updatedPeerComparison);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "PeerComparison with ID {PeerComparisonId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating peer comparison with ID {PeerComparisonId}", id);
                return BadRequest(new { message = "Error updating peer comparison", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePeerComparison(long id)
        {
            try
            {
                var result = await _peerComparisonService.DeletePeerComparisonAsync(id);

                if (!result)
                    return NotFound(new { message = $"PeerComparison with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting peer comparison with ID {PeerComparisonId}", id);
                return StatusCode(500, new { message = "Error deleting peer comparison", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountPeerComparisons(
            [FromQuery] long? studentSubjectId = null,
            [FromQuery] string? trendComparison = null)
        {
            try
            {
                var count = await _peerComparisonService.CountPeerComparisonsAsync(studentSubjectId, trendComparison);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting peer comparisons");
                return StatusCode(500, new { message = "Error counting peer comparisons", error = ex.Message });
            }
        }
    }
}