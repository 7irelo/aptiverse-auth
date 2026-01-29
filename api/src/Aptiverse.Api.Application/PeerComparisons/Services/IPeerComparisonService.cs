using Aptiverse.Api.Application.PeerComparisons.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.PeerComparisons.Services
{
    public interface IPeerComparisonService
    {
        Task<PeerComparisonDto> CreatePeerComparisonAsync(CreatePeerComparisonDto createPeerComparisonDto);
        Task<PeerComparisonDto?> GetPeerComparisonByIdAsync(long id);
        Task<PeerComparisonDto?> GetPeerComparisonByStudentSubjectIdAsync(long studentSubjectId);
        Task<PaginatedResult<PeerComparisonDto>> GetPeerComparisonsAsync(
            long? studentSubjectId = null,
            string? trendComparison = null,
            int? minPercentile = null,
            int? maxPercentile = null,
            int? minRanking = null,
            int? maxRanking = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<PeerComparisonDto> UpdatePeerComparisonAsync(long id, UpdatePeerComparisonDto updatePeerComparisonDto);
        Task<bool> DeletePeerComparisonAsync(long id);
        Task<int> CountPeerComparisonsAsync(long? studentSubjectId = null, string? trendComparison = null);
        Task<bool> PeerComparisonExistsAsync(long id);
        Task<bool> PeerComparisonExistsForStudentSubjectAsync(long studentSubjectId);
    }
}