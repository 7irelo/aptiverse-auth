using Aptiverse.Api.Application.ModuleLessons.Dtos;
using Aptiverse.Api.Application.ModuleLessons.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/modules/{moduleId}/lessons")]
    public class ModuleLessonsController(
        IModuleLessonService lessonService,
        ILogger<ModuleLessonsController> logger) : ControllerBase
    {
        private readonly IModuleLessonService _lessonService = lessonService;
        private readonly ILogger<ModuleLessonsController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<ModuleLessonDto>> CreateLesson(
            long moduleId,
            [FromBody] CreateModuleLessonDto createLessonDto)
        {
            try
            {
                if (createLessonDto.ModuleId != moduleId)
                {
                    return BadRequest(new { message = "Module ID in path does not match Module ID in request body" });
                }

                var createdLesson = await _lessonService.CreateLessonAsync(createLessonDto);
                return CreatedAtAction(
                    nameof(GetLesson),
                    new { moduleId, id = createdLesson.Id },
                    createdLesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lesson for module {ModuleId}", moduleId);
                return BadRequest(new { message = "Error creating lesson", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ModuleLessonDto>> GetLesson(long moduleId, long id)
        {
            try
            {
                var lesson = await _lessonService.GetLessonByIdAsync(id);

                if (lesson == null)
                    return NotFound(new { message = $"Lesson with ID {id} not found" });

                if (lesson.ModuleId != moduleId)
                {
                    return NotFound(new { message = $"Lesson with ID {id} not found in module {moduleId}" });
                }

                return Ok(lesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lesson with ID {LessonId} for module {ModuleId}", id, moduleId);
                return StatusCode(500, new { message = "Error retrieving lesson", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ModuleLessonDto>>> GetLessons(
            long moduleId,
            [FromQuery] string? search = null,
            [FromQuery] bool? isFreePreview = null,
            [FromQuery] string? sortBy = "Order",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _lessonService.GetLessonsAsync(
                    currentUser: User,
                    moduleId: moduleId,
                    search: search,
                    isFreePreview: isFreePreview,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lessons for module {ModuleId}", moduleId);
                return StatusCode(500, new { message = "Error retrieving lessons", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ModuleLessonDto>> UpdateLesson(
            long moduleId,
            long id,
            [FromBody] UpdateModuleLessonDto updateLessonDto)
        {
            try
            {
                var lesson = await _lessonService.GetLessonByIdAsync(id);
                if (lesson == null || lesson.ModuleId != moduleId)
                {
                    return NotFound(new { message = $"Lesson with ID {id} not found in module {moduleId}" });
                }

                var updatedLesson = await _lessonService.UpdateLessonAsync(id, updateLessonDto);
                return Ok(updatedLesson);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Lesson with ID {LessonId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson with ID {LessonId} for module {ModuleId}", id, moduleId);
                return BadRequest(new { message = "Error updating lesson", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(long moduleId, long id)
        {
            try
            {
                var lesson = await _lessonService.GetLessonByIdAsync(id);
                if (lesson == null || lesson.ModuleId != moduleId)
                {
                    return NotFound(new { message = $"Lesson with ID {id} not found in module {moduleId}" });
                }

                var result = await _lessonService.DeleteLessonAsync(id);

                if (!result)
                    return NotFound(new { message = $"Lesson with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lesson with ID {LessonId} from module {ModuleId}", id, moduleId);
                return StatusCode(500, new { message = "Error deleting lesson", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountLessons(long moduleId, [FromQuery] bool? isFreePreview = null)
        {
            try
            {
                var count = await _lessonService.CountLessonsAsync(User, moduleId, isFreePreview);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting lessons for module {ModuleId}", moduleId);
                return StatusCode(500, new { message = "Error counting lessons", error = ex.Message });
            }
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<ModuleLessonDto>>> ListLessons(long moduleId)
        {
            try
            {
                var lessons = await _lessonService.GetLessonsByModuleAsync(moduleId);
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lesson list for module {ModuleId}", moduleId);
                return StatusCode(500, new { message = "Error retrieving lessons", error = ex.Message });
            }
        }

        [HttpPost("reorder")]
        public async Task<IActionResult> ReorderLessons(long moduleId, [FromBody] List<long> lessonIds)
        {
            try
            {
                if (lessonIds == null || !lessonIds.Any())
                {
                    return BadRequest(new { message = "Lesson IDs list cannot be empty" });
                }

                await _lessonService.ReorderLessonsAsync(moduleId, lessonIds);
                return Ok(new { message = "Lessons reordered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reordering lessons for module {ModuleId}", moduleId);
                return StatusCode(500, new { message = "Error reordering lessons", error = ex.Message });
            }
        }

        [HttpGet("total-duration")]
        public async Task<ActionResult<decimal>> GetModuleTotalDuration(long moduleId)
        {
            try
            {
                var totalDuration = await _lessonService.GetModuleTotalDurationAsync(moduleId);
                return Ok(new { totalDuration });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total duration for module {ModuleId}", moduleId);
                return StatusCode(500, new { message = "Error calculating total duration", error = ex.Message });
            }
        }
    }
}