using Aptiverse.Api.Application.RewardFeatures.Dtos;
using Aptiverse.Api.Domain.Models.Features;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.RewardFeatures.Services
{
    public class RewardFeatureService(
        IRepository<RewardFeature> rewardFeatureRepository,
        IMapper mapper) : IRewardFeatureService
    {
        private readonly IRepository<RewardFeature> _rewardFeatureRepository = rewardFeatureRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<RewardFeatureDto> CreateRewardFeatureAsync(CreateRewardFeatureDto createRewardFeatureDto)
        {
            ArgumentNullException.ThrowIfNull(createRewardFeatureDto);

            var rewardFeature = _mapper.Map<RewardFeature>(createRewardFeatureDto);

            await _rewardFeatureRepository.AddAsync(rewardFeature);
            return _mapper.Map<RewardFeatureDto>(rewardFeature);
        }

        public async Task<RewardFeatureDto?> GetRewardFeatureByIdAsync(long id)
        {
            var rewardFeature = await _rewardFeatureRepository.GetAsync(
                predicate: rf => rf.Id == id,
                disableTracking: false);

            return _mapper.Map<RewardFeatureDto>(rewardFeature);
        }

        public async Task<PaginatedResult<RewardFeatureDto>> GetRewardFeaturesAsync(
            ClaimsPrincipal currentUser,
            long? rewardId = null,
            long? featureId = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<RewardFeature, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (rewardId.HasValue || featureId.HasValue)
            {
                Expression<Func<RewardFeature, bool>> filterPredicate = rf =>
                    (!rewardId.HasValue || rf.RewardId == rewardId.Value) &&
                    (!featureId.HasValue || rf.FeatureId == featureId.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<RewardFeature>, IOrderedQueryable<RewardFeature>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _rewardFeatureRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var rewardFeatureDtos = _mapper.Map<List<RewardFeatureDto>>(paginatedResult.Data);

            return new PaginatedResult<RewardFeatureDto>(
                rewardFeatureDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<RewardFeature, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }

            return null;
        }

        private Expression<Func<RewardFeature, bool>> CombinePredicates(
            Expression<Func<RewardFeature, bool>> left,
            Expression<Func<RewardFeature, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(RewardFeature), "rf");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<RewardFeature, bool>>(combined, parameter);
        }

        private Func<IQueryable<RewardFeature>, IOrderedQueryable<RewardFeature>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "durationdays" => sortDescending
                    ? query => query.OrderByDescending(rf => rf.DurationDays)
                    : query => query.OrderBy(rf => rf.DurationDays),
                "featureweight" => sortDescending
                    ? query => query.OrderByDescending(rf => rf.FeatureWeight)
                    : query => query.OrderBy(rf => rf.FeatureWeight),
                _ => sortDescending
                    ? query => query.OrderByDescending(rf => rf.Id)
                    : query => query.OrderBy(rf => rf.Id)
            };
        }

        public async Task<RewardFeatureDto> UpdateRewardFeatureAsync(long id, UpdateRewardFeatureDto updateRewardFeatureDto)
        {
            var existingRewardFeature = await _rewardFeatureRepository.GetAsync(
                predicate: rf => rf.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Reward feature with ID {id} not found");

            _mapper.Map(updateRewardFeatureDto, existingRewardFeature);

            await _rewardFeatureRepository.UpdateAsync(existingRewardFeature);
            return _mapper.Map<RewardFeatureDto>(existingRewardFeature);
        }

        public async Task<bool> DeleteRewardFeatureAsync(long id)
        {
            var rewardFeature = await _rewardFeatureRepository.GetAsync(
                predicate: rf => rf.Id == id,
                disableTracking: false);

            if (rewardFeature == null)
                return false;

            await _rewardFeatureRepository.DeleteAsync(rewardFeature);
            return true;
        }

        public async Task<int> CountRewardFeaturesAsync(
            ClaimsPrincipal currentUser,
            long? rewardId = null,
            long? featureId = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<RewardFeature, bool>> filterPredicate = rf =>
                (!rewardId.HasValue || rf.RewardId == rewardId.Value) &&
                (!featureId.HasValue || rf.FeatureId == featureId.Value);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _rewardFeatureRepository.CountAsync(predicate);
        }

        public async Task<bool> RewardFeatureExistsAsync(long id)
        {
            return await _rewardFeatureRepository.ExistsAsync(rf => rf.Id == id);
        }

        public async Task<IEnumerable<RewardFeatureDto>> GetRewardFeaturesByRewardAsync(long rewardId)
        {
            var rewardFeatures = await _rewardFeatureRepository.GetManyAsync(
                predicate: rf => rf.RewardId == rewardId,
                orderBy: query => query.OrderByDescending(rf => rf.FeatureWeight),
                disableTracking: true);

            return _mapper.Map<IEnumerable<RewardFeatureDto>>(rewardFeatures);
        }

        public async Task<IEnumerable<RewardFeatureDto>> GetRewardFeaturesByFeatureAsync(long featureId)
        {
            var rewardFeatures = await _rewardFeatureRepository.GetManyAsync(
                predicate: rf => rf.FeatureId == featureId,
                orderBy: query => query.OrderByDescending(rf => rf.FeatureWeight),
                disableTracking: true);

            return _mapper.Map<IEnumerable<RewardFeatureDto>>(rewardFeatures);
        }

        public async Task<bool> ExistsAsync(long rewardId, long featureId)
        {
            return await _rewardFeatureRepository.ExistsAsync(rf =>
                rf.RewardId == rewardId && rf.FeatureId == featureId);
        }
    }
}