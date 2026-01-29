using Aptiverse.Api.Application.RewardFeatures.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.RewardFeatures.Services
{
    public interface IRewardFeatureService
    {
        Task<RewardFeatureDto> CreateRewardFeatureAsync(CreateRewardFeatureDto createRewardFeatureDto);
        Task<RewardFeatureDto?> GetRewardFeatureByIdAsync(long id);

        Task<PaginatedResult<RewardFeatureDto>> GetRewardFeaturesAsync(
            ClaimsPrincipal currentUser,
            long? rewardId = null,
            long? featureId = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);

        Task<RewardFeatureDto> UpdateRewardFeatureAsync(long id, UpdateRewardFeatureDto updateRewardFeatureDto);
        Task<bool> DeleteRewardFeatureAsync(long id);
        Task<int> CountRewardFeaturesAsync(ClaimsPrincipal currentUser,
            long? rewardId = null,
            long? featureId = null);
        Task<bool> RewardFeatureExistsAsync(long id);

        Task<IEnumerable<RewardFeatureDto>> GetRewardFeaturesByRewardAsync(long rewardId);
        Task<IEnumerable<RewardFeatureDto>> GetRewardFeaturesByFeatureAsync(long featureId);
        Task<bool> ExistsAsync(long rewardId, long featureId);
    }
}