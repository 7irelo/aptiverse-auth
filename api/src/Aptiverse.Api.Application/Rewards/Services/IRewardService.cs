using Aptiverse.Api.Application.Rewards.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.Rewards.Services
{
    public interface IRewardService
    {
        Task<RewardDto> CreateRewardAsync(CreateRewardDto createRewardDto);
        Task<RewardDto?> GetRewardByIdAsync(long id);
        Task<PaginatedResult<RewardDto>> GetRewardsAsync(
            string? search = null,
            string? rewardType = null,
            int? difficultyTier = null,
            bool? isActive = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<RewardDto> UpdateRewardAsync(long id, UpdateRewardDto updateRewardDto);
        Task<bool> DeleteRewardAsync(long id);
        Task<int> CountRewardsAsync(string? rewardType = null, bool? isActive = null);
        Task<bool> RewardExistsAsync(long id);
    }
}