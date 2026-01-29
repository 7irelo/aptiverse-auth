using Aptiverse.Api.Application.CourseModules.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.CourseModules.Services
{
    public interface ICourseModuleService
    {
        Task<CourseModuleDto> CreateModuleAsync(CreateCourseModuleDto createModuleDto);
        Task<CourseModuleDto?> GetModuleByIdAsync(long id);

        Task<PaginatedResult<CourseModuleDto>> GetModulesAsync(
            ClaimsPrincipal currentUser,
            long? courseId = null,
            string? search = null,
            string? sortBy = "Order",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);

        Task<CourseModuleDto> UpdateModuleAsync(long id, UpdateCourseModuleDto updateModuleDto);
        Task<bool> DeleteModuleAsync(long id);
        Task<int> CountModulesAsync(ClaimsPrincipal currentUser, long? courseId = null);
        Task<bool> ModuleExistsAsync(long id);

        // Additional methods
        Task<IEnumerable<CourseModuleDto>> GetModulesByCourseAsync(long courseId);
        Task ReorderModulesAsync(long courseId, List<long> moduleIdsInOrder);
        Task<decimal> GetCourseTotalDurationAsync(long courseId);
    }
}