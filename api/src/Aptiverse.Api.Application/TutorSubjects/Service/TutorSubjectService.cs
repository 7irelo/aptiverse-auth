using Aptiverse.Api.Application.TutorSubjects.Dtos;
using Aptiverse.Api.Domain.Models.Teachers;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.TutorSubjects.Services
{
    public class TutorSubjectService(
        IRepository<TutorSubject> tutorSubjectRepository,
        IMapper mapper) : ITutorSubjectService
    {
        private readonly IRepository<TutorSubject> _tutorSubjectRepository = tutorSubjectRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<TutorSubjectDto> CreateTutorSubjectAsync(CreateTutorSubjectDto createTutorSubjectDto)
        {
            ArgumentNullException.ThrowIfNull(createTutorSubjectDto);

            TutorSubject tutorSubject = _mapper.Map<TutorSubject>(createTutorSubjectDto);
            await _tutorSubjectRepository.AddAsync(tutorSubject);
            return _mapper.Map<TutorSubjectDto>(tutorSubject);
        }

        public async Task<TutorSubjectDto?> GetTutorSubjectByIdAsync(long id)
        {
            var tutorSubject = await _tutorSubjectRepository.GetAsync(
                predicate: ts => ts.Id == id,
                include: query => query
                    .Include(ts => ts.Tutor)
                    .Include(ts => ts.Subject),
                disableTracking: false);

            if (tutorSubject == null)
                return null;

            return _mapper.Map<TutorSubjectDto>(tutorSubject);
        }

        public async Task<PaginatedResult<TutorSubjectDto>> GetTutorSubjectsAsync(
            long? tutorId = null,
            string? subjectId = null,
            int? minProficiencyLevel = null,
            int? maxProficiencyLevel = null,
            decimal? minHourlyRate = null,
            decimal? maxHourlyRate = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<TutorSubject, bool>>? predicate = BuildFilterPredicate(
                tutorId, subjectId, minProficiencyLevel, maxProficiencyLevel, minHourlyRate, maxHourlyRate);

            Func<IQueryable<TutorSubject>, IOrderedQueryable<TutorSubject>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _tutorSubjectRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(ts => ts.Tutor)
                    .Include(ts => ts.Subject));

            var tutorSubjectDtos = _mapper.Map<List<TutorSubjectDto>>(paginatedResult.Data);

            return new PaginatedResult<TutorSubjectDto>(
                tutorSubjectDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<TutorSubject, bool>>? BuildFilterPredicate(
            long? tutorId,
            string? subjectId,
            int? minProficiencyLevel,
            int? maxProficiencyLevel,
            decimal? minHourlyRate,
            decimal? maxHourlyRate)
        {
            if (!tutorId.HasValue && string.IsNullOrEmpty(subjectId) &&
                !minProficiencyLevel.HasValue && !maxProficiencyLevel.HasValue &&
                !minHourlyRate.HasValue && !maxHourlyRate.HasValue)
                return null;

            return ts =>
                (!tutorId.HasValue || ts.TutorId == tutorId.Value) &&
                (string.IsNullOrEmpty(subjectId) || ts.SubjectId == subjectId) &&
                (!minProficiencyLevel.HasValue || ts.ProficiencyLevel >= minProficiencyLevel.Value) &&
                (!maxProficiencyLevel.HasValue || ts.ProficiencyLevel <= maxProficiencyLevel.Value) &&
                (!minHourlyRate.HasValue || ts.CustomHourlyRate >= minHourlyRate.Value) &&
                (!maxHourlyRate.HasValue || ts.CustomHourlyRate <= maxHourlyRate.Value);
        }

        private Func<IQueryable<TutorSubject>, IOrderedQueryable<TutorSubject>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "proficiencylevel" => sortDescending
                    ? query => query.OrderByDescending(ts => ts.ProficiencyLevel).ThenByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.ProficiencyLevel).ThenBy(ts => ts.Id),
                "customhourlyrate" => sortDescending
                    ? query => query.OrderByDescending(ts => ts.CustomHourlyRate).ThenByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.CustomHourlyRate).ThenBy(ts => ts.Id),
                "tutorid" => sortDescending
                    ? query => query.OrderByDescending(ts => ts.TutorId).ThenByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.TutorId).ThenBy(ts => ts.Id),
                "subjectid" => sortDescending
                    ? query => query.OrderByDescending(ts => ts.SubjectId).ThenByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.SubjectId).ThenBy(ts => ts.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.Id)
            };
        }

        public async Task<TutorSubjectDto> UpdateTutorSubjectAsync(long id, UpdateTutorSubjectDto updateTutorSubjectDto)
        {
            var existingTutorSubject = await _tutorSubjectRepository.GetAsync(
                predicate: ts => ts.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"TutorSubject with ID {id} not found");

            _mapper.Map(updateTutorSubjectDto, existingTutorSubject);
            await _tutorSubjectRepository.UpdateAsync(existingTutorSubject);
            return _mapper.Map<TutorSubjectDto>(existingTutorSubject);
        }

        public async Task<bool> DeleteTutorSubjectAsync(long id)
        {
            var tutorSubject = await _tutorSubjectRepository.GetAsync(
                predicate: ts => ts.Id == id,
                disableTracking: false);

            if (tutorSubject == null)
                return false;

            await _tutorSubjectRepository.DeleteAsync(tutorSubject);
            return true;
        }

        public async Task<int> CountTutorSubjectsAsync(long? tutorId = null, string? subjectId = null)
        {
            if (!tutorId.HasValue && string.IsNullOrEmpty(subjectId))
                return await _tutorSubjectRepository.CountAsync();

            Expression<Func<TutorSubject, bool>> predicate = ts =>
                (!tutorId.HasValue || ts.TutorId == tutorId.Value) &&
                (string.IsNullOrEmpty(subjectId) || ts.SubjectId == subjectId);

            return await _tutorSubjectRepository.CountAsync(predicate);
        }

        public async Task<bool> TutorSubjectExistsAsync(long id)
        {
            return await _tutorSubjectRepository.ExistsAsync(ts => ts.Id == id);
        }
    }
}