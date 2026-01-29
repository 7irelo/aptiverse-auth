using Aptiverse.Api.Application.TeacherStudents.Dtos;
using Aptiverse.Api.Domain.Models.Teachers;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.TeacherStudents.Services
{
    public class TeacherStudentService(
        IRepository<TeacherStudent> teacherStudentRepository,
        IMapper mapper) : ITeacherStudentService
    {
        private readonly IRepository<TeacherStudent> _teacherStudentRepository = teacherStudentRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<TeacherStudentDto> CreateTeacherStudentAsync(CreateTeacherStudentDto createTeacherStudentDto)
        {
            ArgumentNullException.ThrowIfNull(createTeacherStudentDto);

            TeacherStudent teacherStudent = _mapper.Map<TeacherStudent>(createTeacherStudentDto);
            await _teacherStudentRepository.AddAsync(teacherStudent);
            return _mapper.Map<TeacherStudentDto>(teacherStudent);
        }

        public async Task<TeacherStudentDto?> GetTeacherStudentByIdAsync(long id)
        {
            var teacherStudent = await _teacherStudentRepository.GetAsync(
                predicate: ts => ts.Id == id,
                include: query => query
                    .Include(ts => ts.Teacher)
                    .Include(ts => ts.Student),
                disableTracking: false);

            if (teacherStudent == null)
                return null;

            return _mapper.Map<TeacherStudentDto>(teacherStudent);
        }

        public async Task<PaginatedResult<TeacherStudentDto>> GetTeacherStudentsAsync(
            long? teacherId = null,
            long? studentId = null,
            bool? isActive = null,
            DateTime? assignedAfter = null,
            DateTime? assignedBefore = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<TeacherStudent, bool>>? predicate = BuildFilterPredicate(
                teacherId, studentId, isActive, assignedAfter, assignedBefore);

            Func<IQueryable<TeacherStudent>, IOrderedQueryable<TeacherStudent>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _teacherStudentRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(ts => ts.Teacher)
                    .Include(ts => ts.Student));

            var teacherStudentDtos = _mapper.Map<List<TeacherStudentDto>>(paginatedResult.Data);

            return new PaginatedResult<TeacherStudentDto>(
                teacherStudentDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<TeacherStudent, bool>>? BuildFilterPredicate(
            long? teacherId,
            long? studentId,
            bool? isActive,
            DateTime? assignedAfter,
            DateTime? assignedBefore)
        {
            if (!teacherId.HasValue && !studentId.HasValue &&
                !isActive.HasValue && !assignedAfter.HasValue && !assignedBefore.HasValue)
                return null;

            return ts =>
                (!teacherId.HasValue || ts.TeacherId == teacherId.Value) &&
                (!studentId.HasValue || ts.StudentId == studentId.Value) &&
                (!isActive.HasValue || ts.IsActive == isActive.Value) &&
                (!assignedAfter.HasValue || ts.AssignedDate >= assignedAfter.Value) &&
                (!assignedBefore.HasValue || ts.AssignedDate <= assignedBefore.Value);
        }

        private Func<IQueryable<TeacherStudent>, IOrderedQueryable<TeacherStudent>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "assigneddate" => sortDescending
                    ? query => query.OrderByDescending(ts => ts.AssignedDate).ThenByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.AssignedDate).ThenBy(ts => ts.Id),
                "teacherid" => sortDescending
                    ? query => query.OrderByDescending(ts => ts.TeacherId).ThenByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.TeacherId).ThenBy(ts => ts.Id),
                "studentid" => sortDescending
                    ? query => query.OrderByDescending(ts => ts.StudentId).ThenByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.StudentId).ThenBy(ts => ts.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(ts => ts.Id)
                    : query => query.OrderBy(ts => ts.Id)
            };
        }

        public async Task<TeacherStudentDto> UpdateTeacherStudentAsync(long id, UpdateTeacherStudentDto updateTeacherStudentDto)
        {
            var existingTeacherStudent = await _teacherStudentRepository.GetAsync(
                predicate: ts => ts.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"TeacherStudent with ID {id} not found");

            _mapper.Map(updateTeacherStudentDto, existingTeacherStudent);
            await _teacherStudentRepository.UpdateAsync(existingTeacherStudent);
            return _mapper.Map<TeacherStudentDto>(existingTeacherStudent);
        }

        public async Task<bool> DeleteTeacherStudentAsync(long id)
        {
            var teacherStudent = await _teacherStudentRepository.GetAsync(
                predicate: ts => ts.Id == id,
                disableTracking: false);

            if (teacherStudent == null)
                return false;

            await _teacherStudentRepository.DeleteAsync(teacherStudent);
            return true;
        }

        public async Task<int> CountTeacherStudentsAsync(long? teacherId = null, long? studentId = null, bool? isActive = null)
        {
            if (!teacherId.HasValue && !studentId.HasValue && !isActive.HasValue)
                return await _teacherStudentRepository.CountAsync();

            Expression<Func<TeacherStudent, bool>> predicate = ts =>
                (!teacherId.HasValue || ts.TeacherId == teacherId.Value) &&
                (!studentId.HasValue || ts.StudentId == studentId.Value) &&
                (!isActive.HasValue || ts.IsActive == isActive.Value);

            return await _teacherStudentRepository.CountAsync(predicate);
        }

        public async Task<bool> TeacherStudentExistsAsync(long id)
        {
            return await _teacherStudentRepository.ExistsAsync(ts => ts.Id == id);
        }
    }
}