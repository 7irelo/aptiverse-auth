using Aptiverse.Api.Application.WeeklyStudyHours.Dtos;
using Aptiverse.Api.Application.WeeklyStudyHours.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/weekly-study-hours")]
    public class WeeklyStudyHoursController(
        IWeeklyStudyHourService weeklyStudyHourService,
        ILogger<WeeklyStudyHoursController> logger) : ControllerBase
    {
        private readonly IWeeklyStudyHourService _weeklyStudyHourService = weeklyStudyHourService;
        private readonly ILogger<WeeklyStudyHoursController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<WeeklyStudyHourDto>> CreateWeeklyStudyHour([FromBody] CreateWeeklyStudyHourDto createWeeklyStudyHourDto)
        {
            try
            {
                var createdWeeklyStudyHour = await _weeklyStudyHourService.CreateWeeklyStudyHourAsync(createWeeklyStudyHourDto);
                return CreatedAtAction(nameof(GetWeeklyStudyHour), new { id = createdWeeklyStudyHour.Id }, createdWeeklyStudyHour);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating weekly study hour");
                return BadRequest(new { message = "Error creating weekly study hour", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WeeklyStudyHourDto>> GetWeeklyStudyHour(long id)
        {
            try
            {
                var weeklyStudyHour = await _weeklyStudyHourService.GetWeeklyStudyHourByIdAsync(id);

                if (weeklyStudyHour == null)
                    return NotFound(new { message = $"WeeklyStudyHour with ID {id} not found" });

                return Ok(weeklyStudyHour);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving weekly study hour with ID {WeeklyStudyHourId}", id);
                return StatusCode(500, new { message = "Error retrieving weekly study hour", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<WeeklyStudyHourDto>>> GetWeeklyStudyHours(
            [FromQuery] long? studentSubjectId = null,
            [FromQuery] int? weekNumber = null,
            [FromQuery] int? minHours = null,
            [FromQuery] int? maxHours = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _weeklyStudyHourService.GetWeeklyStudyHoursAsync(
                    studentSubjectId: studentSubjectId,
                    weekNumber: weekNumber,
                    minHours: minHours,
                    maxHours: maxHours,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving weekly study hours");
                return StatusCode(500, new { message = "Error retrieving weekly study hours", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WeeklyStudyHourDto>> UpdateWeeklyStudyHour(long id, [FromBody] UpdateWeeklyStudyHourDto updateWeeklyStudyHourDto)
        {
            try
            {
                var updatedWeeklyStudyHour = await _weeklyStudyHourService.UpdateWeeklyStudyHourAsync(id, updateWeeklyStudyHourDto);
                return Ok(updatedWeeklyStudyHour);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "WeeklyStudyHour with ID {WeeklyStudyHourId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating weekly study hour with ID {WeeklyStudyHourId}", id);
                return BadRequest(new { message = "Error updating weekly study hour", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeeklyStudyHour(long id)
        {
            try
            {
                var result = await _weeklyStudyHourService.DeleteWeeklyStudyHourAsync(id);

                if (!result)
                    return NotFound(new { message = $"WeeklyStudyHour with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting weekly study hour with ID {WeeklyStudyHourId}", id);
                return StatusCode(500, new { message = "Error deleting weekly study hour", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountWeeklyStudyHours(
            [FromQuery] long? studentSubjectId = null,
            [FromQuery] int? weekNumber = null)
        {
            try
            {
                var count = await _weeklyStudyHourService.CountWeeklyStudyHoursAsync(studentSubjectId, weekNumber);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting weekly study hours");
                return StatusCode(500, new { message = "Error counting weekly study hours", error = ex.Message });
            }
        }
    }
}