using Aptiverse.Api.Application.KnowledgeGaps.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.KnowledgeGaps.Services
{
    public class KnowledgeGapService(
        IRepository<KnowledgeGap> knowledgeGapRepository,
        IMapper mapper) : IKnowledgeGapService
    {
        private readonly IRepository<KnowledgeGap> _knowledgeGapRepository = knowledgeGapRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<KnowledgeGapDto> CreateKnowledgeGapAsync(CreateKnowledgeGapDto createKnowledgeGapDto)
        {
            ArgumentNullException.ThrowIfNull(createKnowledgeGapDto);

            KnowledgeGap knowledgeGap = _mapper.Map<KnowledgeGap>(createKnowledgeGapDto);
            await _knowledgeGapRepository.AddAsync(knowledgeGap);
            return _mapper.Map<KnowledgeGapDto>(knowledgeGap);
        }

        public async Task<KnowledgeGapDto?> GetKnowledgeGapByIdAsync(long id)
        {
            var knowledgeGap = await _knowledgeGapRepository.GetAsync(
                predicate: kg => kg.Id == id,
                include: query => query.Include(kg => kg.StudentSubject),
                disableTracking: false);

            if (knowledgeGap == null)
                return null;

            return _mapper.Map<KnowledgeGapDto>(knowledgeGap);
        }

        public async Task<PaginatedResult<KnowledgeGapDto>> GetKnowledgeGapsAsync(
            long? studentSubjectId = null,
            string? severity = null,
            string? search = null,
            DateTime? lastTestedAfter = null,
            DateTime? lastTestedBefore = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<KnowledgeGap, bool>>? predicate = BuildFilterPredicate(
                studentSubjectId, severity, search, lastTestedAfter, lastTestedBefore);

            Func<IQueryable<KnowledgeGap>, IOrderedQueryable<KnowledgeGap>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _knowledgeGapRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query.Include(kg => kg.StudentSubject));

            var knowledgeGapDtos = _mapper.Map<List<KnowledgeGapDto>>(paginatedResult.Data);

            return new PaginatedResult<KnowledgeGapDto>(
                knowledgeGapDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<KnowledgeGap, bool>>? BuildFilterPredicate(
            long? studentSubjectId,
            string? severity,
            string? search,
            DateTime? lastTestedAfter,
            DateTime? lastTestedBefore)
        {
            if (!studentSubjectId.HasValue && string.IsNullOrEmpty(severity) &&
                string.IsNullOrEmpty(search) && !lastTestedAfter.HasValue && !lastTestedBefore.HasValue)
                return null;

            return kg =>
                (!studentSubjectId.HasValue || kg.StudentSubjectId == studentSubjectId.Value) &&
                (string.IsNullOrEmpty(severity) || kg.Severity == severity) &&
                (string.IsNullOrEmpty(search) || kg.Concept.Contains(search)) &&
                (!lastTestedAfter.HasValue || kg.LastTested >= lastTestedAfter.Value) &&
                (!lastTestedBefore.HasValue || kg.LastTested <= lastTestedBefore.Value);
        }

        private Func<IQueryable<KnowledgeGap>, IOrderedQueryable<KnowledgeGap>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "severity" => sortDescending
                    ? query => query.OrderByDescending(kg => kg.Severity).ThenByDescending(kg => kg.Id)
                    : query => query.OrderBy(kg => kg.Severity).ThenBy(kg => kg.Id),
                "concept" => sortDescending
                    ? query => query.OrderByDescending(kg => kg.Concept).ThenByDescending(kg => kg.Id)
                    : query => query.OrderBy(kg => kg.Concept).ThenBy(kg => kg.Id),
                "lasttested" => sortDescending
                    ? query => query.OrderByDescending(kg => kg.LastTested).ThenByDescending(kg => kg.Id)
                    : query => query.OrderBy(kg => kg.LastTested).ThenBy(kg => kg.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(kg => kg.Id)
                    : query => query.OrderBy(kg => kg.Id)
            };
        }

        public async Task<KnowledgeGapDto> UpdateKnowledgeGapAsync(long id, UpdateKnowledgeGapDto updateKnowledgeGapDto)
        {
            var existingKnowledgeGap = await _knowledgeGapRepository.GetAsync(
                predicate: kg => kg.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"KnowledgeGap with ID {id} not found");

            _mapper.Map(updateKnowledgeGapDto, existingKnowledgeGap);
            await _knowledgeGapRepository.UpdateAsync(existingKnowledgeGap);
            return _mapper.Map<KnowledgeGapDto>(existingKnowledgeGap);
        }

        public async Task<bool> DeleteKnowledgeGapAsync(long id)
        {
            var knowledgeGap = await _knowledgeGapRepository.GetAsync(
                predicate: kg => kg.Id == id,
                disableTracking: false);

            if (knowledgeGap == null)
                return false;

            await _knowledgeGapRepository.DeleteAsync(knowledgeGap);
            return true;
        }

        public async Task<int> CountKnowledgeGapsAsync(long? studentSubjectId = null, string? severity = null)
        {
            if (!studentSubjectId.HasValue && string.IsNullOrEmpty(severity))
                return await _knowledgeGapRepository.CountAsync();

            Expression<Func<KnowledgeGap, bool>> predicate = kg =>
                (!studentSubjectId.HasValue || kg.StudentSubjectId == studentSubjectId.Value) &&
                (string.IsNullOrEmpty(severity) || kg.Severity == severity);

            return await _knowledgeGapRepository.CountAsync(predicate);
        }

        public async Task<bool> KnowledgeGapExistsAsync(long id)
        {
            return await _knowledgeGapRepository.ExistsAsync(kg => kg.Id == id);
        }
    }
}