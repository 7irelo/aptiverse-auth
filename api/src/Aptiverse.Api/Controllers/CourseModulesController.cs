using Aptiverse.Api.Application.CourseModules.Dtos;
using Aptiverse.Api.Application.CourseModules.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/courses/{courseId}/modules")]
    public class CourseModulesController(
        ICourseModuleService moduleService,
        ILogger<CourseModulesController> logger) : ControllerBase
    {
        private readonly ICourseModuleService _moduleService = moduleService;
        private readonly ILogger<CourseModulesController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<CourseModuleDto>> CreateModule(
            long courseId,
            [FromBody] CreateCourseModuleDto createModuleDto)
        {
            try
            {
                if (createModuleDto.CourseId != courseId)
                {
                    return BadRequest(new { message = "Course ID in path does not match Course ID in request body" });
                }

                var createdModule = await _moduleService.CreateModuleAsync(createModuleDto);
                return CreatedAtAction(
                    nameof(GetModule),
                    new { courseId, id = createdModule.Id },
                    createdModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating module for course {CourseId}", courseId);
                return BadRequest(new { message = "Error creating module", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseModuleDto>> GetModule(long courseId, long id)
        {
            try
            {
                var module = await _moduleService.GetModuleByIdAsync(id);

                if (module == null)
                    return NotFound(new { message = $"Module with ID {id} not found" });

                if (module.CourseId != courseId)
                {
                    return NotFound(new { message = $"Module with ID {id} not found in course {courseId}" });
                }

                return Ok(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving module with ID {ModuleId} for course {CourseId}", id, courseId);
                return StatusCode(500, new { message = "Error retrieving module", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<CourseModuleDto>>> GetModules(
            long courseId,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = "Order",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _moduleService.GetModulesAsync(
                    currentUser: User,
                    courseId: courseId,
                    search: search,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving modules for course {CourseId}", courseId);
                return StatusCode(500, new { message = "Error retrieving modules", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CourseModuleDto>> UpdateModule(
            long courseId,
            long id,
            [FromBody] UpdateCourseModuleDto updateModuleDto)
        {
            try
            {
                var module = await _moduleService.GetModuleByIdAsync(id);
                if (module == null || module.CourseId != courseId)
                {
                    return NotFound(new { message = $"Module with ID {id} not found in course {courseId}" });
                }

                var updatedModule = await _moduleService.UpdateModuleAsync(id, updateModuleDto);
                return Ok(updatedModule);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Module with ID {ModuleId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating module with ID {ModuleId} for course {CourseId}", id, courseId);
                return BadRequest(new { message = "Error updating module", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(long courseId, long id)
        {
            try
            {
                var module = await _moduleService.GetModuleByIdAsync(id);
                if (module == null || module.CourseId != courseId)
                {
                    return NotFound(new { message = $"Module with ID {id} not found in course {courseId}" });
                }

                var result = await _moduleService.DeleteModuleAsync(id);

                if (!result)
                    return NotFound(new { message = $"Module with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting module with ID {ModuleId} from course {CourseId}", id, courseId);
                return StatusCode(500, new { message = "Error deleting module", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountModules(long courseId)
        {
            try
            {
                var count = await _moduleService.CountModulesAsync(User, courseId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting modules for course {CourseId}", courseId);
                return StatusCode(500, new { message = "Error counting modules", error = ex.Message });
            }
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<CourseModuleDto>>> ListModules(long courseId)
        {
            try
            {
                var modules = await _moduleService.GetModulesByCourseAsync(courseId);
                return Ok(modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving module list for course {CourseId}", courseId);
                return StatusCode(500, new { message = "Error retrieving modules", error = ex.Message });
            }
        }

        [HttpPost("reorder")]
        public async Task<IActionResult> ReorderModules(long courseId, [FromBody] List<long> moduleIds)
        {
            try
            {
                if (moduleIds == null || !moduleIds.Any())
                {
                    return BadRequest(new { message = "Module IDs list cannot be empty" });
                }

                await _moduleService.ReorderModulesAsync(courseId, moduleIds);
                return Ok(new { message = "Modules reordered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reordering modules for course {CourseId}", courseId);
                return StatusCode(500, new { message = "Error reordering modules", error = ex.Message });
            }
        }

        [HttpGet("total-duration")]
        public async Task<ActionResult<decimal>> GetCourseTotalDuration(long courseId)
        {
            try
            {
                var totalDuration = await _moduleService.GetCourseTotalDurationAsync(courseId);
                return Ok(new { totalDuration });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total duration for course {CourseId}", courseId);
                return StatusCode(500, new { message = "Error calculating total duration", error = ex.Message });
            }
        }
    }
}