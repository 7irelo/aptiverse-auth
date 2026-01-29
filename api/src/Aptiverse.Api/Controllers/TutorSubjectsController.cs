using Aptiverse.Api.Application.TutorSubjects.Dtos;
using Aptiverse.Api.Application.TutorSubjects.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/tutor-subjects")]
    public class TutorSubjectsController(
        ITutorSubjectService tutorSubjectService,
        ILogger<TutorSubjectsController> logger) : ControllerBase
    {
        private readonly ITutorSubjectService _tutorSubjectService = tutorSubjectService;
        private readonly ILogger<TutorSubjectsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<TutorSubjectDto>> CreateTutorSubject([FromBody] CreateTutorSubjectDto createTutorSubjectDto)
        {
            try
            {
                var createdTutorSubject = await _tutorSubjectService.CreateTutorSubjectAsync(createTutorSubjectDto);
                return CreatedAtAction(nameof(GetTutorSubject), new { id = createdTutorSubject.Id }, createdTutorSubject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tutor subject");
                return BadRequest(new { message = "Error creating tutor subject", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TutorSubjectDto>> GetTutorSubject(long id)
        {
            try
            {
                var tutorSubject = await _tutorSubjectService.GetTutorSubjectByIdAsync(id);

                if (tutorSubject == null)
                    return NotFound(new { message = $"TutorSubject with ID {id} not found" });

                return Ok(tutorSubject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tutor subject with ID {TutorSubjectId}", id);
                return StatusCode(500, new { message = "Error retrieving tutor subject", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<TutorSubjectDto>>> GetTutorSubjects(
            [FromQuery] long? tutorId = null,
            [FromQuery] string? subjectId = null,
            [FromQuery] int? minProficiencyLevel = null,
            [FromQuery] int? maxProficiencyLevel = null,
            [FromQuery] decimal? minHourlyRate = null,
            [FromQuery] decimal? maxHourlyRate = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _tutorSubjectService.GetTutorSubjectsAsync(
                    tutorId: tutorId,
                    subjectId: subjectId,
                    minProficiencyLevel: minProficiencyLevel,
                    maxProficiencyLevel: maxProficiencyLevel,
                    minHourlyRate: minHourlyRate,
                    maxHourlyRate: maxHourlyRate,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tutor subjects");
                return StatusCode(500, new { message = "Error retrieving tutor subjects", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TutorSubjectDto>> UpdateTutorSubject(long id, [FromBody] UpdateTutorSubjectDto updateTutorSubjectDto)
        {
            try
            {
                var updatedTutorSubject = await _tutorSubjectService.UpdateTutorSubjectAsync(id, updateTutorSubjectDto);
                return Ok(updatedTutorSubject);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "TutorSubject with ID {TutorSubjectId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tutor subject with ID {TutorSubjectId}", id);
                return BadRequest(new { message = "Error updating tutor subject", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTutorSubject(long id)
        {
            try
            {
                var result = await _tutorSubjectService.DeleteTutorSubjectAsync(id);

                if (!result)
                    return NotFound(new { message = $"TutorSubject with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tutor subject with ID {TutorSubjectId}", id);
                return StatusCode(500, new { message = "Error deleting tutor subject", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountTutorSubjects(
            [FromQuery] long? tutorId = null,
            [FromQuery] string? subjectId = null)
        {
            try
            {
                var count = await _tutorSubjectService.CountTutorSubjectsAsync(tutorId, subjectId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting tutor subjects");
                return StatusCode(500, new { message = "Error counting tutor subjects", error = ex.Message });
            }
        }
    }
}