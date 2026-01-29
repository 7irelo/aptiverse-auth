using Aptiverse.Api.Application.StudentSubjectTopics.Dtos;
using Aptiverse.Api.Application.StudentSubjectTopics.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/student-subject-topics")]
    public class StudentSubjectTopicsController(
        IStudentSubjectTopicService studentSubjectTopicService,
        ILogger<StudentSubjectTopicsController> logger) : ControllerBase
    {
        private readonly IStudentSubjectTopicService _studentSubjectTopicService = studentSubjectTopicService;
        private readonly ILogger<StudentSubjectTopicsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<StudentSubjectTopicDto>> CreateStudentSubjectTopic([FromBody] CreateStudentSubjectTopicDto createStudentSubjectTopicDto)
        {
            try
            {
                var createdStudentSubjectTopic = await _studentSubjectTopicService.CreateStudentSubjectTopicAsync(createStudentSubjectTopicDto);
                return CreatedAtAction(nameof(GetStudentSubjectTopic), new { id = createdStudentSubjectTopic.Id }, createdStudentSubjectTopic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student subject topic");
                return BadRequest(new { message = "Error creating student subject topic", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentSubjectTopicDto>> GetStudentSubjectTopic(long id)
        {
            try
            {
                var studentSubjectTopic = await _studentSubjectTopicService.GetStudentSubjectTopicByIdAsync(id);

                if (studentSubjectTopic == null)
                    return NotFound(new { message = $"StudentSubjectTopic with ID {id} not found" });

                return Ok(studentSubjectTopic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student subject topic with ID {StudentSubjectTopicId}", id);
                return StatusCode(500, new { message = "Error retrieving student subject topic", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<StudentSubjectTopicDto>>> GetStudentSubjectTopics(
            [FromQuery] long? studentSubjectId = null,
            [FromQuery] long? topicId = null,
            [FromQuery] double? minScore = null,
            [FromQuery] double? maxScore = null,
            [FromQuery] string? trend = null,
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

                var result = await _studentSubjectTopicService.GetStudentSubjectTopicsAsync(
                    studentSubjectId: studentSubjectId,
                    topicId: topicId,
                    minScore: minScore,
                    maxScore: maxScore,
                    trend: trend,
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
                _logger.LogError(ex, "Error retrieving student subject topics");
                return StatusCode(500, new { message = "Error retrieving student subject topics", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StudentSubjectTopicDto>> UpdateStudentSubjectTopic(long id, [FromBody] UpdateStudentSubjectTopicDto updateStudentSubjectTopicDto)
        {
            try
            {
                var updatedStudentSubjectTopic = await _studentSubjectTopicService.UpdateStudentSubjectTopicAsync(id, updateStudentSubjectTopicDto);
                return Ok(updatedStudentSubjectTopic);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "StudentSubjectTopic with ID {StudentSubjectTopicId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student subject topic with ID {StudentSubjectTopicId}", id);
                return BadRequest(new { message = "Error updating student subject topic", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentSubjectTopic(long id)
        {
            try
            {
                var result = await _studentSubjectTopicService.DeleteStudentSubjectTopicAsync(id);

                if (!result)
                    return NotFound(new { message = $"StudentSubjectTopic with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student subject topic with ID {StudentSubjectTopicId}", id);
                return StatusCode(500, new { message = "Error deleting student subject topic", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountStudentSubjectTopics(
            [FromQuery] long? studentSubjectId = null,
            [FromQuery] long? topicId = null,
            [FromQuery] string? trend = null)
        {
            try
            {
                var count = await _studentSubjectTopicService.CountStudentSubjectTopicsAsync(studentSubjectId, topicId, trend);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting student subject topics");
                return StatusCode(500, new { message = "Error counting student subject topics", error = ex.Message });
            }
        }
    }
}