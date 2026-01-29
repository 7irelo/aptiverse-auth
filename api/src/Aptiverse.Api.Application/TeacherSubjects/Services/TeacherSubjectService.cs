using Aptiverse.Api.Application.TeacherSubjects.Dtos;
using Aptiverse.Api.Domain.Models.Teachers;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.TeacherSubjects.Services
{
    public class TeacherSubjectService(
        IRepository<TeacherSubject> teacherSubjectRepository,
        IMapper mapper) : ITeacherSubjectService
    {
        private readonly IRepository<TeacherSubject> _teacherSubjectRepository = teacherSubjectRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<TeacherSubjectDto> CreateTeacherSubjectAsync(CreateTeacherSubjectDto createTeacherSubjectDto)
        {
            ArgumentNullException.ThrowIfNull(createTeacherSubjectDto);

            TeacherSubject teacherSubject = _mapper.Map<TeacherSubject>(createTeacherSubjectDto);
            await _teacherSubjectRepository.AddAsync(teacherSubject);
            return _mapper.Map<TeacherSubjectDto>(teacherSubject);
        }

        public async Task<TeacherSubjectDto?> GetTeacherSubjectByIdAsync(long id)
        {
            var teacherSubject = await _teacherSubjectRepository.GetAsync(
                predicate: ts => ts.Id == id,
                include: query => query
                    .Include(ts => ts.Teacher)
                    .Include(ts => ts.Subject),
                disableTracking: false);

            if (teacherSubject == null)
                return null;

            return _mapper.Map<TeacherSubjectDto>(teacherSubject);
        }

        public async Task<PaginatedResult<TeacherSubjectDto>> GetTeacherSubjectsAsync(
            long? teacherId = null,
            string? subjectId = null,
            int? minProficiencyLevel = null,
            int? maxProficiencyLevel = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<TeacherSubject, bool>>? predicate = BuildFilterPredicate(
                teacherId, subjectId, minProficiencyLevel, maxProficiencyLevel);

            Func<IQueryable<TeacherSubject>, IOrderedQueryable<TeacherSubject>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _teacherSubjectRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(ts => ts.Teacher)
                    .Include(ts => ts.Subject));

            var teacherSubjectDtos = _mapper.Map<List<TeacherSubjectDto>>(paginatedResult.Data);

            return new PaginatedResult<TeacherSubjectDto>(
                teacherSubjectDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<TeacherSubject, bool>>? BuildFilterPredicate(
            long? teacherId,
            string? subjectId,
            int? minProficiencyLevel,
            int? maxProficiencyLevel)
        {
            if (!teacherId.HasValue && string.IsNullOrEmpty(subjectId) &&
                !minProficiencyLevel.HasValue && !maxProficiencyLevel.HasValue)
                return null;

            return ts =>
                (!teacherId.HasValue || ts.TeacherId == teacherId.Value) &&
                (string.IsNullOrEmpty(subjectId) || ts.SubjectId == subjectId) &&
                (!minProficiencyLevel.HasValue || ts.ProficiencyLevel >= minProficiencyLevel.Value) &&
                (!maxProficiencyLevel.HasValue || ts.ProficiencyLevel <= maxProficiencyLevel.Value);
        }

        private Func<IQueryable<TeacherSubject>, IOrderedQueryable<TeacherSubject>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "proficiencylevel" => sortDescending
                    ? query => query.OrderByDescending(ts => ts.ProficiencyLevel).ThenByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.ProficiencyLevel).ThenBy(ts => ts.Id),
                "teacherid" => sortDescending
                    ? query => query.OrderByDescending(ts => ts.TeacherId).ThenByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.TeacherId).ThenBy(ts => ts.Id),
                "subjectid" => sortDescending
                    ? query => query.OrderByDescending(ts => ts.SubjectId).ThenByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.SubjectId).ThenBy(ts => ts.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.Id)
            };
        }

        public async Task<TeacherSubjectDto> UpdateTeacherSubjectAsync(long id, UpdateTeacherSubjectDto updateTeacherSubjectDto)
        {
            var existingTeacherSubject = await _teacherSubjectRepository.GetAsync(
                predicate: ts => ts.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"TeacherSubject with ID {id} not found");

            _mapper.Map(updateTeacherSubjectDto, existingTeacherSubject);
            await _teacherSubjectRepository.UpdateAsync(existingTeacherSubject);
            return _mapper.Map<TeacherSubjectDto>(existingTeacherSubject);
        }

        public async Task<bool> DeleteTeacherSubjectAsync(long id)
        {
            var teacherSubject = await _teacherSubjectRepository.GetAsync(
                predicate: ts => ts.Id == id,
                disableTracking: false);

            if (teacherSubject == null)
                return false;

            await _teacherSubjectRepository.DeleteAsync(teacherSubject);
            return true;
        }

        public async Task<int> CountTeacherSubjectsAsync(long? teacherId = null, string? subjectId = null)
        {
            if (!teacherId.HasValue && string.IsNullOrEmpty(subjectId))
                return await _teacherSubjectRepository.CountAsync();

            Expression<Func<TeacherSubject, bool>> predicate = ts =>
                (!teacherId.HasValue || ts.TeacherId == teacherId.Value) &&
                (string.IsNullOrEmpty(subjectId) || ts.SubjectId == subjectId);

            return await _teacherSubjectRepository.CountAsync(predicate);
        }

        public async Task<bool> TeacherSubjectExistsAsync(long id)
        {
            return await _teacherSubjectRepository.ExistsAsync(ts => ts.Id == id);
        }
    }
}