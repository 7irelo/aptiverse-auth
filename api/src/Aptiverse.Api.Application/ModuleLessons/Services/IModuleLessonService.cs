using Aptiverse.Api.Application.ModuleLessons.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.ModuleLessons.Services
{
    public interface IModuleLessonService
    {
        Task<ModuleLessonDto> CreateLessonAsync(CreateModuleLessonDto createLessonDto);
        Task<ModuleLessonDto?> GetLessonByIdAsync(long id);

        Task<PaginatedResult<ModuleLessonDto>> GetLessonsAsync(
            ClaimsPrincipal currentUser,
            long? moduleId = null,
            string? search = null,
            bool? isFreePreview = null,
            string? sortBy = "Order",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);

        Task<ModuleLessonDto> UpdateLessonAsync(long id, UpdateModuleLessonDto updateLessonDto);
        Task<bool> DeleteLessonAsync(long id);
        Task<int> CountLessonsAsync(ClaimsPrincipal currentUser, long? moduleId = null, bool? isFreePreview = null);
        Task<bool> LessonExistsAsync(long id);

        Task<IEnumerable<ModuleLessonDto>> GetLessonsByModuleAsync(long moduleId);
        Task ReorderLessonsAsync(long moduleId, List<long> lessonIdsInOrder);
        Task<decimal> GetModuleTotalDurationAsync(long moduleId);
    }
}