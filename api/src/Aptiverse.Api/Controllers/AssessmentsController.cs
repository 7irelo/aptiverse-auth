using Aptiverse.Api.Application.Assessments.Dtos;
using Aptiverse.Api.Application.Assessments.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/assessments")]
    public class AssessmentsController(
        IAssessmentService assessmentService,
        ILogger<AssessmentsController> logger) : ControllerBase
    {
        private readonly IAssessmentService _assessmentService = assessmentService;
        private readonly ILogger<AssessmentsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<AssessmentDto>> CreateAssessment([FromBody] CreateAssessmentDto createDto)
        {
            try
            {
                var createdAssessment = await _assessmentService.CreateAssessmentAsync(createDto);
                return CreatedAtAction(nameof(GetAssessment), new { id = createdAssessment.Id }, createdAssessment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating assessment");
                return BadRequest(new { message = "Error creating assessment", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssessmentDto>> GetAssessment(long id)
        {
            try
            {
                var assessment = await _assessmentService.GetAssessmentByIdAsync(id);

                if (assessment == null)
                    return NotFound(new { message = $"Assessment with ID {id} not found" });

                return Ok(assessment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assessment with ID {AssessmentId}", id);
                return StatusCode(500, new { message = "Error retrieving assessment", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<AssessmentDto>>> GetAssessments(
            [FromQuery] long? studentId = null,
            [FromQuery] string? subjectId = null,
            [FromQuery] string? type = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? sortBy = "DateTaken",
            [FromQuery] bool sortDescending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _assessmentService.GetAssessmentsAsync(
                    currentUser: User,
                    studentId: studentId,
                    subjectId: subjectId,
                    type: type,
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
                _logger.LogError(ex, "Error retrieving assessments");
                return StatusCode(500, new { message = "Error retrieving assessments", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/average")]
        public async Task<ActionResult<double>> GetStudentAverage(long studentId, [FromQuery] string? subjectId = null)
        {
            try
            {
                var average = await _assessmentService.GetStudentAverageScoreAsync(studentId, subjectId);
                return Ok(new { average });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error calculating average", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/summary")]
        public async Task<ActionResult<IEnumerable<AssessmentSummaryDto>>> GetStudentSummary(long studentId)
        {
            try
            {
                var summary = await _assessmentService.GetAssessmentSummaryAsync(studentId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting assessment summary for student {StudentId}", studentId);
                return StatusCode(500, new { message = "Error getting assessment summary", error = ex.Message });
            }
        }

        [HttpGet("student/{studentId}/trend")]
        public async Task<ActionResult<IEnumerable<AssessmentTrendDto>>> GetAssessmentTrend(
            long studentId,
            [FromQuery] string subjectId,
            [FromQuery] string type)
        {
            try
            {
                var trend = await _assessmentService.GetAssessmentTrendAsync(studentId, subjectId, type);
                return Ok(trend);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting assessment trend for student {StudentId}, subject {SubjectId}, type {Type}",
                    studentId, subjectId, type);
                return StatusCode(500, new { message = "Error getting assessment trend", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AssessmentDto>> UpdateAssessment(long id, [FromBody] UpdateAssessmentDto updateDto)
        {
            try
            {
                var updatedAssessment = await _assessmentService.UpdateAssessmentAsync(id, updateDto);
                return Ok(updatedAssessment);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Assessment with ID {AssessmentId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating assessment with ID {AssessmentId}", id);
                return BadRequest(new { message = "Error updating assessment", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssessment(long id)
        {
            try
            {
                var result = await _assessmentService.DeleteAssessmentAsync(id);

                if (!result)
                    return NotFound(new { message = $"Assessment with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting assessment with ID {AssessmentId}", id);
                return StatusCode(500, new { message = "Error deleting assessment", error = ex.Message });
            }
        }
    }
}