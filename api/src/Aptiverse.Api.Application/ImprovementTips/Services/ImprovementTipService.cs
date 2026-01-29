using Aptiverse.Api.Application.ImprovementTips.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.ImprovementTips.Services
{
    public class ImprovementTipService(
        IRepository<ImprovementTip> improvementTipRepository,
        IMapper mapper) : IImprovementTipService
    {
        private readonly IRepository<ImprovementTip> _improvementTipRepository = improvementTipRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ImprovementTipDto> CreateImprovementTipAsync(CreateImprovementTipDto createImprovementTipDto)
        {
            ArgumentNullException.ThrowIfNull(createImprovementTipDto);

            ImprovementTip improvementTip = _mapper.Map<ImprovementTip>(createImprovementTipDto);
            await _improvementTipRepository.AddAsync(improvementTip);
            return _mapper.Map<ImprovementTipDto>(improvementTip);
        }

        public async Task<ImprovementTipDto?> GetImprovementTipByIdAsync(long id)
        {
            var improvementTip = await _improvementTipRepository.GetAsync(
                predicate: it => it.Id == id,
                include: query => query.Include(it => it.StudentSubject),
                disableTracking: false);

            if (improvementTip == null)
                return null;

            return _mapper.Map<ImprovementTipDto>(improvementTip);
        }

        public async Task<PaginatedResult<ImprovementTipDto>> GetImprovementTipsAsync(
            long? studentSubjectId = null,
            string? search = null,
            int? minPriority = null,
            int? maxPriority = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<ImprovementTip, bool>>? predicate = BuildFilterPredicate(
                studentSubjectId, search, minPriority, maxPriority);

            Func<IQueryable<ImprovementTip>, IOrderedQueryable<ImprovementTip>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _improvementTipRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query.Include(it => it.StudentSubject));

            var improvementTipDtos = _mapper.Map<List<ImprovementTipDto>>(paginatedResult.Data);

            return new PaginatedResult<ImprovementTipDto>(
                improvementTipDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<ImprovementTip, bool>>? BuildFilterPredicate(
            long? studentSubjectId,
            string? search,
            int? minPriority,
            int? maxPriority)
        {
            if (!studentSubjectId.HasValue && string.IsNullOrEmpty(search) &&
                !minPriority.HasValue && !maxPriority.HasValue)
                return null;

            return it =>
                (!studentSubjectId.HasValue || it.StudentSubjectId == studentSubjectId.Value) &&
                (string.IsNullOrEmpty(search) || it.Tip.Contains(search)) &&
                (!minPriority.HasValue || it.Priority >= minPriority.Value) &&
                (!maxPriority.HasValue || it.Priority <= maxPriority.Value);
        }

        private Func<IQueryable<ImprovementTip>, IOrderedQueryable<ImprovementTip>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "priority" => sortDescending
                    ? query => query.OrderByDescending(it => it.Priority).ThenByDescending(it => it.Id)
                    : query => query.OrderBy(it => it.Priority).ThenBy(it => it.Id),
                "tip" => sortDescending
                    ? query => query.OrderByDescending(it => it.Tip).ThenByDescending(it => it.Id)
                    : query => query.OrderBy(it => it.Tip).ThenBy(it => it.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(it => it.Id)
                    : query => query.OrderBy(it => it.Id)
            };
        }

        public async Task<ImprovementTipDto> UpdateImprovementTipAsync(long id, UpdateImprovementTipDto updateImprovementTipDto)
        {
            var existingImprovementTip = await _improvementTipRepository.GetAsync(
                predicate: it => it.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"ImprovementTip with ID {id} not found");

            _mapper.Map(updateImprovementTipDto, existingImprovementTip);
            await _improvementTipRepository.UpdateAsync(existingImprovementTip);
            return _mapper.Map<ImprovementTipDto>(existingImprovementTip);
        }

        public async Task<bool> DeleteImprovementTipAsync(long id)
        {
            var improvementTip = await _improvementTipRepository.GetAsync(
                predicate: it => it.Id == id,
                disableTracking: false);

            if (improvementTip == null)
                return false;

            await _improvementTipRepository.DeleteAsync(improvementTip);
            return true;
        }

        public async Task<int> CountImprovementTipsAsync(long? studentSubjectId = null)
        {
            if (!studentSubjectId.HasValue)
                return await _improvementTipRepository.CountAsync();

            Expression<Func<ImprovementTip, bool>> predicate = it => it.StudentSubjectId == studentSubjectId.Value;
            return await _improvementTipRepository.CountAsync(predicate);
        }

        public async Task<bool> ImprovementTipExistsAsync(long id)
        {
            return await _improvementTipRepository.ExistsAsync(it => it.Id == id);
        }
    }
}