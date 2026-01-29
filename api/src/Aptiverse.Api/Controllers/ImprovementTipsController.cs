using Aptiverse.Api.Application.ImprovementTips.Dtos;
using Aptiverse.Api.Application.ImprovementTips.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/improvement-tips")]
    public class ImprovementTipsController(
        IImprovementTipService improvementTipService,
        ILogger<ImprovementTipsController> logger) : ControllerBase
    {
        private readonly IImprovementTipService _improvementTipService = improvementTipService;
        private readonly ILogger<ImprovementTipsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<ImprovementTipDto>> CreateImprovementTip([FromBody] CreateImprovementTipDto createImprovementTipDto)
        {
            try
            {
                var createdImprovementTip = await _improvementTipService.CreateImprovementTipAsync(createImprovementTipDto);
                return CreatedAtAction(nameof(GetImprovementTip), new { id = createdImprovementTip.Id }, createdImprovementTip);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating improvement tip");
                return BadRequest(new { message = "Error creating improvement tip", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ImprovementTipDto>> GetImprovementTip(long id)
        {
            try
            {
                var improvementTip = await _improvementTipService.GetImprovementTipByIdAsync(id);

                if (improvementTip == null)
                    return NotFound(new { message = $"ImprovementTip with ID {id} not found" });

                return Ok(improvementTip);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving improvement tip with ID {ImprovementTipId}", id);
                return StatusCode(500, new { message = "Error retrieving improvement tip", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ImprovementTipDto>>> GetImprovementTips(
            [FromQuery] long? studentSubjectId = null,
            [FromQuery] string? search = null,
            [FromQuery] int? minPriority = null,
            [FromQuery] int? maxPriority = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _improvementTipService.GetImprovementTipsAsync(
                    studentSubjectId: studentSubjectId,
                    search: search,
                    minPriority: minPriority,
                    maxPriority: maxPriority,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving improvement tips");
                return StatusCode(500, new { message = "Error retrieving improvement tips", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ImprovementTipDto>> UpdateImprovementTip(long id, [FromBody] UpdateImprovementTipDto updateImprovementTipDto)
        {
            try
            {
                var updatedImprovementTip = await _improvementTipService.UpdateImprovementTipAsync(id, updateImprovementTipDto);
                return Ok(updatedImprovementTip);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "ImprovementTip with ID {ImprovementTipId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating improvement tip with ID {ImprovementTipId}", id);
                return BadRequest(new { message = "Error updating improvement tip", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImprovementTip(long id)
        {
            try
            {
                var result = await _improvementTipService.DeleteImprovementTipAsync(id);

                if (!result)
                    return NotFound(new { message = $"ImprovementTip with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting improvement tip with ID {ImprovementTipId}", id);
                return StatusCode(500, new { message = "Error deleting improvement tip", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountImprovementTips([FromQuery] long? studentSubjectId = null)
        {
            try
            {
                var count = await _improvementTipService.CountImprovementTipsAsync(studentSubjectId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting improvement tips");
                return StatusCode(500, new { message = "Error counting improvement tips", error = ex.Message });
            }
        }
    }
}