using Aptiverse.Api.Application.UserFeatures.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.UserFeatures.Services
{
    public interface IUserFeatureService
    {
        Task<UserFeatureDto> CreateUserFeatureAsync(CreateUserFeatureDto createUserFeatureDto);
        Task<UserFeatureDto?> GetUserFeatureByIdAsync(long id);

        Task<PaginatedResult<UserFeatureDto>> GetUserFeaturesAsync(
            ClaimsPrincipal currentUser,
            string? userId = null,
            long? featureId = null,
            string? grantType = null,
            bool? isActive = null,
            string? sortBy = "GrantedAt",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20);

        Task<UserFeatureDto> UpdateUserFeatureAsync(long id, UpdateUserFeatureDto updateUserFeatureDto);
        Task<bool> DeleteUserFeatureAsync(long id);
        Task<int> CountUserFeaturesAsync(ClaimsPrincipal currentUser,
            string? userId = null,
            long? featureId = null,
            string? grantType = null,
            bool? isActive = null);
        Task<bool> UserFeatureExistsAsync(long id);

        Task<IEnumerable<UserFeatureDto>> GetUserFeaturesByUserAsync(string userId);
        Task<IEnumerable<UserFeatureDto>> GetUserFeaturesByFeatureAsync(long featureId);
        Task<IEnumerable<UserFeatureDto>> GetActiveUserFeaturesAsync(string userId);
        Task<bool> HasActiveFeatureAsync(string userId, long featureId);
        Task<bool> HasAnyActiveFeatureAsync(string userId, List<long> featureIds);
        Task<bool> ExistsAsync(string userId, long featureId);
    }
}