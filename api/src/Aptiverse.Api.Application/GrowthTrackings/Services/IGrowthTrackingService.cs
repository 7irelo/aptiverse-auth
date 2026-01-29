using Aptiverse.Api.Application.GrowthTrackings.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.GrowthTrackings.Services
{
    public interface IGrowthTrackingService
    {
        Task<GrowthTrackingDto> CreateGrowthTrackingAsync(CreateGrowthTrackingDto createGrowthTrackingDto);
        Task<GrowthTrackingDto?> GetGrowthTrackingByIdAsync(long id);

        Task<PaginatedResult<GrowthTrackingDto>> GetGrowthTrackingsAsync(
            ClaimsPrincipal currentUser,
            long? studentId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            decimal? minGrowth = null,
            decimal? maxGrowth = null,
            string? sortBy = "TrackingDate",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20);

        Task<GrowthTrackingDto> UpdateGrowthTrackingAsync(long id, UpdateGrowthTrackingDto updateGrowthTrackingDto);
        Task<bool> DeleteGrowthTrackingAsync(long id);
        Task<int> CountGrowthTrackingsAsync(ClaimsPrincipal currentUser,
            long? studentId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);
        Task<bool> GrowthTrackingExistsAsync(long id);

        Task<IEnumerable<GrowthTrackingDto>> GetGrowthTrackingsByStudentAsync(long studentId);
        Task<IEnumerable<GrowthTrackingDto>> GetGrowthTrackingsByDateRangeAsync(long studentId, DateTime startDate, DateTime endDate);
        Task<GrowthTrackingDto?> GetLatestGrowthTrackingAsync(long studentId);
        Task<GrowthTrackingDto?> GetGrowthTrackingByDateAsync(long studentId, DateTime date);
        Task<Dictionary<string, decimal>> GetGrowthTrendsAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<decimal> GetAverageOverallGrowthAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<GrowthTrackingDto>> GetRecentGrowthTrackingsAsync(long studentId, int count = 10);
    }
}