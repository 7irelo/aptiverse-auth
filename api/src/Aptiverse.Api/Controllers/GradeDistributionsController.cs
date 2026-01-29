using Aptiverse.Api.Application.GradeDistributions.Dtos;
using Aptiverse.Api.Application.GradeDistributions.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/grade-distributions")]
    public class GradeDistributionsController(
        IGradeDistributionService gradeDistributionService,
        ILogger<GradeDistributionsController> logger) : ControllerBase
    {
        private readonly IGradeDistributionService _gradeDistributionService = gradeDistributionService;
        private readonly ILogger<GradeDistributionsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<GradeDistributionDto>> CreateGradeDistribution([FromBody] CreateGradeDistributionDto createGradeDistributionDto)
        {
            try
            {
                var createdGradeDistribution = await _gradeDistributionService.CreateGradeDistributionAsync(createGradeDistributionDto);
                return CreatedAtAction(nameof(GetGradeDistribution), new { id = createdGradeDistribution.Id }, createdGradeDistribution);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating grade distribution");
                return BadRequest(new { message = "Error creating grade distribution", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GradeDistributionDto>> GetGradeDistribution(long id)
        {
            try
            {
                var gradeDistribution = await _gradeDistributionService.GetGradeDistributionByIdAsync(id);

                if (gradeDistribution == null)
                    return NotFound(new { message = $"GradeDistribution with ID {id} not found" });

                return Ok(gradeDistribution);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving grade distribution with ID {GradeDistributionId}", id);
                return StatusCode(500, new { message = "Error retrieving grade distribution", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GradeDistributionDto>>> GetGradeDistributions(
            [FromQuery] long? studentSubjectId = null,
            [FromQuery] string? grade = null,
            [FromQuery] int? minCount = null,
            [FromQuery] int? maxCount = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _gradeDistributionService.GetGradeDistributionsAsync(
                    studentSubjectId: studentSubjectId,
                    grade: grade,
                    minCount: minCount,
                    maxCount: maxCount,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving grade distributions");
                return StatusCode(500, new { message = "Error retrieving grade distributions", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GradeDistributionDto>> UpdateGradeDistribution(long id, [FromBody] UpdateGradeDistributionDto updateGradeDistributionDto)
        {
            try
            {
                var updatedGradeDistribution = await _gradeDistributionService.UpdateGradeDistributionAsync(id, updateGradeDistributionDto);
                return Ok(updatedGradeDistribution);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "GradeDistribution with ID {GradeDistributionId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating grade distribution with ID {GradeDistributionId}", id);
                return BadRequest(new { message = "Error updating grade distribution", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGradeDistribution(long id)
        {
            try
            {
                var result = await _gradeDistributionService.DeleteGradeDistributionAsync(id);

                if (!result)
                    return NotFound(new { message = $"GradeDistribution with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting grade distribution with ID {GradeDistributionId}", id);
                return StatusCode(500, new { message = "Error deleting grade distribution", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountGradeDistributions(
            [FromQuery] long? studentSubjectId = null,
            [FromQuery] string? grade = null)
        {
            try
            {
                var count = await _gradeDistributionService.CountGradeDistributionsAsync(studentSubjectId, grade);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting grade distributions");
                return StatusCode(500, new { message = "Error counting grade distributions", error = ex.Message });
            }
        }
    }
}