using Aptiverse.Api.Application.PredictionMetricss.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.PredictionMetricss.Services
{
    public interface IPredictionMetricsService
    {
        Task<PredictionMetricsDto> CreatePredictionMetricsAsync(CreatePredictionMetricsDto createPredictionMetricsDto);
        Task<PredictionMetricsDto?> GetPredictionMetricsByIdAsync(long id);
        Task<PredictionMetricsDto?> GetPredictionMetricsByStudentSubjectIdAsync(long studentSubjectId);
        Task<PaginatedResult<PredictionMetricsDto>> GetPredictionMetricsAsync(
            long? studentSubjectId = null,
            string? riskLevel = null,
            bool? interventionNeeded = null,
            int? minProbabilityA = null,
            int? minProbabilityB = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<PredictionMetricsDto> UpdatePredictionMetricsAsync(long id, UpdatePredictionMetricsDto updatePredictionMetricsDto);
        Task<bool> DeletePredictionMetricsAsync(long id);
        Task<int> CountPredictionMetricsAsync(long? studentSubjectId = null, string? riskLevel = null, bool? interventionNeeded = null);
        Task<bool> PredictionMetricsExistsAsync(long id);
        Task<bool> PredictionMetricsExistsForStudentSubjectAsync(long studentSubjectId);
    }
}