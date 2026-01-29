using Aptiverse.Api.Application.PeerComparisons.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.PeerComparisons.Services
{
    public class PeerComparisonService(
        IRepository<PeerComparison> peerComparisonRepository,
        IMapper mapper) : IPeerComparisonService
    {
        private readonly IRepository<PeerComparison> _peerComparisonRepository = peerComparisonRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<PeerComparisonDto> CreatePeerComparisonAsync(CreatePeerComparisonDto createPeerComparisonDto)
        {
            ArgumentNullException.ThrowIfNull(createPeerComparisonDto);

            PeerComparison peerComparison = _mapper.Map<PeerComparison>(createPeerComparisonDto);
            await _peerComparisonRepository.AddAsync(peerComparison);
            return _mapper.Map<PeerComparisonDto>(peerComparison);
        }

        public async Task<PeerComparisonDto?> GetPeerComparisonByIdAsync(long id)
        {
            var peerComparison = await _peerComparisonRepository.GetAsync(
                predicate: pc => pc.Id == id,
                include: query => query.Include(pc => pc.StudentSubject),
                disableTracking: false);

            if (peerComparison == null)
                return null;

            return _mapper.Map<PeerComparisonDto>(peerComparison);
        }

        public async Task<PeerComparisonDto?> GetPeerComparisonByStudentSubjectIdAsync(long studentSubjectId)
        {
            var peerComparison = await _peerComparisonRepository.GetAsync(
                predicate: pc => pc.StudentSubjectId == studentSubjectId,
                include: query => query.Include(pc => pc.StudentSubject),
                disableTracking: false);

            if (peerComparison == null)
                return null;

            return _mapper.Map<PeerComparisonDto>(peerComparison);
        }

        public async Task<PaginatedResult<PeerComparisonDto>> GetPeerComparisonsAsync(
            long? studentSubjectId = null,
            string? trendComparison = null,
            int? minPercentile = null,
            int? maxPercentile = null,
            int? minRanking = null,
            int? maxRanking = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<PeerComparison, bool>>? predicate = BuildFilterPredicate(
                studentSubjectId, trendComparison, minPercentile, maxPercentile, minRanking, maxRanking);

            Func<IQueryable<PeerComparison>, IOrderedQueryable<PeerComparison>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _peerComparisonRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query.Include(pc => pc.StudentSubject));

            var peerComparisonDtos = _mapper.Map<List<PeerComparisonDto>>(paginatedResult.Data);

            return new PaginatedResult<PeerComparisonDto>(
                peerComparisonDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<PeerComparison, bool>>? BuildFilterPredicate(
            long? studentSubjectId,
            string? trendComparison,
            int? minPercentile,
            int? maxPercentile,
            int? minRanking,
            int? maxRanking)
        {
            if (!studentSubjectId.HasValue && string.IsNullOrEmpty(trendComparison) &&
                !minPercentile.HasValue && !maxPercentile.HasValue &&
                !minRanking.HasValue && !maxRanking.HasValue)
                return null;

            return pc =>
                (!studentSubjectId.HasValue || pc.StudentSubjectId == studentSubjectId.Value) &&
                (string.IsNullOrEmpty(trendComparison) || pc.TrendComparison == trendComparison) &&
                (!minPercentile.HasValue || pc.Percentile >= minPercentile.Value) &&
                (!maxPercentile.HasValue || pc.Percentile <= maxPercentile.Value) &&
                (!minRanking.HasValue || pc.Ranking >= minRanking.Value) &&
                (!maxRanking.HasValue || pc.Ranking <= maxRanking.Value);
        }

        private Func<IQueryable<PeerComparison>, IOrderedQueryable<PeerComparison>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "percentile" => sortDescending
                    ? query => query.OrderByDescending(pc => pc.Percentile).ThenByDescending(pc => pc.Id)
                    : query => query.OrderBy(pc => pc.Percentile).ThenBy(pc => pc.Id),
                "ranking" => sortDescending
                    ? query => query.OrderByDescending(pc => pc.Ranking).ThenByDescending(pc => pc.Id)
                    : query => query.OrderBy(pc => pc.Ranking).ThenBy(pc => pc.Id),
                "classaverage" => sortDescending
                    ? query => query.OrderByDescending(pc => pc.ClassAverage).ThenByDescending(pc => pc.Id)
                    : query => query.OrderBy(pc => pc.ClassAverage).ThenBy(pc => pc.Id),
                "trendcomparison" => sortDescending
                    ? query => query.OrderByDescending(pc => pc.TrendComparison).ThenByDescending(pc => pc.Id)
                    : query => query.OrderBy(pc => pc.TrendComparison).ThenBy(pc => pc.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(pc => pc.Id)
                    : query => query.OrderBy(pc => pc.Id)
            };
        }

        public async Task<PeerComparisonDto> UpdatePeerComparisonAsync(long id, UpdatePeerComparisonDto updatePeerComparisonDto)
        {
            var existingPeerComparison = await _peerComparisonRepository.GetAsync(
                predicate: pc => pc.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"PeerComparison with ID {id} not found");

            _mapper.Map(updatePeerComparisonDto, existingPeerComparison);
            await _peerComparisonRepository.UpdateAsync(existingPeerComparison);
            return _mapper.Map<PeerComparisonDto>(existingPeerComparison);
        }

        public async Task<bool> DeletePeerComparisonAsync(long id)
        {
            var peerComparison = await _peerComparisonRepository.GetAsync(
                predicate: pc => pc.Id == id,
                disableTracking: false);

            if (peerComparison == null)
                return false;

            await _peerComparisonRepository.DeleteAsync(peerComparison);
            return true;
        }

        public async Task<int> CountPeerComparisonsAsync(long? studentSubjectId = null, string? trendComparison = null)
        {
            if (!studentSubjectId.HasValue && string.IsNullOrEmpty(trendComparison))
                return await _peerComparisonRepository.CountAsync();

            Expression<Func<PeerComparison, bool>> predicate = pc =>
                (!studentSubjectId.HasValue || pc.StudentSubjectId == studentSubjectId.Value) &&
                (string.IsNullOrEmpty(trendComparison) || pc.TrendComparison == trendComparison);

            return await _peerComparisonRepository.CountAsync(predicate);
        }

        public async Task<bool> PeerComparisonExistsAsync(long id)
        {
            return await _peerComparisonRepository.ExistsAsync(pc => pc.Id == id);
        }

        public async Task<bool> PeerComparisonExistsForStudentSubjectAsync(long studentSubjectId)
        {
            return await _peerComparisonRepository.ExistsAsync(pc => pc.StudentSubjectId == studentSubjectId);
        }
    }
}