using Aptiverse.Api.Application.GrowthTrackings.Dtos;
using Aptiverse.Api.Domain.Models.Rewards;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.GrowthTrackings.Services
{
    public class GrowthTrackingService(
        IRepository<GrowthTracking> growthTrackingRepository,
        IMapper mapper) : IGrowthTrackingService
    {
        private readonly IRepository<GrowthTracking> _growthTrackingRepository = growthTrackingRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<GrowthTrackingDto> CreateGrowthTrackingAsync(CreateGrowthTrackingDto createGrowthTrackingDto)
        {
            ArgumentNullException.ThrowIfNull(createGrowthTrackingDto);

            var existingTracking = await GetGrowthTrackingByDateAsync(
                createGrowthTrackingDto.StudentId,
                createGrowthTrackingDto.TrackingDate.Date);

            if (existingTracking != null)
            {
                throw new InvalidOperationException($"Growth tracking already exists for student {createGrowthTrackingDto.StudentId} on date {createGrowthTrackingDto.TrackingDate.Date:yyyy-MM-dd}");
            }

            var growthTracking = _mapper.Map<GrowthTracking>(createGrowthTrackingDto);
            growthTracking.OverallGrowth = (growthTracking.AcademicGrowth + growthTracking.StudyHabitGrowth + growthTracking.EmotionalGrowth) / 3;

            await _growthTrackingRepository.AddAsync(growthTracking);
            return _mapper.Map<GrowthTrackingDto>(growthTracking);
        }

        public async Task<GrowthTrackingDto?> GetGrowthTrackingByIdAsync(long id)
        {
            var growthTracking = await _growthTrackingRepository.GetAsync(
                predicate: gt => gt.Id == id,
                disableTracking: false);

            return _mapper.Map<GrowthTrackingDto>(growthTracking);
        }

        public async Task<PaginatedResult<GrowthTrackingDto>> GetGrowthTrackingsAsync(
            ClaimsPrincipal currentUser,
            long? studentId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            decimal? minGrowth = null,
            decimal? maxGrowth = null,
            string? sortBy = "TrackingDate",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<GrowthTracking, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (studentId.HasValue || fromDate.HasValue || toDate.HasValue || minGrowth.HasValue || maxGrowth.HasValue)
            {
                Expression<Func<GrowthTracking, bool>> filterPredicate = gt =>
                    (!studentId.HasValue || gt.StudentId == studentId.Value) &&
                    (!fromDate.HasValue || gt.TrackingDate >= fromDate.Value.Date) &&
                    (!toDate.HasValue || gt.TrackingDate <= toDate.Value.Date) &&
                    (!minGrowth.HasValue || gt.OverallGrowth >= minGrowth.Value) &&
                    (!maxGrowth.HasValue || gt.OverallGrowth <= maxGrowth.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<GrowthTracking>, IOrderedQueryable<GrowthTracking>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _growthTrackingRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var growthTrackingDtos = _mapper.Map<List<GrowthTrackingDto>>(paginatedResult.Data);

            return new PaginatedResult<GrowthTrackingDto>(
                growthTrackingDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<GrowthTracking, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsStudent(user))
            {
                return gt => gt.Student != null && gt.Student.UserId == userId;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return gt => gt.Student != null &&
                           gt.Student.TeacherStudents != null &&
                           gt.Student.TeacherStudents.Any(ts => ts.Teacher != null && ts.Teacher.UserId == userId);
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return gt => gt.Student != null &&
                           gt.Student.TutorStudents != null &&
                           gt.Student.TutorStudents.Any(ts => ts.Tutor != null && ts.Tutor.UserId == userId);
            }
            else if (UserContextHelper.IsParent(user))
            {
                return gt => gt.Student != null &&
                           gt.Student.ParentStudents != null &&
                           gt.Student.ParentStudents.Any(ps => ps.Parent != null && ps.Parent.UserId == userId);
            }
            else if (UserContextHelper.IsAdmin(user))
            {
                return gt => gt.Student != null &&
                           gt.Student.Admin != null &&
                           gt.Student.Admin.UserId == userId;
            }
            else
            {
                return gt => false;
            }
        }

        private Expression<Func<GrowthTracking, bool>> CombinePredicates(
            Expression<Func<GrowthTracking, bool>> left,
            Expression<Func<GrowthTracking, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(GrowthTracking), "gt");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<GrowthTracking, bool>>(combined, parameter);
        }

        private Func<IQueryable<GrowthTracking>, IOrderedQueryable<GrowthTracking>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "trackingdate" => sortDescending
                    ? query => query.OrderByDescending(gt => gt.TrackingDate)
                    : query => query.OrderBy(gt => gt.TrackingDate),
                "academicgrowth" => sortDescending
                    ? query => query.OrderByDescending(gt => gt.AcademicGrowth)
                    : query => query.OrderBy(gt => gt.AcademicGrowth),
                "studyhabitgrowth" => sortDescending
                    ? query => query.OrderByDescending(gt => gt.StudyHabitGrowth)
                    : query => query.OrderBy(gt => gt.StudyHabitGrowth),
                "emotionalgrowth" => sortDescending
                    ? query => query.OrderByDescending(gt => gt.EmotionalGrowth)
                    : query => query.OrderBy(gt => gt.EmotionalGrowth),
                "overallgrowth" => sortDescending
                    ? query => query.OrderByDescending(gt => gt.OverallGrowth)
                    : query => query.OrderBy(gt => gt.OverallGrowth),
                _ => sortDescending
                    ? query => query.OrderByDescending(gt => gt.Id)
                    : query => query.OrderBy(gt => gt.Id)
            };
        }

        public async Task<GrowthTrackingDto> UpdateGrowthTrackingAsync(long id, UpdateGrowthTrackingDto updateGrowthTrackingDto)
        {
            var existingGrowthTracking = await _growthTrackingRepository.GetAsync(
                predicate: gt => gt.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Growth tracking with ID {id} not found");

            _mapper.Map(updateGrowthTrackingDto, existingGrowthTracking);
            existingGrowthTracking.OverallGrowth = (existingGrowthTracking.AcademicGrowth + existingGrowthTracking.StudyHabitGrowth + existingGrowthTracking.EmotionalGrowth) / 3;

            await _growthTrackingRepository.UpdateAsync(existingGrowthTracking);
            return _mapper.Map<GrowthTrackingDto>(existingGrowthTracking);
        }

        public async Task<bool> DeleteGrowthTrackingAsync(long id)
        {
            var growthTracking = await _growthTrackingRepository.GetAsync(
                predicate: gt => gt.Id == id,
                disableTracking: false);

            if (growthTracking == null)
                return false;

            await _growthTrackingRepository.DeleteAsync(growthTracking);
            return true;
        }

        public async Task<int> CountGrowthTrackingsAsync(
            ClaimsPrincipal currentUser,
            long? studentId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<GrowthTracking, bool>> filterPredicate = gt =>
                (!studentId.HasValue || gt.StudentId == studentId.Value) &&
                (!fromDate.HasValue || gt.TrackingDate >= fromDate.Value.Date) &&
                (!toDate.HasValue || gt.TrackingDate <= toDate.Value.Date);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _growthTrackingRepository.CountAsync(predicate);
        }

        public async Task<bool> GrowthTrackingExistsAsync(long id)
        {
            return await _growthTrackingRepository.ExistsAsync(gt => gt.Id == id);
        }

        public async Task<IEnumerable<GrowthTrackingDto>> GetGrowthTrackingsByStudentAsync(long studentId)
        {
            var growthTrackings = await _growthTrackingRepository.GetManyAsync(
                predicate: gt => gt.StudentId == studentId,
                orderBy: query => query.OrderByDescending(gt => gt.TrackingDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<GrowthTrackingDto>>(growthTrackings);
        }

        public async Task<IEnumerable<GrowthTrackingDto>> GetGrowthTrackingsByDateRangeAsync(long studentId, DateTime startDate, DateTime endDate)
        {
            var growthTrackings = await _growthTrackingRepository.GetManyAsync(
                predicate: gt => gt.StudentId == studentId &&
                               gt.TrackingDate >= startDate.Date &&
                               gt.TrackingDate <= endDate.Date,
                orderBy: query => query.OrderBy(gt => gt.TrackingDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<GrowthTrackingDto>>(growthTrackings);
        }

        public async Task<GrowthTrackingDto?> GetLatestGrowthTrackingAsync(long studentId)
        {
            var growthTrackings = await _growthTrackingRepository.GetManyAsync(
                predicate: gt => gt.StudentId == studentId,
                orderBy: query => query.OrderByDescending(gt => gt.TrackingDate),
                disableTracking: true);

            return _mapper.Map<GrowthTrackingDto>(growthTrackings.FirstOrDefault());
        }

        public async Task<GrowthTrackingDto?> GetGrowthTrackingByDateAsync(long studentId, DateTime date)
        {
            var growthTracking = await _growthTrackingRepository.GetAsync(
                predicate: gt => gt.StudentId == studentId && gt.TrackingDate.Date == date.Date,
                disableTracking: true);

            return _mapper.Map<GrowthTrackingDto>(growthTracking);
        }

        public async Task<Dictionary<string, decimal>> GetGrowthTrendsAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            Expression<Func<GrowthTracking, bool>> predicate = gt => gt.StudentId == studentId;

            if (fromDate.HasValue || toDate.HasValue)
            {
                predicate = gt => gt.StudentId == studentId &&
                    (!fromDate.HasValue || gt.TrackingDate >= fromDate.Value.Date) &&
                    (!toDate.HasValue || gt.TrackingDate <= toDate.Value.Date);
            }

            var growthTrackings = await _growthTrackingRepository.GetManyAsync(
                predicate: predicate,
                orderBy: query => query.OrderBy(gt => gt.TrackingDate),
                disableTracking: true);

            if (!growthTrackings.Any())
                return new Dictionary<string, decimal>();

            return new Dictionary<string, decimal>
            {
                ["AverageAcademicGrowth"] = growthTrackings.Average(gt => gt.AcademicGrowth),
                ["AverageStudyHabitGrowth"] = growthTrackings.Average(gt => gt.StudyHabitGrowth),
                ["AverageEmotionalGrowth"] = growthTrackings.Average(gt => gt.EmotionalGrowth),
                ["AverageOverallGrowth"] = growthTrackings.Average(gt => gt.OverallGrowth),
                ["TotalTrackings"] = growthTrackings.Count()
            };
        }

        public async Task<decimal> GetAverageOverallGrowthAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            Expression<Func<GrowthTracking, bool>> predicate = gt => gt.StudentId == studentId;

            if (fromDate.HasValue || toDate.HasValue)
            {
                predicate = gt => gt.StudentId == studentId &&
                    (!fromDate.HasValue || gt.TrackingDate >= fromDate.Value.Date) &&
                    (!toDate.HasValue || gt.TrackingDate <= toDate.Value.Date);
            }

            var growthTrackings = await _growthTrackingRepository.GetManyAsync(
                predicate: predicate,
                disableTracking: true);

            if (!growthTrackings.Any())
                return 0;

            return growthTrackings.Average(gt => gt.OverallGrowth);
        }

        public async Task<IEnumerable<GrowthTrackingDto>> GetRecentGrowthTrackingsAsync(long studentId, int count = 10)
        {
            var growthTrackings = await _growthTrackingRepository.GetManyAsync(
                predicate: gt => gt.StudentId == studentId,
                orderBy: query => query.OrderByDescending(gt => gt.TrackingDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<GrowthTrackingDto>>(growthTrackings.Take(count));
        }
    }
}