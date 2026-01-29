using Aptiverse.Api.Application.UserFeatures.Dtos;
using Aptiverse.Api.Domain.Models.Features;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.UserFeatures.Services
{
    public class UserFeatureService(
        IRepository<UserFeature> userFeatureRepository,
        IMapper mapper) : IUserFeatureService
    {
        private readonly IRepository<UserFeature> _userFeatureRepository = userFeatureRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<UserFeatureDto> CreateUserFeatureAsync(CreateUserFeatureDto createUserFeatureDto)
        {
            ArgumentNullException.ThrowIfNull(createUserFeatureDto);

            var userFeature = _mapper.Map<UserFeature>(createUserFeatureDto);

            await _userFeatureRepository.AddAsync(userFeature);
            return _mapper.Map<UserFeatureDto>(userFeature);
        }

        public async Task<UserFeatureDto?> GetUserFeatureByIdAsync(long id)
        {
            var userFeature = await _userFeatureRepository.GetAsync(
                predicate: uf => uf.Id == id,
                disableTracking: false);

            return _mapper.Map<UserFeatureDto>(userFeature);
        }

        public async Task<PaginatedResult<UserFeatureDto>> GetUserFeaturesAsync(
            ClaimsPrincipal currentUser,
            string? userId = null,
            long? featureId = null,
            string? grantType = null,
            bool? isActive = null,
            string? sortBy = "GrantedAt",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<UserFeature, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (!string.IsNullOrEmpty(userId) || featureId.HasValue || !string.IsNullOrEmpty(grantType) || isActive.HasValue)
            {
                Expression<Func<UserFeature, bool>> filterPredicate = uf =>
                    (string.IsNullOrEmpty(userId) || uf.UserId == userId) &&
                    (!featureId.HasValue || uf.FeatureId == featureId.Value) &&
                    (string.IsNullOrEmpty(grantType) || uf.GrantType == grantType) &&
                    (!isActive.HasValue || uf.IsActive == isActive.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<UserFeature>, IOrderedQueryable<UserFeature>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _userFeatureRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var userFeatureDtos = _mapper.Map<List<UserFeatureDto>>(paginatedResult.Data);

            return new PaginatedResult<UserFeatureDto>(
                userFeatureDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<UserFeature, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user) || UserContextHelper.IsAdmin(user))
            {
                return null;
            }

            return uf => uf.UserId == userId;
        }

        private Expression<Func<UserFeature, bool>> CombinePredicates(
            Expression<Func<UserFeature, bool>> left,
            Expression<Func<UserFeature, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(UserFeature), "uf");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<UserFeature, bool>>(combined, parameter);
        }

        private Func<IQueryable<UserFeature>, IOrderedQueryable<UserFeature>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "grantedat" => sortDescending
                    ? query => query.OrderByDescending(uf => uf.GrantedAt)
                    : query => query.OrderBy(uf => uf.GrantedAt),
                "expiresat" => sortDescending
                    ? query => query.OrderByDescending(uf => uf.ExpiresAt)
                    : query => query.OrderBy(uf => uf.ExpiresAt),
                "granttype" => sortDescending
                    ? query => query.OrderByDescending(uf => uf.GrantType)
                    : query => query.OrderBy(uf => uf.GrantType),
                _ => sortDescending
                    ? query => query.OrderByDescending(uf => uf.Id)
                    : query => query.OrderBy(uf => uf.Id)
            };
        }

        public async Task<UserFeatureDto> UpdateUserFeatureAsync(long id, UpdateUserFeatureDto updateUserFeatureDto)
        {
            var existingUserFeature = await _userFeatureRepository.GetAsync(
                predicate: uf => uf.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"User feature with ID {id} not found");

            _mapper.Map(updateUserFeatureDto, existingUserFeature);

            await _userFeatureRepository.UpdateAsync(existingUserFeature);
            return _mapper.Map<UserFeatureDto>(existingUserFeature);
        }

        public async Task<bool> DeleteUserFeatureAsync(long id)
        {
            var userFeature = await _userFeatureRepository.GetAsync(
                predicate: uf => uf.Id == id,
                disableTracking: false);

            if (userFeature == null)
                return false;

            await _userFeatureRepository.DeleteAsync(userFeature);
            return true;
        }

        public async Task<int> CountUserFeaturesAsync(
            ClaimsPrincipal currentUser,
            string? userId = null,
            long? featureId = null,
            string? grantType = null,
            bool? isActive = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<UserFeature, bool>> filterPredicate = uf =>
                (string.IsNullOrEmpty(userId) || uf.UserId == userId) &&
                (!featureId.HasValue || uf.FeatureId == featureId.Value) &&
                (string.IsNullOrEmpty(grantType) || uf.GrantType == grantType) &&
                (!isActive.HasValue || uf.IsActive == isActive.Value);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _userFeatureRepository.CountAsync(predicate);
        }

        public async Task<bool> UserFeatureExistsAsync(long id)
        {
            return await _userFeatureRepository.ExistsAsync(uf => uf.Id == id);
        }

        public async Task<IEnumerable<UserFeatureDto>> GetUserFeaturesByUserAsync(string userId)
        {
            var userFeatures = await _userFeatureRepository.GetManyAsync(
                predicate: uf => uf.UserId == userId,
                orderBy: query => query.OrderByDescending(uf => uf.GrantedAt),
                disableTracking: true);

            return _mapper.Map<IEnumerable<UserFeatureDto>>(userFeatures);
        }

        public async Task<IEnumerable<UserFeatureDto>> GetUserFeaturesByFeatureAsync(long featureId)
        {
            var userFeatures = await _userFeatureRepository.GetManyAsync(
                predicate: uf => uf.FeatureId == featureId,
                orderBy: query => query.OrderByDescending(uf => uf.GrantedAt),
                disableTracking: true);

            return _mapper.Map<IEnumerable<UserFeatureDto>>(userFeatures);
        }

        public async Task<IEnumerable<UserFeatureDto>> GetActiveUserFeaturesAsync(string userId)
        {
            var now = DateTime.UtcNow;
            var userFeatures = await _userFeatureRepository.GetManyAsync(
                predicate: uf => uf.UserId == userId &&
                               uf.IsActive &&
                               (!uf.ExpiresAt.HasValue || uf.ExpiresAt > now),
                orderBy: query => query.OrderByDescending(uf => uf.GrantedAt),
                disableTracking: true);

            return _mapper.Map<IEnumerable<UserFeatureDto>>(userFeatures);
        }

        public async Task<bool> HasActiveFeatureAsync(string userId, long featureId)
        {
            var now = DateTime.UtcNow;
            return await _userFeatureRepository.ExistsAsync(uf =>
                uf.UserId == userId &&
                uf.FeatureId == featureId &&
                uf.IsActive &&
                (!uf.ExpiresAt.HasValue || uf.ExpiresAt > now));
        }

        public async Task<bool> HasAnyActiveFeatureAsync(string userId, List<long> featureIds)
        {
            if (featureIds == null || !featureIds.Any())
                return false;

            var now = DateTime.UtcNow;
            return await _userFeatureRepository.ExistsAsync(uf =>
                uf.UserId == userId &&
                featureIds.Contains(uf.FeatureId) &&
                uf.IsActive &&
                (!uf.ExpiresAt.HasValue || uf.ExpiresAt > now));
        }

        public async Task<bool> ExistsAsync(string userId, long featureId)
        {
            return await _userFeatureRepository.ExistsAsync(uf =>
                uf.UserId == userId && uf.FeatureId == featureId);
        }
    }
}