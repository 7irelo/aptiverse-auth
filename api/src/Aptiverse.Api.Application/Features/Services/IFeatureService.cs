using Aptiverse.Api.Application.Features.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.Features.Services
{
    public interface IFeatureService
    {
        Task<FeatureDto> CreateFeatureAsync(CreateFeatureDto createFeatureDto);
        Task<FeatureDto?> GetFeatureByIdAsync(long id);
        Task<FeatureDto?> GetFeatureByCodeAsync(string code);

        Task<PaginatedResult<FeatureDto>> GetFeaturesAsync(
            ClaimsPrincipal currentUser,
            string? search = null,
            string? category = null,
            string? billingCycle = null,
            bool? isActive = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);

        Task<FeatureDto> UpdateFeatureAsync(long id, UpdateFeatureDto updateFeatureDto);
        Task<bool> DeleteFeatureAsync(long id);
        Task<int> CountFeaturesAsync(ClaimsPrincipal currentUser,
            string? category = null,
            string? billingCycle = null,
            bool? isActive = null);
        Task<bool> FeatureExistsAsync(long id);

        Task<IEnumerable<FeatureDto>> GetActiveFeaturesAsync();
        Task<IEnumerable<FeatureDto>> GetFeaturesByCategoryAsync(string category);
        Task<IEnumerable<FeatureDto>> GetFeaturesForUserAsync(string userId);
    }
}