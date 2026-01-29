using Aptiverse.Api.Application.Courses.Dtos;
using Aptiverse.Api.Application.Courses.Services;
using Aptiverse.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aptiverse.Api.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CoursesController(
        ICourseService courseService,
        ILogger<CoursesController> logger) : ControllerBase
    {
        private readonly ICourseService _courseService = courseService;
        private readonly ILogger<CoursesController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto createCourseDto)
        {
            try
            {
                var createdCourse = await _courseService.CreateCourseAsync(createCourseDto);
                return CreatedAtAction(nameof(GetCourse), new { id = createdCourse.Id }, createdCourse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                return BadRequest(new { message = "Error creating course", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(long id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);

                if (course == null)
                    return NotFound(new { message = $"Course with ID {id} not found" });

                return Ok(course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course with ID {CourseId}", id);
                return StatusCode(500, new { message = "Error retrieving course", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<CourseDto>>> GetCourses(
            [FromQuery] string? search = null,
            [FromQuery] string? subjectId = null,
            [FromQuery] string? level = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] bool? isPublished = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                // Validate pagination parameters
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                // Validate price range
                if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                {
                    return BadRequest(new { message = "minPrice cannot be greater than maxPrice" });
                }

                var result = await _courseService.GetCoursesAsync(
                    currentUser: User,
                    search: search,
                    subjectId: subjectId,
                    level: level,
                    minPrice: minPrice,
                    maxPrice: maxPrice,
                    isPublished: isPublished,
                    sortBy: sortBy,
                    sortDescending: sortDescending,
                    page: page,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses");
                return StatusCode(500, new { message = "Error retrieving courses", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDto>> UpdateCourse(long id, [FromBody] UpdateCourseDto updateCourseDto)
        {
            try
            {
                var updatedCourse = await _courseService.UpdateCourseAsync(id, updateCourseDto);
                return Ok(updatedCourse);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Course with ID {CourseId} not found for update", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course with ID {CourseId}", id);
                return BadRequest(new { message = "Error updating course", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(long id)
        {
            try
            {
                var result = await _courseService.DeleteCourseAsync(id);

                if (!result)
                    return NotFound(new { message = $"Course with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course with ID {CourseId}", id);
                return StatusCode(500, new { message = "Error deleting course", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountCourses(
            [FromQuery] string? subjectId = null,
            [FromQuery] string? level = null,
            [FromQuery] bool? isPublished = null)
        {
            try
            {
                var count = await _courseService.CountCoursesAsync(User, subjectId, level, isPublished);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting courses");
                return StatusCode(500, new { message = "Error counting courses", error = ex.Message });
            }
        }

        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetPopularCourses(
            [FromQuery] int count = 10)
        {
            try
            {
                if (count < 1 || count > 50)
                    count = 10;

                var courses = await _courseService.GetPopularCoursesAsync(count);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving popular courses");
                return StatusCode(500, new { message = "Error retrieving popular courses", error = ex.Message });
            }
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesByTeacher(long teacherId)
        {
            try
            {
                var courses = await _courseService.GetCoursesByTeacherAsync(teacherId);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses for teacher {TeacherId}", teacherId);
                return StatusCode(500, new { message = "Error retrieving courses", error = ex.Message });
            }
        }

        [HttpGet("tutor/{tutorId}")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesByTutor(long tutorId)
        {
            try
            {
                var courses = await _courseService.GetCoursesByTutorAsync(tutorId);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses for tutor {TutorId}", tutorId);
                return StatusCode(500, new { message = "Error retrieving courses", error = ex.Message });
            }
        }

        [HttpGet("recommended")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetRecommendedCourses()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var courses = await _courseService.GetRecommendedCoursesAsync(userId);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recommended courses");
                return StatusCode(500, new { message = "Error retrieving recommended courses", error = ex.Message });
            }
        }
    }
}