using Aptiverse.Api.Application.ResourceDownloads.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.ResourceDownloads.Services
{
    public interface IResourceDownloadService
    {
        Task<ResourceDownloadDto> CreateResourceDownloadAsync(CreateResourceDownloadDto createResourceDownloadDto);
        Task<ResourceDownloadDto?> GetResourceDownloadByIdAsync(long id);

        Task<PaginatedResult<ResourceDownloadDto>> GetResourceDownloadsAsync(
            ClaimsPrincipal currentUser,
            long? resourceId = null,
            long? studentId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? sortBy = "DownloadedAt",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20);

        Task<bool> DeleteResourceDownloadAsync(long id);
        Task<int> CountResourceDownloadsAsync(ClaimsPrincipal currentUser,
            long? resourceId = null,
            long? studentId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);
        Task<bool> ResourceDownloadExistsAsync(long id);

        Task<IEnumerable<ResourceDownloadDto>> GetDownloadsByResourceAsync(long resourceId);
        Task<IEnumerable<ResourceDownloadDto>> GetDownloadsByStudentAsync(long studentId);
        Task<IEnumerable<ResourceDownloadDto>> GetRecentDownloadsAsync(long studentId, int count = 10);
        Task<bool> HasStudentDownloadedResourceAsync(long studentId, long resourceId);
        Task<int> GetDownloadCountByResourceAsync(long resourceId);
        Task<int> GetDownloadCountByStudentAsync(long studentId);
        Task<Dictionary<long, int>> GetPopularResourcesAsync(int count = 10, DateTime? fromDate = null, DateTime? toDate = null);
    }
}