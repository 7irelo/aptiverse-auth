using Aptiverse.Api.Application.FeaturePurchases.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.FeaturePurchases.Services
{
    public interface IFeaturePurchaseService
    {
        Task<FeaturePurchaseDto> CreatePurchaseAsync(CreateFeaturePurchaseDto createPurchaseDto);
        Task<FeaturePurchaseDto?> GetPurchaseByIdAsync(long id);

        Task<PaginatedResult<FeaturePurchaseDto>> GetPurchasesAsync(
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
            int pageSize = 20);

        Task<FeaturePurchaseDto> UpdatePurchaseAsync(long id, UpdateFeaturePurchaseDto updatePurchaseDto);
        Task<bool> DeletePurchaseAsync(long id);
        Task<int> CountPurchasesAsync(ClaimsPrincipal currentUser,
            string? userId = null,
            long? featureId = null,
            string? paymentStatus = null);
        Task<bool> PurchaseExistsAsync(long id);

        Task<IEnumerable<FeaturePurchaseDto>> GetPurchasesByUserAsync(string userId);
        Task<IEnumerable<FeaturePurchaseDto>> GetPurchasesByFeatureAsync(long featureId);
        Task<IEnumerable<FeaturePurchaseDto>> GetActivePurchasesAsync(string userId);
        Task<bool> HasActivePurchaseAsync(string userId, long featureId);
        Task<decimal> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null);
    }
}