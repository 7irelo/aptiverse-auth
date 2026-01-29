using Aptiverse.Api.Application.StudentSubjectAnalyticss.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.StudentSubjectAnalyticss.Services
{
    public class StudentSubjectAnalyticsService(
        IRepository<StudentSubjectAnalytics> studentSubjectAnalyticsRepository,
        IMapper mapper) : IStudentSubjectAnalyticsService
    {
        private readonly IRepository<StudentSubjectAnalytics> _studentSubjectAnalyticsRepository = studentSubjectAnalyticsRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<StudentSubjectAnalyticsDto> CreateStudentSubjectAnalyticsAsync(CreateStudentSubjectAnalyticsDto createStudentSubjectAnalyticsDto)
        {
            ArgumentNullException.ThrowIfNull(createStudentSubjectAnalyticsDto);

            StudentSubjectAnalytics studentSubjectAnalytics = _mapper.Map<StudentSubjectAnalytics>(createStudentSubjectAnalyticsDto);
            await _studentSubjectAnalyticsRepository.AddAsync(studentSubjectAnalytics);
            return _mapper.Map<StudentSubjectAnalyticsDto>(studentSubjectAnalytics);
        }

        public async Task<StudentSubjectAnalyticsDto?> GetStudentSubjectAnalyticsByIdAsync(long id)
        {
            var studentSubjectAnalytics = await _studentSubjectAnalyticsRepository.GetAsync(
                predicate: ssa => ssa.Id == id,
                include: query => query.Include(ssa => ssa.StudentSubject),
                disableTracking: false);

            if (studentSubjectAnalytics == null)
                return null;

            return _mapper.Map<StudentSubjectAnalyticsDto>(studentSubjectAnalytics);
        }

        public async Task<StudentSubjectAnalyticsDto?> GetStudentSubjectAnalyticsByStudentSubjectIdAsync(long studentSubjectId)
        {
            var studentSubjectAnalytics = await _studentSubjectAnalyticsRepository.GetAsync(
                predicate: ssa => ssa.StudentSubjectId == studentSubjectId,
                include: query => query.Include(ssa => ssa.StudentSubject),
                disableTracking: false);

            if (studentSubjectAnalytics == null)
                return null;

            return _mapper.Map<StudentSubjectAnalyticsDto>(studentSubjectAnalytics);
        }

        public async Task<PaginatedResult<StudentSubjectAnalyticsDto>> GetStudentSubjectAnalyticsAsync(
            long? studentSubjectId = null,
            int? minConsistency = null,
            int? maxConsistency = null,
            double? minAttendanceRate = null,
            double? maxAttendanceRate = null,
            double? minMotivationLevel = null,
            double? maxMotivationLevel = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<StudentSubjectAnalytics, bool>>? predicate = BuildFilterPredicate(
                studentSubjectId, minConsistency, maxConsistency, minAttendanceRate, maxAttendanceRate, minMotivationLevel, maxMotivationLevel);

            Func<IQueryable<StudentSubjectAnalytics>, IOrderedQueryable<StudentSubjectAnalytics>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _studentSubjectAnalyticsRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query.Include(ssa => ssa.StudentSubject));

            var studentSubjectAnalyticsDtos = _mapper.Map<List<StudentSubjectAnalyticsDto>>(paginatedResult.Data);

            return new PaginatedResult<StudentSubjectAnalyticsDto>(
                studentSubjectAnalyticsDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<StudentSubjectAnalytics, bool>>? BuildFilterPredicate(
            long? studentSubjectId,
            int? minConsistency,
            int? maxConsistency,
            double? minAttendanceRate,
            double? maxAttendanceRate,
            double? minMotivationLevel,
            double? maxMotivationLevel)
        {
            if (!studentSubjectId.HasValue && !minConsistency.HasValue && !maxConsistency.HasValue &&
                !minAttendanceRate.HasValue && !maxAttendanceRate.HasValue &&
                !minMotivationLevel.HasValue && !maxMotivationLevel.HasValue)
                return null;

            return ssa =>
                (!studentSubjectId.HasValue || ssa.StudentSubjectId == studentSubjectId.Value) &&
                (!minConsistency.HasValue || ssa.Consistency >= minConsistency.Value) &&
                (!maxConsistency.HasValue || ssa.Consistency <= maxConsistency.Value) &&
                (!minAttendanceRate.HasValue || ssa.AttendanceRate >= minAttendanceRate.Value) &&
                (!maxAttendanceRate.HasValue || ssa.AttendanceRate <= maxAttendanceRate.Value) &&
                (!minMotivationLevel.HasValue || ssa.MotivationLevel >= minMotivationLevel.Value) &&
                (!maxMotivationLevel.HasValue || ssa.MotivationLevel <= maxMotivationLevel.Value);
        }

        private Func<IQueryable<StudentSubjectAnalytics>, IOrderedQueryable<StudentSubjectAnalytics>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "consistency" => sortDescending
                    ? query => query.OrderByDescending(ssa => ssa.Consistency).ThenByDescending(ssa => ssa.Id)
                    : query => query.OrderBy(ssa => ssa.Consistency).ThenBy(ssa => ssa.Id),
                "attendancerate" => sortDescending
                    ? query => query.OrderByDescending(ssa => ssa.AttendanceRate).ThenByDescending(ssa => ssa.Id)
                    : query => query.OrderBy(ssa => ssa.AttendanceRate).ThenBy(ssa => ssa.Id),
                "motivationlevel" => sortDescending
                    ? query => query.OrderByDescending(ssa => ssa.MotivationLevel).ThenByDescending(ssa => ssa.Id)
                    : query => query.OrderBy(ssa => ssa.MotivationLevel).ThenBy(ssa => ssa.Id),
                "interestlevel" => sortDescending
                    ? query => query.OrderByDescending(ssa => ssa.InterestLevel).ThenByDescending(ssa => ssa.Id)
                    : query => query.OrderBy(ssa => ssa.InterestLevel).ThenBy(ssa => ssa.Id),
                "participationrate" => sortDescending
                    ? query => query.OrderByDescending(ssa => ssa.ParticipationRate).ThenByDescending(ssa => ssa.Id)
                    : query => query.OrderBy(ssa => ssa.ParticipationRate).ThenBy(ssa => ssa.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(ssa => ssa.Id)
                    : query => query.OrderBy(ssa => ssa.Id)
            };
        }

        public async Task<StudentSubjectAnalyticsDto> UpdateStudentSubjectAnalyticsAsync(long id, UpdateStudentSubjectAnalyticsDto updateStudentSubjectAnalyticsDto)
        {
            var existingStudentSubjectAnalytics = await _studentSubjectAnalyticsRepository.GetAsync(
                predicate: ssa => ssa.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"StudentSubjectAnalytics with ID {id} not found");

            _mapper.Map(updateStudentSubjectAnalyticsDto, existingStudentSubjectAnalytics);
            await _studentSubjectAnalyticsRepository.UpdateAsync(existingStudentSubjectAnalytics);
            return _mapper.Map<StudentSubjectAnalyticsDto>(existingStudentSubjectAnalytics);
        }

        public async Task<bool> DeleteStudentSubjectAnalyticsAsync(long id)
        {
            var studentSubjectAnalytics = await _studentSubjectAnalyticsRepository.GetAsync(
                predicate: ssa => ssa.Id == id,
                disableTracking: false);

            if (studentSubjectAnalytics == null)
                return false;

            await _studentSubjectAnalyticsRepository.DeleteAsync(studentSubjectAnalytics);
            return true;
        }

        public async Task<int> CountStudentSubjectAnalyticsAsync(long? studentSubjectId = null)
        {
            if (!studentSubjectId.HasValue)
                return await _studentSubjectAnalyticsRepository.CountAsync();

            Expression<Func<StudentSubjectAnalytics, bool>> predicate = ssa => ssa.StudentSubjectId == studentSubjectId.Value;
            return await _studentSubjectAnalyticsRepository.CountAsync(predicate);
        }

        public async Task<bool> StudentSubjectAnalyticsExistsAsync(long id)
        {
            return await _studentSubjectAnalyticsRepository.ExistsAsync(ssa => ssa.Id == id);
        }

        public async Task<bool> StudentSubjectAnalyticsExistsForStudentSubjectAsync(long studentSubjectId)
        {
            return await _studentSubjectAnalyticsRepository.ExistsAsync(ssa => ssa.StudentSubjectId == studentSubjectId);
        }
    }
}