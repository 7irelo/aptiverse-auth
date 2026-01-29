using Aptiverse.Api.Application.PredictionMetricss.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.PredictionMetricss.Services
{
    public class PredictionMetricsService(
        IRepository<PredictionMetrics> predictionMetricsRepository,
        IMapper mapper) : IPredictionMetricsService
    {
        private readonly IRepository<PredictionMetrics> _predictionMetricsRepository = predictionMetricsRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<PredictionMetricsDto> CreatePredictionMetricsAsync(CreatePredictionMetricsDto createPredictionMetricsDto)
        {
            ArgumentNullException.ThrowIfNull(createPredictionMetricsDto);

            PredictionMetrics predictionMetrics = _mapper.Map<PredictionMetrics>(createPredictionMetricsDto);
            await _predictionMetricsRepository.AddAsync(predictionMetrics);
            return _mapper.Map<PredictionMetricsDto>(predictionMetrics);
        }

        public async Task<PredictionMetricsDto?> GetPredictionMetricsByIdAsync(long id)
        {
            var predictionMetrics = await _predictionMetricsRepository.GetAsync(
                predicate: pm => pm.Id == id,
                include: query => query.Include(pm => pm.StudentSubject),
                disableTracking: false);

            if (predictionMetrics == null)
                return null;

            return _mapper.Map<PredictionMetricsDto>(predictionMetrics);
        }

        public async Task<PredictionMetricsDto?> GetPredictionMetricsByStudentSubjectIdAsync(long studentSubjectId)
        {
            var predictionMetrics = await _predictionMetricsRepository.GetAsync(
                predicate: pm => pm.StudentSubjectId == studentSubjectId,
                include: query => query.Include(pm => pm.StudentSubject),
                disableTracking: false);

            if (predictionMetrics == null)
                return null;

            return _mapper.Map<PredictionMetricsDto>(predictionMetrics);
        }

        public async Task<PaginatedResult<PredictionMetricsDto>> GetPredictionMetricsAsync(
            long? studentSubjectId = null,
            string? riskLevel = null,
            bool? interventionNeeded = null,
            int? minProbabilityA = null,
            int? minProbabilityB = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<PredictionMetrics, bool>>? predicate = BuildFilterPredicate(
                studentSubjectId, riskLevel, interventionNeeded, minProbabilityA, minProbabilityB);

            Func<IQueryable<PredictionMetrics>, IOrderedQueryable<PredictionMetrics>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _predictionMetricsRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query.Include(pm => pm.StudentSubject));

            var predictionMetricsDtos = _mapper.Map<List<PredictionMetricsDto>>(paginatedResult.Data);

            return new PaginatedResult<PredictionMetricsDto>(
                predictionMetricsDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<PredictionMetrics, bool>>? BuildFilterPredicate(
            long? studentSubjectId,
            string? riskLevel,
            bool? interventionNeeded,
            int? minProbabilityA,
            int? minProbabilityB)
        {
            if (!studentSubjectId.HasValue && string.IsNullOrEmpty(riskLevel) &&
                !interventionNeeded.HasValue && !minProbabilityA.HasValue && !minProbabilityB.HasValue)
                return null;

            return pm =>
                (!studentSubjectId.HasValue || pm.StudentSubjectId == studentSubjectId.Value) &&
                (string.IsNullOrEmpty(riskLevel) || pm.RiskLevel == riskLevel) &&
                (!interventionNeeded.HasValue || pm.InterventionNeeded == interventionNeeded.Value) &&
                (!minProbabilityA.HasValue || pm.FinalGradeProbabilityA >= minProbabilityA.Value) &&
                (!minProbabilityB.HasValue || pm.FinalGradeProbabilityB >= minProbabilityB.Value);
        }

        private Func<IQueryable<PredictionMetrics>, IOrderedQueryable<PredictionMetrics>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "risklevel" => sortDescending
                    ? query => query.OrderByDescending(pm => pm.RiskLevel).ThenByDescending(pm => pm.Id)
                    : query => query.OrderBy(pm => pm.RiskLevel).ThenBy(pm => pm.Id),
                "finalgradeprobabilitya" => sortDescending
                    ? query => query.OrderByDescending(pm => pm.FinalGradeProbabilityA).ThenByDescending(pm => pm.Id)
                    : query => query.OrderBy(pm => pm.FinalGradeProbabilityA).ThenBy(pm => pm.Id),
                "finalgradeprobabilityb" => sortDescending
                    ? query => query.OrderByDescending(pm => pm.FinalGradeProbabilityB).ThenByDescending(pm => pm.Id)
                    : query => query.OrderBy(pm => pm.FinalGradeProbabilityB).ThenBy(pm => pm.Id),
                "interventionneeded" => sortDescending
                    ? query => query.OrderByDescending(pm => pm.InterventionNeeded).ThenByDescending(pm => pm.Id)
                    : query => query.OrderBy(pm => pm.InterventionNeeded).ThenBy(pm => pm.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(pm => pm.Id)
                    : query => query.OrderBy(pm => pm.Id)
            };
        }

        public async Task<PredictionMetricsDto> UpdatePredictionMetricsAsync(long id, UpdatePredictionMetricsDto updatePredictionMetricsDto)
        {
            var existingPredictionMetrics = await _predictionMetricsRepository.GetAsync(
                predicate: pm => pm.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"PredictionMetrics with ID {id} not found");

            _mapper.Map(updatePredictionMetricsDto, existingPredictionMetrics);
            await _predictionMetricsRepository.UpdateAsync(existingPredictionMetrics);
            return _mapper.Map<PredictionMetricsDto>(existingPredictionMetrics);
        }

        public async Task<bool> DeletePredictionMetricsAsync(long id)
        {
            var predictionMetrics = await _predictionMetricsRepository.GetAsync(
                predicate: pm => pm.Id == id,
                disableTracking: false);

            if (predictionMetrics == null)
                return false;

            await _predictionMetricsRepository.DeleteAsync(predictionMetrics);
            return true;
        }

        public async Task<int> CountPredictionMetricsAsync(long? studentSubjectId = null, string? riskLevel = null, bool? interventionNeeded = null)
        {
            if (!studentSubjectId.HasValue && string.IsNullOrEmpty(riskLevel) && !interventionNeeded.HasValue)
                return await _predictionMetricsRepository.CountAsync();

            Expression<Func<PredictionMetrics, bool>> predicate = pm =>
                (!studentSubjectId.HasValue || pm.StudentSubjectId == studentSubjectId.Value) &&
                (string.IsNullOrEmpty(riskLevel) || pm.RiskLevel == riskLevel) &&
                (!interventionNeeded.HasValue || pm.InterventionNeeded == interventionNeeded.Value);

            return await _predictionMetricsRepository.CountAsync(predicate);
        }

        public async Task<bool> PredictionMetricsExistsAsync(long id)
        {
            return await _predictionMetricsRepository.ExistsAsync(pm => pm.Id == id);
        }

        public async Task<bool> PredictionMetricsExistsForStudentSubjectAsync(long studentSubjectId)
        {
            return await _predictionMetricsRepository.ExistsAsync(pm => pm.StudentSubjectId == studentSubjectId);
        }
    }
}