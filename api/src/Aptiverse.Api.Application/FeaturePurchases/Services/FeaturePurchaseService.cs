using Aptiverse.Api.Application.FeaturePurchases.Dtos;
using Aptiverse.Api.Domain.Models.Features;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.FeaturePurchases.Services
{
    public class FeaturePurchaseService(
        IRepository<FeaturePurchase> purchaseRepository,
        IMapper mapper) : IFeaturePurchaseService
    {
        private readonly IRepository<FeaturePurchase> _purchaseRepository = purchaseRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<FeaturePurchaseDto> CreatePurchaseAsync(CreateFeaturePurchaseDto createPurchaseDto)
        {
            ArgumentNullException.ThrowIfNull(createPurchaseDto);

            var purchase = _mapper.Map<FeaturePurchase>(createPurchaseDto);

            await _purchaseRepository.AddAsync(purchase);
            return _mapper.Map<FeaturePurchaseDto>(purchase);
        }

        public async Task<FeaturePurchaseDto?> GetPurchaseByIdAsync(long id)
        {
            var purchase = await _purchaseRepository.GetAsync(
                predicate: p => p.Id == id,
                disableTracking: false);

            return _mapper.Map<FeaturePurchaseDto>(purchase);
        }

        public async Task<PaginatedResult<FeaturePurchaseDto>> GetPurchasesAsync(
            ClaimsPrincipal currentUser,
            string? userId = null,
            long? featureId = null,
            string? paymentStatus = null,
            string? billingCycle = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? sortBy = "PurchaseDate",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<FeaturePurchase, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (!string.IsNullOrEmpty(userId) || featureId.HasValue || !string.IsNullOrEmpty(paymentStatus) ||
                !string.IsNullOrEmpty(billingCycle) || fromDate.HasValue || toDate.HasValue)
            {
                Expression<Func<FeaturePurchase, bool>> filterPredicate = p =>
                    (string.IsNullOrEmpty(userId) || p.UserId == userId) &&
                    (!featureId.HasValue || p.FeatureId == featureId.Value) &&
                    (string.IsNullOrEmpty(paymentStatus) || p.PaymentStatus == paymentStatus) &&
                    (string.IsNullOrEmpty(billingCycle) || p.BillingCycle == billingCycle) &&
                    (!fromDate.HasValue || p.PurchaseDate >= fromDate.Value) &&
                    (!toDate.HasValue || p.PurchaseDate <= toDate.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<FeaturePurchase>, IOrderedQueryable<FeaturePurchase>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _purchaseRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var purchaseDtos = _mapper.Map<List<FeaturePurchaseDto>>(paginatedResult.Data);

            return new PaginatedResult<FeaturePurchaseDto>(
                purchaseDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<FeaturePurchase, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsAdmin(user))
            {
                return null;
            }
            else
            {
                return p => p.UserId == userId;
            }
        }

        private Expression<Func<FeaturePurchase, bool>> CombinePredicates(
            Expression<Func<FeaturePurchase, bool>> left,
            Expression<Func<FeaturePurchase, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(FeaturePurchase), "p");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<FeaturePurchase, bool>>(combined, parameter);
        }

        private Func<IQueryable<FeaturePurchase>, IOrderedQueryable<FeaturePurchase>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "purchasedate" => sortDescending
                    ? query => query.OrderByDescending(p => p.PurchaseDate)
                    : query => query.OrderBy(p => p.PurchaseDate),
                "amountpaid" => sortDescending
                    ? query => query.OrderByDescending(p => p.AmountPaid)
                    : query => query.OrderBy(p => p.AmountPaid),
                "activationdate" => sortDescending
                    ? query => query.OrderByDescending(p => p.ActivationDate)
                    : query => query.OrderBy(p => p.ActivationDate),
                "expirydate" => sortDescending
                    ? query => query.OrderByDescending(p => p.ExpiryDate)
                    : query => query.OrderBy(p => p.ExpiryDate),
                _ => sortDescending
                    ? query => query.OrderByDescending(p => p.Id)
                    : query => query.OrderBy(p => p.Id)
            };
        }

        public async Task<FeaturePurchaseDto> UpdatePurchaseAsync(long id, UpdateFeaturePurchaseDto updatePurchaseDto)
        {
            var existingPurchase = await _purchaseRepository.GetAsync(
                predicate: p => p.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Feature purchase with ID {id} not found");

            _mapper.Map(updatePurchaseDto, existingPurchase);

            await _purchaseRepository.UpdateAsync(existingPurchase);
            return _mapper.Map<FeaturePurchaseDto>(existingPurchase);
        }

        public async Task<bool> DeletePurchaseAsync(long id)
        {
            var purchase = await _purchaseRepository.GetAsync(
                predicate: p => p.Id == id,
                disableTracking: false);

            if (purchase == null)
                return false;

            await _purchaseRepository.DeleteAsync(purchase);
            return true;
        }

        public async Task<int> CountPurchasesAsync(
            ClaimsPrincipal currentUser,
            string? userId = null,
            long? featureId = null,
            string? paymentStatus = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<FeaturePurchase, bool>> filterPredicate = p =>
                (string.IsNullOrEmpty(userId) || p.UserId == userId) &&
                (!featureId.HasValue || p.FeatureId == featureId.Value) &&
                (string.IsNullOrEmpty(paymentStatus) || p.PaymentStatus == paymentStatus);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _purchaseRepository.CountAsync(predicate);
        }

        public async Task<bool> PurchaseExistsAsync(long id)
        {
            return await _purchaseRepository.ExistsAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<FeaturePurchaseDto>> GetPurchasesByUserAsync(string userId)
        {
            var purchases = await _purchaseRepository.GetManyAsync(
                predicate: p => p.UserId == userId,
                orderBy: query => query.OrderByDescending(p => p.PurchaseDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<FeaturePurchaseDto>>(purchases);
        }

        public async Task<IEnumerable<FeaturePurchaseDto>> GetPurchasesByFeatureAsync(long featureId)
        {
            var purchases = await _purchaseRepository.GetManyAsync(
                predicate: p => p.FeatureId == featureId && p.PaymentStatus == "Completed",
                orderBy: query => query.OrderByDescending(p => p.PurchaseDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<FeaturePurchaseDto>>(purchases);
        }

        public async Task<IEnumerable<FeaturePurchaseDto>> GetActivePurchasesAsync(string userId)
        {
            var now = DateTime.UtcNow;
            var purchases = await _purchaseRepository.GetManyAsync(
                predicate: p => p.UserId == userId &&
                               p.PaymentStatus == "Completed" &&
                               (!p.ExpiryDate.HasValue || p.ExpiryDate > now),
                orderBy: query => query.OrderByDescending(p => p.PurchaseDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<FeaturePurchaseDto>>(purchases);
        }

        public async Task<bool> HasActivePurchaseAsync(string userId, long featureId)
        {
            var now = DateTime.UtcNow;
            return await _purchaseRepository.ExistsAsync(p =>
                p.UserId == userId &&
                p.FeatureId == featureId &&
                p.PaymentStatus == "Completed" &&
                (!p.ExpiryDate.HasValue || p.ExpiryDate > now));
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var predicate = (Expression<Func<FeaturePurchase, bool>>?)null;

            if (fromDate.HasValue || toDate.HasValue)
            {
                predicate = p =>
                    p.PaymentStatus == "Completed" &&
                    (!fromDate.HasValue || p.PurchaseDate >= fromDate.Value) &&
                    (!toDate.HasValue || p.PurchaseDate <= toDate.Value);
            }
            else
            {
                predicate = p => p.PaymentStatus == "Completed";
            }

            var purchases = await _purchaseRepository.GetManyAsync(
                predicate: predicate,
                disableTracking: true);

            return purchases.Sum(p => p.AmountPaid);
        }
    }
}