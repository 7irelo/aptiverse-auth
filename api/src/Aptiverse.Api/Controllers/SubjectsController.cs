using Aptiverse.Api.Application.Subjects.Dtos;
using Aptiverse.Api.Application.Subjects.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/subjects")]
    public class SubjectsController(
        ISubjectService subjectService,
        ILogger<SubjectsController> logger) : ControllerBase
    {
        private readonly ISubjectService _subjectService = subjectService;
        private readonly ILogger<SubjectsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<SubjectDto>> CreateSubject([FromBody] CreateSubjectDto createSubjectDto)
        {
            try
            {
                var createdSubject = await _subjectService.CreateSubjectAsync(createSubjectDto);
                return CreatedAtAction(nameof(GetSubject), new { id = createdSubject.Id }, createdSubject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subject");
                return BadRequest(new { message = "Error creating subject", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectDto>> GetSubject(string id)
        {
            try
            {
                var subject = await _subjectService.GetSubjectByIdAsync(id);

                if (subject == null)
                    return NotFound(new { message = $"Subject with ID {id} not found" });

                return Ok(subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subject with ID {SubjectId}", id);
                return StatusCode(500, new { message = "Error retrieving subject", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<SubjectDto>>> GetSubjects(
            [FromQuery] string? search = null,
            [FromQuery] string? code = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _subjectService.GetSubjectsAsync(
                    search: search,
                    code: code,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subjects");
                return StatusCode(500, new { message = "Error retrieving subjects", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SubjectDto>> UpdateSubject(string id, [FromBody] UpdateSubjectDto updateSubjectDto)
        {
            try
            {
                var updatedSubject = await _subjectService.UpdateSubjectAsync(id, updateSubjectDto);
                return Ok(updatedSubject);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Subject with ID {SubjectId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subject with ID {SubjectId}", id);
                return BadRequest(new { message = "Error updating subject", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(string id)
        {
            try
            {
                var result = await _subjectService.DeleteSubjectAsync(id);

                if (!result)
                    return NotFound(new { message = $"Subject with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subject with ID {SubjectId}", id);
                return StatusCode(500, new { message = "Error deleting subject", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountSubjects([FromQuery] string? search = null)
        {
            try
            {
                var count = await _subjectService.CountSubjectsAsync(search);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting subjects");
                return StatusCode(500, new { message = "Error counting subjects", error = ex.Message });
            }
        }
    }
}