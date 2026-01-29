using Aptiverse.Api.Application.TutorAvailabilities.Dtos;
using Aptiverse.Api.Application.TutorAvailabilities.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/tutor-availabilities")]
    public class TutorAvailabilitiesController(
        ITutorAvailabilityService tutorAvailabilityService,
        ILogger<TutorAvailabilitiesController> logger) : ControllerBase
    {
        private readonly ITutorAvailabilityService _tutorAvailabilityService = tutorAvailabilityService;
        private readonly ILogger<TutorAvailabilitiesController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<TutorAvailabilityDto>> CreateTutorAvailability([FromBody] CreateTutorAvailabilityDto createTutorAvailabilityDto)
        {
            try
            {
                var createdTutorAvailability = await _tutorAvailabilityService.CreateTutorAvailabilityAsync(createTutorAvailabilityDto);
                return CreatedAtAction(nameof(GetTutorAvailability), new { id = createdTutorAvailability.Id }, createdTutorAvailability);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tutor availability");
                return BadRequest(new { message = "Error creating tutor availability", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TutorAvailabilityDto>> GetTutorAvailability(long id)
        {
            try
            {
                var tutorAvailability = await _tutorAvailabilityService.GetTutorAvailabilityByIdAsync(id);

                if (tutorAvailability == null)
                    return NotFound(new { message = $"TutorAvailability with ID {id} not found" });

                return Ok(tutorAvailability);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tutor availability with ID {TutorAvailabilityId}", id);
                return StatusCode(500, new { message = "Error retrieving tutor availability", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<TutorAvailabilityDto>>> GetTutorAvailabilities(
            [FromQuery] long? tutorId = null,
            [FromQuery] DayOfWeek? dayOfWeek = null,
            [FromQuery] bool? isAvailable = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _tutorAvailabilityService.GetTutorAvailabilitiesAsync(
                    tutorId: tutorId,
                    dayOfWeek: dayOfWeek,
                    isAvailable: isAvailable,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tutor availabilities");
                return StatusCode(500, new { message = "Error retrieving tutor availabilities", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TutorAvailabilityDto>> UpdateTutorAvailability(long id, [FromBody] UpdateTutorAvailabilityDto updateTutorAvailabilityDto)
        {
            try
            {
                var updatedTutorAvailability = await _tutorAvailabilityService.UpdateTutorAvailabilityAsync(id, updateTutorAvailabilityDto);
                return Ok(updatedTutorAvailability);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "TutorAvailability with ID {TutorAvailabilityId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tutor availability with ID {TutorAvailabilityId}", id);
                return BadRequest(new { message = "Error updating tutor availability", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTutorAvailability(long id)
        {
            try
            {
                var result = await _tutorAvailabilityService.DeleteTutorAvailabilityAsync(id);

                if (!result)
                    return NotFound(new { message = $"TutorAvailability with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tutor availability with ID {TutorAvailabilityId}", id);
                return StatusCode(500, new { message = "Error deleting tutor availability", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountTutorAvailabilities(
            [FromQuery] long? tutorId = null,
            [FromQuery] bool? isAvailable = null)
        {
            try
            {
                var count = await _tutorAvailabilityService.CountTutorAvailabilitiesAsync(tutorId, isAvailable);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting tutor availabilities");
                return StatusCode(500, new { message = "Error counting tutor availabilities", error = ex.Message });
            }
        }
    }
}