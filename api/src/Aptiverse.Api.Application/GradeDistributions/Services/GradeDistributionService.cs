using Aptiverse.Api.Application.GradeDistributions.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.GradeDistributions.Services
{
    public class GradeDistributionService(
        IRepository<GradeDistribution> gradeDistributionRepository,
        IMapper mapper) : IGradeDistributionService
    {
        private readonly IRepository<GradeDistribution> _gradeDistributionRepository = gradeDistributionRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<GradeDistributionDto> CreateGradeDistributionAsync(CreateGradeDistributionDto createGradeDistributionDto)
        {
            ArgumentNullException.ThrowIfNull(createGradeDistributionDto);

            GradeDistribution gradeDistribution = _mapper.Map<GradeDistribution>(createGradeDistributionDto);
            await _gradeDistributionRepository.AddAsync(gradeDistribution);
            return _mapper.Map<GradeDistributionDto>(gradeDistribution);
        }

        public async Task<GradeDistributionDto?> GetGradeDistributionByIdAsync(long id)
        {
            var gradeDistribution = await _gradeDistributionRepository.GetAsync(
                predicate: gd => gd.Id == id,
                include: query => query.Include(gd => gd.StudentSubject),
                disableTracking: false);

            if (gradeDistribution == null)
                return null;

            return _mapper.Map<GradeDistributionDto>(gradeDistribution);
        }

        public async Task<PaginatedResult<GradeDistributionDto>> GetGradeDistributionsAsync(
            long? studentSubjectId = null,
            string? grade = null,
            int? minCount = null,
            int? maxCount = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<GradeDistribution, bool>>? predicate = BuildFilterPredicate(
                studentSubjectId, grade, minCount, maxCount);

            Func<IQueryable<GradeDistribution>, IOrderedQueryable<GradeDistribution>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _gradeDistributionRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query.Include(gd => gd.StudentSubject));

            var gradeDistributionDtos = _mapper.Map<List<GradeDistributionDto>>(paginatedResult.Data);

            return new PaginatedResult<GradeDistributionDto>(
                gradeDistributionDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<GradeDistribution, bool>>? BuildFilterPredicate(
            long? studentSubjectId,
            string? grade,
            int? minCount,
            int? maxCount)
        {
            if (!studentSubjectId.HasValue && string.IsNullOrEmpty(grade) && !minCount.HasValue && !maxCount.HasValue)
                return null;

            return gd =>
                (!studentSubjectId.HasValue || gd.StudentSubjectId == studentSubjectId.Value) &&
                (string.IsNullOrEmpty(grade) || gd.Grade == grade) &&
                (!minCount.HasValue || gd.Count >= minCount.Value) &&
                (!maxCount.HasValue || gd.Count <= maxCount.Value);
        }

        private Func<IQueryable<GradeDistribution>, IOrderedQueryable<GradeDistribution>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "grade" => sortDescending
                    ? query => query.OrderByDescending(gd => gd.Grade).ThenByDescending(gd => gd.Id)
                    : query => query.OrderBy(gd => gd.Grade).ThenBy(gd => gd.Id),
                "count" => sortDescending
                    ? query => query.OrderByDescending(gd => gd.Count).ThenByDescending(gd => gd.Id)
                    : query => query.OrderBy(gd => gd.Count).ThenBy(gd => gd.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(gd => gd.Id)
                    : query => query.OrderBy(gd => gd.Id)
            };
        }

        public async Task<GradeDistributionDto> UpdateGradeDistributionAsync(long id, UpdateGradeDistributionDto updateGradeDistributionDto)
        {
            var existingGradeDistribution = await _gradeDistributionRepository.GetAsync(
                predicate: gd => gd.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"GradeDistribution with ID {id} not found");

            _mapper.Map(updateGradeDistributionDto, existingGradeDistribution);
            await _gradeDistributionRepository.UpdateAsync(existingGradeDistribution);
            return _mapper.Map<GradeDistributionDto>(existingGradeDistribution);
        }

        public async Task<bool> DeleteGradeDistributionAsync(long id)
        {
            var gradeDistribution = await _gradeDistributionRepository.GetAsync(
                predicate: gd => gd.Id == id,
                disableTracking: false);

            if (gradeDistribution == null)
                return false;

            await _gradeDistributionRepository.DeleteAsync(gradeDistribution);
            return true;
        }

        public async Task<int> CountGradeDistributionsAsync(long? studentSubjectId = null, string? grade = null)
        {
            if (!studentSubjectId.HasValue && string.IsNullOrEmpty(grade))
                return await _gradeDistributionRepository.CountAsync();

            Expression<Func<GradeDistribution, bool>> predicate = gd =>
                (!studentSubjectId.HasValue || gd.StudentSubjectId == studentSubjectId.Value) &&
                (string.IsNullOrEmpty(grade) || gd.Grade == grade);

            return await _gradeDistributionRepository.CountAsync(predicate);
        }

        public async Task<bool> GradeDistributionExistsAsync(long id)
        {
            return await _gradeDistributionRepository.ExistsAsync(gd => gd.Id == id);
        }
    }
}