using Aptiverse.Api.Application.AssessmentBreakdowns.Dtos;
using Aptiverse.Api.Application.AssessmentBreakdowns.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/assessment-breakdowns")]
    public class AssessmentBreakdownsController(
        IAssessmentBreakdownService breakdownService,
        ILogger<AssessmentBreakdownsController> logger) : ControllerBase
    {
        private readonly IAssessmentBreakdownService _breakdownService = breakdownService;
        private readonly ILogger<AssessmentBreakdownsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<AssessmentBreakdownDto>> CreateAssessmentBreakdown([FromBody] CreateAssessmentBreakdownDto createDto)
        {
            try
            {
                var createdBreakdown = await _breakdownService.CreateAssessmentBreakdownAsync(createDto);
                return CreatedAtAction(nameof(GetAssessmentBreakdown), new { id = createdBreakdown.Id }, createdBreakdown);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating assessment breakdown");
                return BadRequest(new { message = "Error creating assessment breakdown", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssessmentBreakdownDto>> GetAssessmentBreakdown(long id)
        {
            try
            {
                var breakdown = await _breakdownService.GetAssessmentBreakdownByIdAsync(id);

                if (breakdown == null)
                    return NotFound(new { message = $"Assessment breakdown with ID {id} not found" });

                return Ok(breakdown);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assessment breakdown with ID {BreakdownId}", id);
                return StatusCode(500, new { message = "Error retrieving assessment breakdown", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<AssessmentBreakdownDto>>> GetAssessmentBreakdowns(
            [FromQuery] long? studentSubjectId = null,
            [FromQuery] string? assessmentType = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _breakdownService.GetAssessmentBreakdownsAsync(
                    currentUser: User,
                    studentSubjectId: studentSubjectId,
                    assessmentType: assessmentType,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assessment breakdowns");
                return StatusCode(500, new { message = "Error retrieving assessment breakdowns", error = ex.Message });
            }
        }

        [HttpGet("student-subject/{studentSubjectId}")]
        public async Task<ActionResult<IEnumerable<AssessmentBreakdownDto>>> GetBreakdownsByStudentSubject(long studentSubjectId)
        {
            try
            {
                var breakdowns = await _breakdownService.GetBreakdownsByStudentSubjectAsync(studentSubjectId);
                return Ok(breakdowns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving breakdowns for student subject {StudentSubjectId}", studentSubjectId);
                return StatusCode(500, new { message = "Error retrieving breakdowns", error = ex.Message });
            }
        }

        [HttpPost("student-subject/{studentSubjectId}/recalculate")]
        public async Task<IActionResult> RecalculateBreakdown(long studentSubjectId)
        {
            try
            {
                await _breakdownService.RecalculateBreakdownAsync(studentSubjectId);
                return Ok(new { message = "Breakdown recalculated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "StudentSubject with ID {StudentSubjectId} not found", studentSubjectId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recalculating breakdown for student subject {StudentSubjectId}", studentSubjectId);
                return StatusCode(500, new { message = "Error recalculating breakdown", error = ex.Message });
            }
        }

        [HttpPost("recalculate-all")]
        public async Task<IActionResult> RecalculateAllBreakdowns()
        {
            try
            {
                await _breakdownService.RecalculateAllBreakdownsAsync();
                return Ok(new { message = "All breakdowns recalculated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recalculating all breakdowns");
                return StatusCode(500, new { message = "Error recalculating all breakdowns", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AssessmentBreakdownDto>> UpdateAssessmentBreakdown(long id, [FromBody] UpdateAssessmentBreakdownDto updateDto)
        {
            try
            {
                var updatedBreakdown = await _breakdownService.UpdateAssessmentBreakdownAsync(id, updateDto);
                return Ok(updatedBreakdown);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Assessment breakdown with ID {BreakdownId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating assessment breakdown with ID {BreakdownId}", id);
                return BadRequest(new { message = "Error updating assessment breakdown", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssessmentBreakdown(long id)
        {
            try
            {
                var result = await _breakdownService.DeleteAssessmentBreakdownAsync(id);

                if (!result)
                    return NotFound(new { message = $"Assessment breakdown with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting assessment breakdown with ID {BreakdownId}", id);
                return StatusCode(500, new { message = "Error deleting assessment breakdown", error = ex.Message });
            }
        }
    }
}