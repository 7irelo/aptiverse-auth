using Aptiverse.Api.Application.DiaryMoodTrackings.Dtos;
using Aptiverse.Api.Domain.Models.Psychology;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.DiaryMoodTrackings.Services
{
    public class DiaryMoodTrackingService(
        IRepository<DiaryMoodTracking> moodTrackingRepository,
        IMapper mapper) : IDiaryMoodTrackingService
    {
        private readonly IRepository<DiaryMoodTracking> _moodTrackingRepository = moodTrackingRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<DiaryMoodTrackingDto> CreateMoodTrackingAsync(CreateDiaryMoodTrackingDto createMoodTrackingDto)
        {
            ArgumentNullException.ThrowIfNull(createMoodTrackingDto);

            var existingTracking = await GetMoodTrackingByDateAsync(
                createMoodTrackingDto.StudentId,
                createMoodTrackingDto.TrackingDate.Date);

            if (existingTracking != null)
            {
                throw new InvalidOperationException($"Mood tracking already exists for student {createMoodTrackingDto.StudentId} on date {createMoodTrackingDto.TrackingDate.Date:yyyy-MM-dd}");
            }

            var moodTracking = _mapper.Map<DiaryMoodTracking>(createMoodTrackingDto);

            await _moodTrackingRepository.AddAsync(moodTracking);
            return _mapper.Map<DiaryMoodTrackingDto>(moodTracking);
        }

        public async Task<DiaryMoodTrackingDto?> GetMoodTrackingByIdAsync(long id)
        {
            var moodTracking = await _moodTrackingRepository.GetAsync(
                predicate: mt => mt.Id == id,
                disableTracking: false);

            return _mapper.Map<DiaryMoodTrackingDto>(moodTracking);
        }

        public async Task<PaginatedResult<DiaryMoodTrackingDto>> GetMoodTrackingsAsync(
            ClaimsPrincipal currentUser,
            long? studentId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? overallMood = null,
            string? sortBy = "TrackingDate",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<DiaryMoodTracking, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (studentId.HasValue || fromDate.HasValue || toDate.HasValue || !string.IsNullOrEmpty(overallMood))
            {
                Expression<Func<DiaryMoodTracking, bool>> filterPredicate = mt =>
                    (!studentId.HasValue || mt.StudentId == studentId.Value) &&
                    (!fromDate.HasValue || mt.TrackingDate >= fromDate.Value.Date) &&
                    (!toDate.HasValue || mt.TrackingDate <= toDate.Value.Date) &&
                    (string.IsNullOrEmpty(overallMood) || mt.OverallMood == overallMood);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<DiaryMoodTracking>, IOrderedQueryable<DiaryMoodTracking>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _moodTrackingRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var moodTrackingDtos = _mapper.Map<List<DiaryMoodTrackingDto>>(paginatedResult.Data);

            return new PaginatedResult<DiaryMoodTrackingDto>(
                moodTrackingDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<DiaryMoodTracking, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsStudent(user))
            {
                return mt => mt.Student != null && mt.Student.UserId == userId;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return mt => mt.Student != null &&
                           mt.Student.TeacherStudents != null &&
                           mt.Student.TeacherStudents.Any(ts => ts.Teacher != null && ts.Teacher.UserId == userId);
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return mt => mt.Student != null &&
                           mt.Student.TutorStudents != null &&
                           mt.Student.TutorStudents.Any(ts => ts.Tutor != null && ts.Tutor.UserId == userId);
            }
            else if (UserContextHelper.IsParent(user))
            {
                return mt => mt.Student != null &&
                           mt.Student.ParentStudents != null &&
                           mt.Student.ParentStudents.Any(ps => ps.Parent != null && ps.Parent.UserId == userId);
            }
            else if (UserContextHelper.IsAdmin(user))
            {
                return mt => mt.Student != null &&
                           mt.Student.Admin != null &&
                           mt.Student.Admin.UserId == userId;
            }
            else
            {
                return mt => false;
            }
        }

        private Expression<Func<DiaryMoodTracking, bool>> CombinePredicates(
            Expression<Func<DiaryMoodTracking, bool>> left,
            Expression<Func<DiaryMoodTracking, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(DiaryMoodTracking), "mt");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<DiaryMoodTracking, bool>>(combined, parameter);
        }

        private Func<IQueryable<DiaryMoodTracking>, IOrderedQueryable<DiaryMoodTracking>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "trackingdate" => sortDescending
                    ? query => query.OrderByDescending(mt => mt.TrackingDate)
                    : query => query.OrderBy(mt => mt.TrackingDate),
                "energylevel" => sortDescending
                    ? query => query.OrderByDescending(mt => mt.EnergyLevel)
                    : query => query.OrderBy(mt => mt.EnergyLevel),
                "stresslevel" => sortDescending
                    ? query => query.OrderByDescending(mt => mt.StressLevel)
                    : query => query.OrderBy(mt => mt.StressLevel),
                "motivationlevel" => sortDescending
                    ? query => query.OrderByDescending(mt => mt.MotivationLevel)
                    : query => query.OrderBy(mt => mt.MotivationLevel),
                "overallmood" => sortDescending
                    ? query => query.OrderByDescending(mt => mt.OverallMood)
                    : query => query.OrderBy(mt => mt.OverallMood),
                _ => sortDescending
                    ? query => query.OrderByDescending(mt => mt.Id)
                    : query => query.OrderBy(mt => mt.Id)
            };
        }

        public async Task<DiaryMoodTrackingDto> UpdateMoodTrackingAsync(long id, UpdateDiaryMoodTrackingDto updateMoodTrackingDto)
        {
            var existingMoodTracking = await _moodTrackingRepository.GetAsync(
                predicate: mt => mt.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Mood tracking with ID {id} not found");

            _mapper.Map(updateMoodTrackingDto, existingMoodTracking);

            await _moodTrackingRepository.UpdateAsync(existingMoodTracking);
            return _mapper.Map<DiaryMoodTrackingDto>(existingMoodTracking);
        }

        public async Task<bool> DeleteMoodTrackingAsync(long id)
        {
            var moodTracking = await _moodTrackingRepository.GetAsync(
                predicate: mt => mt.Id == id,
                disableTracking: false);

            if (moodTracking == null)
                return false;

            await _moodTrackingRepository.DeleteAsync(moodTracking);
            return true;
        }

        public async Task<int> CountMoodTrackingsAsync(
            ClaimsPrincipal currentUser,
            long? studentId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<DiaryMoodTracking, bool>> filterPredicate = mt =>
                (!studentId.HasValue || mt.StudentId == studentId.Value) &&
                (!fromDate.HasValue || mt.TrackingDate >= fromDate.Value.Date) &&
                (!toDate.HasValue || mt.TrackingDate <= toDate.Value.Date);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _moodTrackingRepository.CountAsync(predicate);
        }

        public async Task<bool> MoodTrackingExistsAsync(long id)
        {
            return await _moodTrackingRepository.ExistsAsync(mt => mt.Id == id);
        }

        public async Task<IEnumerable<DiaryMoodTrackingDto>> GetMoodTrackingsByStudentAsync(long studentId)
        {
            var moodTrackings = await _moodTrackingRepository.GetManyAsync(
                predicate: mt => mt.StudentId == studentId,
                orderBy: query => query.OrderByDescending(mt => mt.TrackingDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<DiaryMoodTrackingDto>>(moodTrackings);
        }

        public async Task<IEnumerable<DiaryMoodTrackingDto>> GetMoodTrackingsByDateRangeAsync(long studentId, DateTime startDate, DateTime endDate)
        {
            var moodTrackings = await _moodTrackingRepository.GetManyAsync(
                predicate: mt => mt.StudentId == studentId &&
                               mt.TrackingDate >= startDate.Date &&
                               mt.TrackingDate <= endDate.Date,
                orderBy: query => query.OrderBy(mt => mt.TrackingDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<DiaryMoodTrackingDto>>(moodTrackings);
        }

        public async Task<DiaryMoodTrackingDto?> GetMoodTrackingByDateAsync(long studentId, DateTime date)
        {
            var moodTracking = await _moodTrackingRepository.GetAsync(
                predicate: mt => mt.StudentId == studentId && mt.TrackingDate.Date == date.Date,
                disableTracking: true);

            return _mapper.Map<DiaryMoodTrackingDto>(moodTracking);
        }

        public async Task<Dictionary<string, int>> GetMoodStatisticsAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            Expression<Func<DiaryMoodTracking, bool>> predicate = mt => mt.StudentId == studentId;

            if (fromDate.HasValue || toDate.HasValue)
            {
                predicate = mt => mt.StudentId == studentId &&
                    (!fromDate.HasValue || mt.TrackingDate >= fromDate.Value.Date) &&
                    (!toDate.HasValue || mt.TrackingDate <= toDate.Value.Date);
            }

            var moodTrackings = await _moodTrackingRepository.GetManyAsync(
                predicate: predicate,
                disableTracking: true);

            return moodTrackings
                .GroupBy(mt => mt.OverallMood)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<string, double>> GetAverageLevelsAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            Expression<Func<DiaryMoodTracking, bool>> predicate = mt => mt.StudentId == studentId;

            if (fromDate.HasValue || toDate.HasValue)
            {
                predicate = mt => mt.StudentId == studentId &&
                    (!fromDate.HasValue || mt.TrackingDate >= fromDate.Value.Date) &&
                    (!toDate.HasValue || mt.TrackingDate <= toDate.Value.Date);
            }

            var moodTrackings = await _moodTrackingRepository.GetManyAsync(
                predicate: predicate,
                disableTracking: true);

            if (!moodTrackings.Any())
                return new Dictionary<string, double>();

            return new Dictionary<string, double>
            {
                ["AverageEnergyLevel"] = moodTrackings.Average(mt => mt.EnergyLevel),
                ["AverageStressLevel"] = moodTrackings.Average(mt => mt.StressLevel),
                ["AverageMotivationLevel"] = moodTrackings.Average(mt => mt.MotivationLevel)
            };
        }

        public async Task<IEnumerable<DiaryMoodTrackingDto>> GetRecentMoodTrackingsAsync(long studentId, int count = 10)
        {
            var moodTrackings = await _moodTrackingRepository.GetManyAsync(
                predicate: mt => mt.StudentId == studentId,
                orderBy: query => query.OrderByDescending(mt => mt.TrackingDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<DiaryMoodTrackingDto>>(moodTrackings.Take(count));
        }
    }
}