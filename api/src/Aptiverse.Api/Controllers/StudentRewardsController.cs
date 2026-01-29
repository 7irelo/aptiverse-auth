using Aptiverse.Api.Application.StudentRewards.Dtos;
using Aptiverse.Api.Application.StudentRewards.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/student-rewards")]
    public class StudentRewardsController(
        IStudentRewardService studentRewardService,
        ILogger<StudentRewardsController> logger) : ControllerBase
    {
        private readonly IStudentRewardService _studentRewardService = studentRewardService;
        private readonly ILogger<StudentRewardsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<StudentRewardDto>> CreateStudentReward([FromBody] CreateStudentRewardDto createStudentRewardDto)
        {
            try
            {
                var createdStudentReward = await _studentRewardService.CreateStudentRewardAsync(createStudentRewardDto);
                return CreatedAtAction(nameof(GetStudentReward), new { id = createdStudentReward.Id }, createdStudentReward);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student reward");
                return BadRequest(new { message = "Error creating student reward", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentRewardDto>> GetStudentReward(long id)
        {
            try
            {
                var studentReward = await _studentRewardService.GetStudentRewardByIdAsync(id);

                if (studentReward == null)
                    return NotFound(new { message = $"StudentReward with ID {id} not found" });

                return Ok(studentReward);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student reward with ID {StudentRewardId}", id);
                return StatusCode(500, new { message = "Error retrieving student reward", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<StudentRewardDto>>> GetStudentRewards(
            [FromQuery] long? studentId = null,
            [FromQuery] long? rewardId = null,
            [FromQuery] long? goalId = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? earnedAfter = null,
            [FromQuery] DateTime? earnedBefore = null,
            [FromQuery] DateTime? expiresBefore = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _studentRewardService.GetStudentRewardsAsync(
                    studentId: studentId,
                    rewardId: rewardId,
                    goalId: goalId,
                    status: status,
                    earnedAfter: earnedAfter,
                    earnedBefore: earnedBefore,
                    expiresBefore: expiresBefore,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student rewards");
                return StatusCode(500, new { message = "Error retrieving student rewards", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StudentRewardDto>> UpdateStudentReward(long id, [FromBody] UpdateStudentRewardDto updateStudentRewardDto)
        {
            try
            {
                var updatedStudentReward = await _studentRewardService.UpdateStudentRewardAsync(id, updateStudentRewardDto);
                return Ok(updatedStudentReward);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "StudentReward with ID {StudentRewardId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student reward with ID {StudentRewardId}", id);
                return BadRequest(new { message = "Error updating student reward", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentReward(long id)
        {
            try
            {
                var result = await _studentRewardService.DeleteStudentRewardAsync(id);

                if (!result)
                    return NotFound(new { message = $"StudentReward with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student reward with ID {StudentRewardId}", id);
                return StatusCode(500, new { message = "Error deleting student reward", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountStudentRewards(
            [FromQuery] long? studentId = null,
            [FromQuery] long? rewardId = null,
            [FromQuery] string? status = null)
        {
            try
            {
                var count = await _studentRewardService.CountStudentRewardsAsync(studentId, rewardId, status);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting student rewards");
                return StatusCode(500, new { message = "Error counting student rewards", error = ex.Message });
            }
        }
    }
}