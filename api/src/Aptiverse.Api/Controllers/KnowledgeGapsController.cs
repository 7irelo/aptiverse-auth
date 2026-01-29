using Aptiverse.Api.Application.KnowledgeGaps.Dtos;
using Aptiverse.Api.Application.KnowledgeGaps.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/knowledge-gaps")]
    public class KnowledgeGapsController(
        IKnowledgeGapService knowledgeGapService,
        ILogger<KnowledgeGapsController> logger) : ControllerBase
    {
        private readonly IKnowledgeGapService _knowledgeGapService = knowledgeGapService;
        private readonly ILogger<KnowledgeGapsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<KnowledgeGapDto>> CreateKnowledgeGap([FromBody] CreateKnowledgeGapDto createKnowledgeGapDto)
        {
            try
            {
                var createdKnowledgeGap = await _knowledgeGapService.CreateKnowledgeGapAsync(createKnowledgeGapDto);
                return CreatedAtAction(nameof(GetKnowledgeGap), new { id = createdKnowledgeGap.Id }, createdKnowledgeGap);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating knowledge gap");
                return BadRequest(new { message = "Error creating knowledge gap", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KnowledgeGapDto>> GetKnowledgeGap(long id)
        {
            try
            {
                var knowledgeGap = await _knowledgeGapService.GetKnowledgeGapByIdAsync(id);

                if (knowledgeGap == null)
                    return NotFound(new { message = $"KnowledgeGap with ID {id} not found" });

                return Ok(knowledgeGap);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving knowledge gap with ID {KnowledgeGapId}", id);
                return StatusCode(500, new { message = "Error retrieving knowledge gap", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<KnowledgeGapDto>>> GetKnowledgeGaps(
            [FromQuery] long? studentSubjectId = null,
            [FromQuery] string? severity = null,
            [FromQuery] string? search = null,
            [FromQuery] DateTime? lastTestedAfter = null,
            [FromQuery] DateTime? lastTestedBefore = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _knowledgeGapService.GetKnowledgeGapsAsync(
                    studentSubjectId: studentSubjectId,
                    severity: severity,
                    search: search,
                    lastTestedAfter: lastTestedAfter,
                    lastTestedBefore: lastTestedBefore,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving knowledge gaps");
                return StatusCode(500, new { message = "Error retrieving knowledge gaps", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<KnowledgeGapDto>> UpdateKnowledgeGap(long id, [FromBody] UpdateKnowledgeGapDto updateKnowledgeGapDto)
        {
            try
            {
                var updatedKnowledgeGap = await _knowledgeGapService.UpdateKnowledgeGapAsync(id, updateKnowledgeGapDto);
                return Ok(updatedKnowledgeGap);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "KnowledgeGap with ID {KnowledgeGapId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating knowledge gap with ID {KnowledgeGapId}", id);
                return BadRequest(new { message = "Error updating knowledge gap", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKnowledgeGap(long id)
        {
            try
            {
                var result = await _knowledgeGapService.DeleteKnowledgeGapAsync(id);

                if (!result)
                    return NotFound(new { message = $"KnowledgeGap with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting knowledge gap with ID {KnowledgeGapId}", id);
                return StatusCode(500, new { message = "Error deleting knowledge gap", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountKnowledgeGaps(
            [FromQuery] long? studentSubjectId = null,
            [FromQuery] string? severity = null)
        {
            try
            {
                var count = await _knowledgeGapService.CountKnowledgeGapsAsync(studentSubjectId, severity);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting knowledge gaps");
                return StatusCode(500, new { message = "Error counting knowledge gaps", error = ex.Message });
            }
        }
    }
}