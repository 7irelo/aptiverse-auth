using Aptiverse.Api.Application.Topics.Dtos;
using Aptiverse.Api.Application.Topics.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/topics")]
    public class TopicsController(
        ITopicService topicService,
        ILogger<TopicsController> logger) : ControllerBase
    {
        private readonly ITopicService _topicService = topicService;
        private readonly ILogger<TopicsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<TopicDto>> CreateTopic([FromBody] CreateTopicDto createTopicDto)
        {
            try
            {
                var createdTopic = await _topicService.CreateTopicAsync(createTopicDto);
                return CreatedAtAction(nameof(GetTopic), new { id = createdTopic.Id }, createdTopic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating topic");
                return BadRequest(new { message = "Error creating topic", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TopicDto>> GetTopic(long id)
        {
            try
            {
                var topic = await _topicService.GetTopicByIdAsync(id);

                if (topic == null)
                    return NotFound(new { message = $"Topic with ID {id} not found" });

                return Ok(topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving topic with ID {TopicId}", id);
                return StatusCode(500, new { message = "Error retrieving topic", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<TopicDto>>> GetTopics(
            [FromQuery] string? subjectId = null,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _topicService.GetTopicsAsync(
                    subjectId: subjectId,
                    search: search,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving topics");
                return StatusCode(500, new { message = "Error retrieving topics", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TopicDto>> UpdateTopic(long id, [FromBody] UpdateTopicDto updateTopicDto)
        {
            try
            {
                var updatedTopic = await _topicService.UpdateTopicAsync(id, updateTopicDto);
                return Ok(updatedTopic);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Topic with ID {TopicId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating topic with ID {TopicId}", id);
                return BadRequest(new { message = "Error updating topic", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(long id)
        {
            try
            {
                var result = await _topicService.DeleteTopicAsync(id);

                if (!result)
                    return NotFound(new { message = $"Topic with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting topic with ID {TopicId}", id);
                return StatusCode(500, new { message = "Error deleting topic", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountTopics(
            [FromQuery] string? subjectId = null,
            [FromQuery] string? search = null)
        {
            try
            {
                var count = await _topicService.CountTopicsAsync(subjectId, search);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting topics");
                return StatusCode(500, new { message = "Error counting topics", error = ex.Message });
            }
        }
    }
}