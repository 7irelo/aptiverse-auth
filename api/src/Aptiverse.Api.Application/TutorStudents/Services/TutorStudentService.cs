using Aptiverse.Api.Application.TutorStudents.Dtos;
using Aptiverse.Api.Domain.Models.Tutors;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.TutorStudents.Services
{
    public class TutorStudentService(
        IRepository<TutorStudent> tutorStudentRepository,
        IMapper mapper) : ITutorStudentService
    {
        private readonly IRepository<TutorStudent> _tutorStudentRepository = tutorStudentRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<TutorStudentDto> CreateTutorStudentAsync(CreateTutorStudentDto createTutorStudentDto)
        {
            ArgumentNullException.ThrowIfNull(createTutorStudentDto);

            TutorStudent tutorStudent = _mapper.Map<TutorStudent>(createTutorStudentDto);
            await _tutorStudentRepository.AddAsync(tutorStudent);
            return _mapper.Map<TutorStudentDto>(tutorStudent);
        }

        public async Task<TutorStudentDto?> GetTutorStudentByIdAsync(long id)
        {
            var tutorStudent = await _tutorStudentRepository.GetAsync(
                predicate: ts => ts.Id == id,
                include: query => query
                    .Include(ts => ts.Tutor)
                    .Include(ts => ts.Student),
                disableTracking: false);

            if (tutorStudent == null)
                return null;

            return _mapper.Map<TutorStudentDto>(tutorStudent);
        }

        public async Task<PaginatedResult<TutorStudentDto>> GetTutorStudentsAsync(
            long? tutorId = null,
            long? studentId = null,
            bool? isActive = null,
            int? minSessionsPerWeek = null,
            int? maxSessionsPerWeek = null,
            DateTime? startedAfter = null,
            DateTime? startedBefore = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<TutorStudent, bool>>? predicate = BuildFilterPredicate(
                tutorId, studentId, isActive, minSessionsPerWeek, maxSessionsPerWeek, startedAfter, startedBefore);

            Func<IQueryable<TutorStudent>, IOrderedQueryable<TutorStudent>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _tutorStudentRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(ts => ts.Tutor)
                    .Include(ts => ts.Student));

            var tutorStudentDtos = _mapper.Map<List<TutorStudentDto>>(paginatedResult.Data);

            return new PaginatedResult<TutorStudentDto>(
                tutorStudentDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<TutorStudent, bool>>? BuildFilterPredicate(
            long? tutorId,
            long? studentId,
            bool? isActive,
            int? minSessionsPerWeek,
            int? maxSessionsPerWeek,
            DateTime? startedAfter,
            DateTime? startedBefore)
        {
            if (!tutorId.HasValue && !studentId.HasValue &&
                !isActive.HasValue && !minSessionsPerWeek.HasValue &&
                !maxSessionsPerWeek.HasValue && !startedAfter.HasValue && !startedBefore.HasValue)
                return null;

            return ts =>
                (!tutorId.HasValue || ts.TutorId == tutorId.Value) &&
                (!studentId.HasValue || ts.StudentId == studentId.Value) &&
                (!isActive.HasValue || ts.IsActive == isActive.Value) &&
                (!minSessionsPerWeek.HasValue || ts.SessionsPerWeek >= minSessionsPerWeek.Value) &&
                (!maxSessionsPerWeek.HasValue || ts.SessionsPerWeek <= maxSessionsPerWeek.Value) &&
                (!startedAfter.HasValue || ts.StartedDate >= startedAfter.Value) &&
                (!startedBefore.HasValue || ts.StartedDate <= startedBefore.Value);
        }

        private Func<IQueryable<TutorStudent>, IOrderedQueryable<TutorStudent>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "starteddate" => sortDescending
                    ? query => query.OrderByDescending(ts => ts.StartedDate).ThenByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.StartedDate).ThenBy(ts => ts.Id),
                "sessionsperweek" => sortDescending
                    ? query => query.OrderByDescending(ts => ts.SessionsPerWeek).ThenByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.SessionsPerWeek).ThenBy(ts => ts.Id),
                "tutorid" => sortDescending
                    ? query => query.OrderByDescending(ts => ts.TutorId).ThenByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.TutorId).ThenBy(ts => ts.Id),
                "studentid" => sortDescending
                    ? query => query.OrderByDescending(ts => ts.StudentId).ThenByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.StudentId).ThenBy(ts => ts.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.Id)
            };
        }

        public async Task<TutorStudentDto> UpdateTutorStudentAsync(long id, UpdateTutorStudentDto updateTutorStudentDto)
        {
            var existingTutorStudent = await _tutorStudentRepository.GetAsync(
                predicate: ts => ts.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"TutorStudent with ID {id} not found");

            _mapper.Map(updateTutorStudentDto, existingTutorStudent);
            await _tutorStudentRepository.UpdateAsync(existingTutorStudent);
            return _mapper.Map<TutorStudentDto>(existingTutorStudent);
        }

        public async Task<bool> DeleteTutorStudentAsync(long id)
        {
            var tutorStudent = await _tutorStudentRepository.GetAsync(
                predicate: ts => ts.Id == id,
                disableTracking: false);

            if (tutorStudent == null)
                return false;

            await _tutorStudentRepository.DeleteAsync(tutorStudent);
            return true;
        }

        public async Task<int> CountTutorStudentsAsync(long? tutorId = null, long? studentId = null, bool? isActive = null)
        {
            if (!tutorId.HasValue && !studentId.HasValue && !isActive.HasValue)
                return await _tutorStudentRepository.CountAsync();

            Expression<Func<TutorStudent, bool>> predicate = ts =>
                (!tutorId.HasValue || ts.TutorId == tutorId.Value) &&
                (!studentId.HasValue || ts.StudentId == studentId.Value) &&
                (!isActive.HasValue || ts.IsActive == isActive.Value);

            return await _tutorStudentRepository.CountAsync(predicate);
        }

        public async Task<bool> TutorStudentExistsAsync(long id)
        {
            return await _tutorStudentRepository.ExistsAsync(ts => ts.Id == id);
        }
    }
}