using Aptiverse.Api.Application.Rewards.Dtos;
using Aptiverse.Api.Domain.Models.Rewards;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.Rewards.Services
{
    public class RewardService(
        IRepository<Reward> rewardRepository,
        IMapper mapper) : IRewardService
    {
        private readonly IRepository<Reward> _rewardRepository = rewardRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<RewardDto> CreateRewardAsync(CreateRewardDto createRewardDto)
        {
            ArgumentNullException.ThrowIfNull(createRewardDto);

            Reward reward = _mapper.Map<Reward>(createRewardDto);
            await _rewardRepository.AddAsync(reward);
            return _mapper.Map<RewardDto>(reward);
        }

        public async Task<RewardDto?> GetRewardByIdAsync(long id)
        {
            var reward = await _rewardRepository.GetAsync(
                predicate: r => r.Id == id,
                include: query => query
                    .Include(r => r.RewardFeatures)
                    .Include(r => r.StudentRewards)
                    .Include(r => r.ApplicableGoals),
                disableTracking: false);

            if (reward == null)
                return null;

            return _mapper.Map<RewardDto>(reward);
        }

        public async Task<PaginatedResult<RewardDto>> GetRewardsAsync(
            string? search = null,
            string? rewardType = null,
            int? difficultyTier = null,
            bool? isActive = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<Reward, bool>>? predicate = BuildFilterPredicate(search, rewardType, difficultyTier, isActive);
            Func<IQueryable<Reward>, IOrderedQueryable<Reward>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _rewardRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(r => r.RewardFeatures)
                    .Include(r => r.StudentRewards)
                    .Include(r => r.ApplicableGoals));

            var rewardDtos = _mapper.Map<List<RewardDto>>(paginatedResult.Data);

            return new PaginatedResult<RewardDto>(
                rewardDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<Reward, bool>>? BuildFilterPredicate(
            string? search,
            string? rewardType,
            int? difficultyTier,
            bool? isActive)
        {
            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(rewardType) &&
                !difficultyTier.HasValue && !isActive.HasValue)
                return null;

            return r =>
                (string.IsNullOrEmpty(search) || r.Name.Contains(search) || r.Description.Contains(search)) &&
                (string.IsNullOrEmpty(rewardType) || r.RewardType == rewardType) &&
                (!difficultyTier.HasValue || r.DifficultyTier == difficultyTier.Value) &&
                (!isActive.HasValue || r.IsActive == isActive.Value);
        }

        private Func<IQueryable<Reward>, IOrderedQueryable<Reward>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "name" => sortDescending
                    ? query => query.OrderByDescending(r => r.Name).ThenByDescending(r => r.Id)
                    : query => query.OrderBy(r => r.Name).ThenBy(r => r.Id),
                "pointscost" => sortDescending
                    ? query => query.OrderByDescending(r => r.PointsCost).ThenByDescending(r => r.Id)
                    : query => query.OrderBy(r => r.PointsCost).ThenBy(r => r.Id),
                "difficultytier" => sortDescending
                    ? query => query.OrderByDescending(r => r.DifficultyTier).ThenByDescending(r => r.Id)
                    : query => query.OrderBy(r => r.DifficultyTier).ThenBy(r => r.Id),
                "createdat" => sortDescending
                    ? query => query.OrderByDescending(r => r.CreatedAt).ThenByDescending(r => r.Id)
                    : query => query.OrderBy(r => r.CreatedAt).ThenBy(r => r.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(r => r.Id)
                    : query => query.OrderBy(r => r.Id)
            };
        }

        public async Task<RewardDto> UpdateRewardAsync(long id, UpdateRewardDto updateRewardDto)
        {
            var existingReward = await _rewardRepository.GetAsync(
                predicate: r => r.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Reward with ID {id} not found");

            _mapper.Map(updateRewardDto, existingReward);
            await _rewardRepository.UpdateAsync(existingReward);
            return _mapper.Map<RewardDto>(existingReward);
        }

        public async Task<bool> DeleteRewardAsync(long id)
        {
            var reward = await _rewardRepository.GetAsync(
                predicate: r => r.Id == id,
                disableTracking: false);

            if (reward == null)
                return false;

            await _rewardRepository.DeleteAsync(reward);
            return true;
        }

        public async Task<int> CountRewardsAsync(string? rewardType = null, bool? isActive = null)
        {
            if (string.IsNullOrEmpty(rewardType) && !isActive.HasValue)
                return await _rewardRepository.CountAsync();

            Expression<Func<Reward, bool>> predicate = r =>
                (string.IsNullOrEmpty(rewardType) || r.RewardType == rewardType) &&
                (!isActive.HasValue || r.IsActive == isActive.Value);

            return await _rewardRepository.CountAsync(predicate);
        }

        public async Task<bool> RewardExistsAsync(long id)
        {
            return await _rewardRepository.ExistsAsync(r => r.Id == id);
        }
    }
}