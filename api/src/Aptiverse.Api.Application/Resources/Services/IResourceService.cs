using Aptiverse.Api.Application.Resources.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.Resources.Services
{
    public interface IResourceService
    {
        Task<ResourceDto> CreateResourceAsync(CreateResourceDto createResourceDto);
        Task<ResourceDto?> GetResourceByIdAsync(long id);

        Task<PaginatedResult<ResourceDto>> GetResourcesAsync(
            ClaimsPrincipal currentUser,
            string? search = null,
            string? subjectId = null,
            string? resourceType = null,
            string? gradeLevel = null,
            long? teacherId = null,
            long? tutorId = null,
            long? courseId = null,
            bool? isFree = null,
            bool? isApproved = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortBy = "CreatedAt",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20);

        Task<ResourceDto> UpdateResourceAsync(long id, UpdateResourceDto updateResourceDto);
        Task<bool> DeleteResourceAsync(long id);
        Task<int> CountResourcesAsync(ClaimsPrincipal currentUser,
            string? subjectId = null,
            string? resourceType = null,
            bool? isFree = null,
            bool? isApproved = null);
        Task<bool> ResourceExistsAsync(long id);

        Task<IEnumerable<ResourceDto>> GetResourcesBySubjectAsync(string subjectId);
        Task<IEnumerable<ResourceDto>> GetResourcesByTeacherAsync(long teacherId);
        Task<IEnumerable<ResourceDto>> GetResourcesByTutorAsync(long tutorId);
        Task<IEnumerable<ResourceDto>> GetResourcesByCourseAsync(long courseId);
        Task<IEnumerable<ResourceDto>> GetPopularResourcesAsync(int count = 10);
        Task<IEnumerable<ResourceDto>> GetFreeResourcesAsync();
        Task<ResourceDto> IncrementDownloadCountAsync(long id);
        Task<ResourceDto> UpdateResourceRatingAsync(long id, double newRating);
    }
}