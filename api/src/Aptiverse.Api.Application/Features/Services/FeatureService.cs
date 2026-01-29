using Aptiverse.Api.Application.Features.Dtos;
using Aptiverse.Api.Domain.Models.Features;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.Features.Services
{
    public class FeatureService(
        IRepository<Feature> featureRepository,
        IMapper mapper) : IFeatureService
    {
        private readonly IRepository<Feature> _featureRepository = featureRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<FeatureDto> CreateFeatureAsync(CreateFeatureDto createFeatureDto)
        {
            ArgumentNullException.ThrowIfNull(createFeatureDto);

            var feature = _mapper.Map<Feature>(createFeatureDto);

            await _featureRepository.AddAsync(feature);
            return _mapper.Map<FeatureDto>(feature);
        }

        public async Task<FeatureDto?> GetFeatureByIdAsync(long id)
        {
            var feature = await _featureRepository.GetAsync(
                predicate: f => f.Id == id,
                disableTracking: false);

            return _mapper.Map<FeatureDto>(feature);
        }

        public async Task<FeatureDto?> GetFeatureByCodeAsync(string code)
        {
            var feature = await _featureRepository.GetAsync(
                predicate: f => f.Code == code,
                disableTracking: false);

            return _mapper.Map<FeatureDto>(feature);
        }

        public async Task<PaginatedResult<FeatureDto>> GetFeaturesAsync(
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
            int pageSize = 20)
        {
            Expression<Func<Feature, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (!string.IsNullOrEmpty(search) || !string.IsNullOrEmpty(category) ||
                !string.IsNullOrEmpty(billingCycle) || isActive.HasValue ||
                minPrice.HasValue || maxPrice.HasValue)
            {
                Expression<Func<Feature, bool>> filterPredicate = f =>
                    (string.IsNullOrEmpty(search) ||
                     f.Name.Contains(search) ||
                     f.Description.Contains(search) ||
                     f.Code.Contains(search)) &&
                    (string.IsNullOrEmpty(category) || f.Category == category) &&
                    (string.IsNullOrEmpty(billingCycle) || f.BillingCycle == billingCycle) &&
                    (!isActive.HasValue || f.IsActive == isActive.Value) &&
                    (!minPrice.HasValue || f.BasePrice >= minPrice.Value) &&
                    (!maxPrice.HasValue || f.BasePrice <= maxPrice.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<Feature>, IOrderedQueryable<Feature>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _featureRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var featureDtos = _mapper.Map<List<FeatureDto>>(paginatedResult.Data);

            return new PaginatedResult<FeatureDto>(
                featureDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<Feature, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            if (UserContextHelper.IsSuperUser(user) || UserContextHelper.IsAdmin(user))
            {
                return null;
            }

            return f => f.IsActive;
        }

        private Expression<Func<Feature, bool>> CombinePredicates(
            Expression<Func<Feature, bool>> left,
            Expression<Func<Feature, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(Feature), "f");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<Feature, bool>>(combined, parameter);
        }

        private Func<IQueryable<Feature>, IOrderedQueryable<Feature>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "name" => sortDescending
                    ? query => query.OrderByDescending(f => f.Name)
                    : query => query.OrderBy(f => f.Name),
                "code" => sortDescending
                    ? query => query.OrderByDescending(f => f.Code)
                    : query => query.OrderBy(f => f.Code),
                "category" => sortDescending
                    ? query => query.OrderByDescending(f => f.Category)
                    : query => query.OrderBy(f => f.Category),
                "baseprice" => sortDescending
                    ? query => query.OrderByDescending(f => f.BasePrice)
                    : query => query.OrderBy(f => f.BasePrice),
                "complexityweight" => sortDescending
                    ? query => query.OrderByDescending(f => f.ComplexityWeight)
                    : query => query.OrderBy(f => f.ComplexityWeight),
                "createdat" => sortDescending
                    ? query => query.OrderByDescending(f => f.CreatedAt)
                    : query => query.OrderBy(f => f.CreatedAt),
                _ => sortDescending
                    ? query => query.OrderByDescending(f => f.Id)
                    : query => query.OrderBy(f => f.Id)
            };
        }

        public async Task<FeatureDto> UpdateFeatureAsync(long id, UpdateFeatureDto updateFeatureDto)
        {
            var existingFeature = await _featureRepository.GetAsync(
                predicate: f => f.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Feature with ID {id} not found");

            _mapper.Map(updateFeatureDto, existingFeature);

            await _featureRepository.UpdateAsync(existingFeature);
            return _mapper.Map<FeatureDto>(existingFeature);
        }

        public async Task<bool> DeleteFeatureAsync(long id)
        {
            var feature = await _featureRepository.GetAsync(
                predicate: f => f.Id == id,
                disableTracking: false);

            if (feature == null)
                return false;

            await _featureRepository.DeleteAsync(feature);
            return true;
        }

        public async Task<int> CountFeaturesAsync(
            ClaimsPrincipal currentUser,
            string? category = null,
            string? billingCycle = null,
            bool? isActive = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<Feature, bool>> filterPredicate = f =>
                (string.IsNullOrEmpty(category) || f.Category == category) &&
                (string.IsNullOrEmpty(billingCycle) || f.BillingCycle == billingCycle) &&
                (!isActive.HasValue || f.IsActive == isActive.Value);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _featureRepository.CountAsync(predicate);
        }

        public async Task<bool> FeatureExistsAsync(long id)
        {
            return await _featureRepository.ExistsAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<FeatureDto>> GetActiveFeaturesAsync()
        {
            var features = await _featureRepository.GetManyAsync(
                predicate: f => f.IsActive,
                orderBy: query => query.OrderBy(f => f.Category).ThenBy(f => f.Name),
                disableTracking: true);

            return _mapper.Map<IEnumerable<FeatureDto>>(features);
        }

        public async Task<IEnumerable<FeatureDto>> GetFeaturesByCategoryAsync(string category)
        {
            var features = await _featureRepository.GetManyAsync(
                predicate: f => f.Category == category && f.IsActive,
                orderBy: query => query.OrderBy(f => f.Name),
                disableTracking: true);

            return _mapper.Map<IEnumerable<FeatureDto>>(features);
        }

        public async Task<IEnumerable<FeatureDto>> GetFeaturesForUserAsync(string userId)
        {
            var features = await _featureRepository.GetManyAsync(
                predicate: f => f.IsActive,
                orderBy: query => query.OrderBy(f => f.Category).ThenBy(f => f.Name),
                disableTracking: true);

            return _mapper.Map<IEnumerable<FeatureDto>>(features);
        }
    }
}