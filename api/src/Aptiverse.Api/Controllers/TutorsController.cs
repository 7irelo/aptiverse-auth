using Aptiverse.Api.Application.Tutors.Dtos;
using Aptiverse.Api.Application.Tutors.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/tutors")]
    public class TutorsController(
        ITutorService tutorService,
        ILogger<TutorsController> logger) : ControllerBase
    {
        private readonly ITutorService _tutorService = tutorService;
        private readonly ILogger<TutorsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<TutorDto>> CreateTutor([FromBody] CreateTutorDto createTutorDto)
        {
            try
            {
                var createdTutor = await _tutorService.CreateTutorAsync(createTutorDto);
                return CreatedAtAction(nameof(GetTutor), new { id = createdTutor.Id }, createdTutor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tutor");
                return BadRequest(new { message = "Error creating tutor", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TutorDto>> GetTutor(long id)
        {
            try
            {
                var tutor = await _tutorService.GetTutorByIdAsync(id);

                if (tutor == null)
                    return NotFound(new { message = $"Tutor with ID {id} not found" });

                return Ok(tutor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tutor with ID {TutorId}", id);
                return StatusCode(500, new { message = "Error retrieving tutor", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<TutorDto>>> GetTutors(
            [FromQuery] string? search = null,
            [FromQuery] string? specialization = null,
            [FromQuery] string? teachingStyle = null,
            [FromQuery] bool? isVerified = null,
            [FromQuery] decimal? minHourlyRate = null,
            [FromQuery] decimal? maxHourlyRate = null,
            [FromQuery] double? minRating = null,
            [FromQuery] double? maxRating = null,
            [FromQuery] int? minYearsOfExperience = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _tutorService.GetTutorsAsync(
                    search: search,
                    specialization: specialization,
                    teachingStyle: teachingStyle,
                    isVerified: isVerified,
                    minHourlyRate: minHourlyRate,
                    maxHourlyRate: maxHourlyRate,
                    minRating: minRating,
                    maxRating: maxRating,
                    minYearsOfExperience: minYearsOfExperience,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tutors");
                return StatusCode(500, new { message = "Error retrieving tutors", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TutorDto>> UpdateTutor(long id, [FromBody] UpdateTutorDto updateTutorDto)
        {
            try
            {
                var updatedTutor = await _tutorService.UpdateTutorAsync(id, updateTutorDto);
                return Ok(updatedTutor);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Tutor with ID {TutorId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tutor with ID {TutorId}", id);
                return BadRequest(new { message = "Error updating tutor", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTutor(long id)
        {
            try
            {
                var result = await _tutorService.DeleteTutorAsync(id);

                if (!result)
                    return NotFound(new { message = $"Tutor with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tutor with ID {TutorId}", id);
                return StatusCode(500, new { message = "Error deleting tutor", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountTutors(
            [FromQuery] string? specialization = null,
            [FromQuery] bool? isVerified = null)
        {
            try
            {
                var count = await _tutorService.CountTutorsAsync(specialization, isVerified);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting tutors");
                return StatusCode(500, new { message = "Error counting tutors", error = ex.Message });
            }
        }
    }
}