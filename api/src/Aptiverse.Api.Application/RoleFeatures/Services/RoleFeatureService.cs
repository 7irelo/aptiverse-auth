using Aptiverse.Api.Application.RoleFeatures.Dtos;
using Aptiverse.Api.Domain.Models.Features;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.RoleFeatures.Services
{
    public class RoleFeatureService(
        IRepository<RoleFeature> roleFeatureRepository,
        IMapper mapper) : IRoleFeatureService
    {
        private readonly IRepository<RoleFeature> _roleFeatureRepository = roleFeatureRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<RoleFeatureDto> CreateRoleFeatureAsync(CreateRoleFeatureDto createRoleFeatureDto)
        {
            ArgumentNullException.ThrowIfNull(createRoleFeatureDto);

            var roleFeature = _mapper.Map<RoleFeature>(createRoleFeatureDto);

            await _roleFeatureRepository.AddAsync(roleFeature);
            return _mapper.Map<RoleFeatureDto>(roleFeature);
        }

        public async Task<RoleFeatureDto?> GetRoleFeatureByIdAsync(long id)
        {
            var roleFeature = await _roleFeatureRepository.GetAsync(
                predicate: rf => rf.Id == id,
                disableTracking: false);

            return _mapper.Map<RoleFeatureDto>(roleFeature);
        }

        public async Task<PaginatedResult<RoleFeatureDto>> GetRoleFeaturesAsync(
            ClaimsPrincipal currentUser,
            string? roleName = null,
            long? featureId = null,
            bool? isDefault = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<RoleFeature, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (!string.IsNullOrEmpty(roleName) || featureId.HasValue || isDefault.HasValue)
            {
                Expression<Func<RoleFeature, bool>> filterPredicate = rf =>
                    (string.IsNullOrEmpty(roleName) || rf.RoleName == roleName) &&
                    (!featureId.HasValue || rf.FeatureId == featureId.Value) &&
                    (!isDefault.HasValue || rf.IsDefault == isDefault.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<RoleFeature>, IOrderedQueryable<RoleFeature>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _roleFeatureRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var roleFeatureDtos = _mapper.Map<List<RoleFeatureDto>>(paginatedResult.Data);

            return new PaginatedResult<RoleFeatureDto>(
                roleFeatureDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<RoleFeature, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            if (UserContextHelper.IsSuperUser(user) || UserContextHelper.IsAdmin(user))
            {
                return null;
            }

            return null;
        }

        private Expression<Func<RoleFeature, bool>> CombinePredicates(
            Expression<Func<RoleFeature, bool>> left,
            Expression<Func<RoleFeature, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(RoleFeature), "rf");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<RoleFeature, bool>>(combined, parameter);
        }

        private Func<IQueryable<RoleFeature>, IOrderedQueryable<RoleFeature>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "rolename" => sortDescending
                    ? query => query.OrderByDescending(rf => rf.RoleName)
                    : query => query.OrderBy(rf => rf.RoleName),
                "assignedat" => sortDescending
                    ? query => query.OrderByDescending(rf => rf.AssignedAt)
                    : query => query.OrderBy(rf => rf.AssignedAt),
                _ => sortDescending
                    ? query => query.OrderByDescending(rf => rf.Id)
                    : query => query.OrderBy(rf => rf.Id)
            };
        }

        public async Task<RoleFeatureDto> UpdateRoleFeatureAsync(long id, UpdateRoleFeatureDto updateRoleFeatureDto)
        {
            var existingRoleFeature = await _roleFeatureRepository.GetAsync(
                predicate: rf => rf.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Role feature with ID {id} not found");

            _mapper.Map(updateRoleFeatureDto, existingRoleFeature);

            await _roleFeatureRepository.UpdateAsync(existingRoleFeature);
            return _mapper.Map<RoleFeatureDto>(existingRoleFeature);
        }

        public async Task<bool> DeleteRoleFeatureAsync(long id)
        {
            var roleFeature = await _roleFeatureRepository.GetAsync(
                predicate: rf => rf.Id == id,
                disableTracking: false);

            if (roleFeature == null)
                return false;

            await _roleFeatureRepository.DeleteAsync(roleFeature);
            return true;
        }

        public async Task<int> CountRoleFeaturesAsync(
            ClaimsPrincipal currentUser,
            string? roleName = null,
            long? featureId = null,
            bool? isDefault = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<RoleFeature, bool>> filterPredicate = rf =>
                (string.IsNullOrEmpty(roleName) || rf.RoleName == roleName) &&
                (!featureId.HasValue || rf.FeatureId == featureId.Value) &&
                (!isDefault.HasValue || rf.IsDefault == isDefault.Value);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _roleFeatureRepository.CountAsync(predicate);
        }

        public async Task<bool> RoleFeatureExistsAsync(long id)
        {
            return await _roleFeatureRepository.ExistsAsync(rf => rf.Id == id);
        }

        public async Task<IEnumerable<RoleFeatureDto>> GetRoleFeaturesByRoleAsync(string roleName)
        {
            var roleFeatures = await _roleFeatureRepository.GetManyAsync(
                predicate: rf => rf.RoleName == roleName,
                orderBy: query => query.OrderBy(rf => rf.AssignedAt),
                disableTracking: true);

            return _mapper.Map<IEnumerable<RoleFeatureDto>>(roleFeatures);
        }

        public async Task<IEnumerable<RoleFeatureDto>> GetRoleFeaturesByFeatureAsync(long featureId)
        {
            var roleFeatures = await _roleFeatureRepository.GetManyAsync(
                predicate: rf => rf.FeatureId == featureId,
                orderBy: query => query.OrderBy(rf => rf.RoleName),
                disableTracking: true);

            return _mapper.Map<IEnumerable<RoleFeatureDto>>(roleFeatures);
        }

        public async Task<IEnumerable<RoleFeatureDto>> GetDefaultFeaturesForRoleAsync(string roleName)
        {
            var roleFeatures = await _roleFeatureRepository.GetManyAsync(
                predicate: rf => rf.RoleName == roleName && rf.IsDefault,
                orderBy: query => query.OrderBy(rf => rf.AssignedAt),
                disableTracking: true);

            return _mapper.Map<IEnumerable<RoleFeatureDto>>(roleFeatures);
        }

        public async Task<bool> HasFeatureAccessAsync(string roleName, long featureId)
        {
            return await _roleFeatureRepository.ExistsAsync(rf =>
                rf.RoleName == roleName && rf.FeatureId == featureId);
        }

        public async Task<bool> ExistsAsync(string roleName, long featureId)
        {
            return await _roleFeatureRepository.ExistsAsync(rf =>
                rf.RoleName == roleName && rf.FeatureId == featureId);
        }
    }
}