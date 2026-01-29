using Aptiverse.Api.Application.PredictionMetricss.Dtos;
using Aptiverse.Api.Application.PredictionMetricss.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/prediction-metrics")]
    public class PredictionMetricsController(
        IPredictionMetricsService predictionMetricsService,
        ILogger<PredictionMetricsController> logger) : ControllerBase
    {
        private readonly IPredictionMetricsService _predictionMetricsService = predictionMetricsService;
        private readonly ILogger<PredictionMetricsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<PredictionMetricsDto>> CreatePredictionMetrics([FromBody] CreatePredictionMetricsDto createPredictionMetricsDto)
        {
            try
            {
                var createdPredictionMetrics = await _predictionMetricsService.CreatePredictionMetricsAsync(createPredictionMetricsDto);
                return CreatedAtAction(nameof(GetPredictionMetrics), new { id = createdPredictionMetrics.Id }, createdPredictionMetrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating prediction metrics");
                return BadRequest(new { message = "Error creating prediction metrics", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PredictionMetricsDto>> GetPredictionMetrics(long id)
        {
            try
            {
                var predictionMetrics = await _predictionMetricsService.GetPredictionMetricsByIdAsync(id);

                if (predictionMetrics == null)
                    return NotFound(new { message = $"PredictionMetrics with ID {id} not found" });

                return Ok(predictionMetrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prediction metrics with ID {PredictionMetricsId}", id);
                return StatusCode(500, new { message = "Error retrieving prediction metrics", error = ex.Message });
            }
        }

        [HttpGet("student-subject/{studentSubjectId}")]
        public async Task<ActionResult<PredictionMetricsDto>> GetPredictionMetricsByStudentSubject(long studentSubjectId)
        {
            try
            {
                var predictionMetrics = await _predictionMetricsService.GetPredictionMetricsByStudentSubjectIdAsync(studentSubjectId);

                if (predictionMetrics == null)
                    return NotFound(new { message = $"PredictionMetrics for student subject ID {studentSubjectId} not found" });

                return Ok(predictionMetrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prediction metrics for student subject ID {StudentSubjectId}", studentSubjectId);
                return StatusCode(500, new { message = "Error retrieving prediction metrics", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<PredictionMetricsDto>>> GetPredictionMetrics(
            [FromQuery] long? studentSubjectId = null,
            [FromQuery] string? riskLevel = null,
            [FromQuery] bool? interventionNeeded = null,
            [FromQuery] int? minProbabilityA = null,
            [FromQuery] int? minProbabilityB = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _predictionMetricsService.GetPredictionMetricsAsync(
                    studentSubjectId: studentSubjectId,
                    riskLevel: riskLevel,
                    interventionNeeded: interventionNeeded,
                    minProbabilityA: minProbabilityA,
                    minProbabilityB: minProbabilityB,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prediction metrics");
                return StatusCode(500, new { message = "Error retrieving prediction metrics", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PredictionMetricsDto>> UpdatePredictionMetrics(long id, [FromBody] UpdatePredictionMetricsDto updatePredictionMetricsDto)
        {
            try
            {
                var updatedPredictionMetrics = await _predictionMetricsService.UpdatePredictionMetricsAsync(id, updatePredictionMetricsDto);
                return Ok(updatedPredictionMetrics);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "PredictionMetrics with ID {PredictionMetricsId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating prediction metrics with ID {PredictionMetricsId}", id);
                return BadRequest(new { message = "Error updating prediction metrics", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePredictionMetrics(long id)
        {
            try
            {
                var result = await _predictionMetricsService.DeletePredictionMetricsAsync(id);

                if (!result)
                    return NotFound(new { message = $"PredictionMetrics with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting prediction metrics with ID {PredictionMetricsId}", id);
                return StatusCode(500, new { message = "Error deleting prediction metrics", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountPredictionMetrics(
            [FromQuery] long? studentSubjectId = null,
            [FromQuery] string? riskLevel = null,
            [FromQuery] bool? interventionNeeded = null)
        {
            try
            {
                var count = await _predictionMetricsService.CountPredictionMetricsAsync(studentSubjectId, riskLevel, interventionNeeded);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting prediction metrics");
                return StatusCode(500, new { message = "Error counting prediction metrics", error = ex.Message });
            }
        }
    }
}