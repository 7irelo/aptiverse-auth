using Aptiverse.Api.Application.StudySessions.Dtos;
using Aptiverse.Api.Application.StudySessions.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/study-sessions")]
    public class StudySessionsController(
        IStudySessionService studySessionService,
        ILogger<StudySessionsController> logger) : ControllerBase
    {
        private readonly IStudySessionService _studySessionService = studySessionService;
        private readonly ILogger<StudySessionsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<StudySessionDto>> CreateStudySession([FromBody] CreateStudySessionDto createStudySessionDto)
        {
            try
            {
                var createdStudySession = await _studySessionService.CreateStudySessionAsync(createStudySessionDto);
                return CreatedAtAction(nameof(GetStudySession), new { id = createdStudySession.Id }, createdStudySession);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating study session");
                return BadRequest(new { message = "Error creating study session", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudySessionDto>> GetStudySession(long id)
        {
            try
            {
                var studySession = await _studySessionService.GetStudySessionByIdAsync(id);

                if (studySession == null)
                    return NotFound(new { message = $"StudySession with ID {id} not found" });

                return Ok(studySession);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving study session with ID {StudySessionId}", id);
                return StatusCode(500, new { message = "Error retrieving study session", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<StudySessionDto>>> GetStudySessions(
            [FromQuery] long? studentId = null,
            [FromQuery] string? subjectId = null,
            [FromQuery] string? sessionType = null,
            [FromQuery] DateTime? startAfter = null,
            [FromQuery] DateTime? startBefore = null,
            [FromQuery] int? minDurationMinutes = null,
            [FromQuery] int? maxDurationMinutes = null,
            [FromQuery] double? minEfficiencyScore = null,
            [FromQuery] double? maxEfficiencyScore = null,
            [FromQuery] int? minConcentrationLevel = null,
            [FromQuery] int? maxConcentrationLevel = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _studySessionService.GetStudySessionsAsync(
                    studentId: studentId,
                    subjectId: subjectId,
                    sessionType: sessionType,
                    startAfter: startAfter,
                    startBefore: startBefore,
                    minDurationMinutes: minDurationMinutes,
                    maxDurationMinutes: maxDurationMinutes,
                    minEfficiencyScore: minEfficiencyScore,
                    maxEfficiencyScore: maxEfficiencyScore,
                    minConcentrationLevel: minConcentrationLevel,
                    maxConcentrationLevel: maxConcentrationLevel,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving study sessions");
                return StatusCode(500, new { message = "Error retrieving study sessions", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StudySessionDto>> UpdateStudySession(long id, [FromBody] UpdateStudySessionDto updateStudySessionDto)
        {
            try
            {
                var updatedStudySession = await _studySessionService.UpdateStudySessionAsync(id, updateStudySessionDto);
                return Ok(updatedStudySession);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "StudySession with ID {StudySessionId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating study session with ID {StudySessionId}", id);
                return BadRequest(new { message = "Error updating study session", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudySession(long id)
        {
            try
            {
                var result = await _studySessionService.DeleteStudySessionAsync(id);

                if (!result)
                    return NotFound(new { message = $"StudySession with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting study session with ID {StudySessionId}", id);
                return StatusCode(500, new { message = "Error deleting study session", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountStudySessions(
            [FromQuery] long? studentId = null,
            [FromQuery] string? subjectId = null,
            [FromQuery] string? sessionType = null)
        {
            try
            {
                var count = await _studySessionService.CountStudySessionsAsync(studentId, subjectId, sessionType);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting study sessions");
                return StatusCode(500, new { message = "Error counting study sessions", error = ex.Message });
            }
        }
    }
}