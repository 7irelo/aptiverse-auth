using Aptiverse.Api.Application.Teachers.Dtos;
using Aptiverse.Api.Domain.Models.Teachers;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.Teachers.Services
{
    public class TeacherService(
        IRepository<Teacher> teacherRepository,
        IMapper mapper) : ITeacherService
    {
        private readonly IRepository<Teacher> _teacherRepository = teacherRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto createTeacherDto)
        {
            ArgumentNullException.ThrowIfNull(createTeacherDto);

            Teacher teacher = _mapper.Map<Teacher>(createTeacherDto);
            teacher.CreatedAt = DateTime.UtcNow;
            await _teacherRepository.AddAsync(teacher);
            return _mapper.Map<TeacherDto>(teacher);
        }

        public async Task<TeacherDto?> GetTeacherByIdAsync(long id)
        {
            var teacher = await _teacherRepository.GetAsync(
                predicate: t => t.Id == id,
                include: query => query
                    .Include(t => t.TeacherSubjects)
                    .Include(t => t.TeacherStudents)
                    .Include(t => t.Courses)
                    .Include(t => t.Resources),
                disableTracking: false);

            if (teacher == null)
                return null;

            return _mapper.Map<TeacherDto>(teacher);
        }

        public async Task<PaginatedResult<TeacherDto>> GetTeachersAsync(
            string? search = null,
            string? specialization = null,
            int? minYearsOfExperience = null,
            bool? isVerified = null,
            decimal? minHourlyRate = null,
            decimal? maxHourlyRate = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<Teacher, bool>>? predicate = BuildFilterPredicate(
                search, specialization, minYearsOfExperience, isVerified, minHourlyRate, maxHourlyRate);

            Func<IQueryable<Teacher>, IOrderedQueryable<Teacher>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _teacherRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(t => t.TeacherSubjects)
                    .Include(t => t.TeacherStudents)
                    .Include(t => t.Courses)
                    .Include(t => t.Resources));

            var teacherDtos = _mapper.Map<List<TeacherDto>>(paginatedResult.Data);

            return new PaginatedResult<TeacherDto>(
                teacherDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<Teacher, bool>>? BuildFilterPredicate(
            string? search,
            string? specialization,
            int? minYearsOfExperience,
            bool? isVerified,
            decimal? minHourlyRate,
            decimal? maxHourlyRate)
        {
            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(specialization) &&
                !minYearsOfExperience.HasValue && !isVerified.HasValue &&
                !minHourlyRate.HasValue && !maxHourlyRate.HasValue)
                return null;

            return t =>
                (string.IsNullOrEmpty(search) || t.UserId.Contains(search) || t.Qualification.Contains(search) || t.Bio.Contains(search)) &&
                (string.IsNullOrEmpty(specialization) || t.Specialization == specialization) &&
                (!minYearsOfExperience.HasValue || t.YearsOfExperience >= minYearsOfExperience.Value) &&
                (!isVerified.HasValue || t.IsVerified == isVerified.Value) &&
                (!minHourlyRate.HasValue || t.HourlyRate >= minHourlyRate.Value) &&
                (!maxHourlyRate.HasValue || t.HourlyRate <= maxHourlyRate.Value);
        }

        private Func<IQueryable<Teacher>, IOrderedQueryable<Teacher>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "userid" => sortDescending
                    ? query => query.OrderByDescending(t => t.UserId).ThenByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.UserId).ThenBy(t => t.Id),
                "specialization" => sortDescending
                    ? query => query.OrderByDescending(t => t.Specialization).ThenByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.Specialization).ThenBy(t => t.Id),
                "yearsofexperience" => sortDescending
                    ? query => query.OrderByDescending(t => t.YearsOfExperience).ThenByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.YearsOfExperience).ThenBy(t => t.Id),
                "hourlyrate" => sortDescending
                    ? query => query.OrderByDescending(t => t.HourlyRate).ThenByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.HourlyRate).ThenBy(t => t.Id),
                "createdat" => sortDescending
                    ? query => query.OrderByDescending(t => t.CreatedAt).ThenByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.CreatedAt).ThenBy(t => t.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.Id)
            };
        }

        public async Task<TeacherDto> UpdateTeacherAsync(long id, UpdateTeacherDto updateTeacherDto)
        {
            var existingTeacher = await _teacherRepository.GetAsync(
                predicate: t => t.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Teacher with ID {id} not found");

            _mapper.Map(updateTeacherDto, existingTeacher);
            await _teacherRepository.UpdateAsync(existingTeacher);
            return _mapper.Map<TeacherDto>(existingTeacher);
        }

        public async Task<bool> DeleteTeacherAsync(long id)
        {
            var teacher = await _teacherRepository.GetAsync(
                predicate: t => t.Id == id,
                disableTracking: false);

            if (teacher == null)
                return false;

            await _teacherRepository.DeleteAsync(teacher);
            return true;
        }

        public async Task<int> CountTeachersAsync(string? specialization = null, bool? isVerified = null)
        {
            if (string.IsNullOrEmpty(specialization) && !isVerified.HasValue)
                return await _teacherRepository.CountAsync();

            Expression<Func<Teacher, bool>> predicate = t =>
                (string.IsNullOrEmpty(specialization) || t.Specialization == specialization) &&
                (!isVerified.HasValue || t.IsVerified == isVerified.Value);

            return await _teacherRepository.CountAsync(predicate);
        }

        public async Task<bool> TeacherExistsAsync(long id)
        {
            return await _teacherRepository.ExistsAsync(t => t.Id == id);
        }
    }
}