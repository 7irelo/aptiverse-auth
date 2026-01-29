using Aptiverse.Api.Application.Courses.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.Courses.Services
{
    public interface ICourseService
    {
        Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto);
        Task<CourseDto?> GetCourseByIdAsync(long id);

        Task<PaginatedResult<CourseDto>> GetCoursesAsync(
            ClaimsPrincipal currentUser,
            string? search = null,
            string? subjectId = null,
            string? level = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? isPublished = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);

        Task<CourseDto> UpdateCourseAsync(long id, UpdateCourseDto updateCourseDto);
        Task<bool> DeleteCourseAsync(long id);
        Task<int> CountCoursesAsync(ClaimsPrincipal currentUser,
            string? subjectId = null,
            string? level = null,
            bool? isPublished = null);
        Task<bool> CourseExistsAsync(long id);

        Task<IEnumerable<CourseDto>> GetPopularCoursesAsync(int count = 10);
        Task<IEnumerable<CourseDto>> GetCoursesByTeacherAsync(long teacherId);
        Task<IEnumerable<CourseDto>> GetCoursesByTutorAsync(long tutorId);
        Task<IEnumerable<CourseDto>> GetRecommendedCoursesAsync(string userId);
    }
}