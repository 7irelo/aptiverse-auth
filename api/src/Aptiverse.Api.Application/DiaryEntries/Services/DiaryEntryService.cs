using Aptiverse.Api.Application.DiaryEntries.Dtos;
using Aptiverse.Api.Domain.Models.Psychology;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.DiaryEntries.Services
{
    public class DiaryEntryService(
        IRepository<DiaryEntry> diaryEntryRepository,
        IMapper mapper) : IDiaryEntryService
    {
        private readonly IRepository<DiaryEntry> _diaryEntryRepository = diaryEntryRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<DiaryEntryDto> CreateDiaryEntryAsync(CreateDiaryEntryDto createDiaryEntryDto)
        {
            ArgumentNullException.ThrowIfNull(createDiaryEntryDto);

            var diaryEntry = _mapper.Map<DiaryEntry>(createDiaryEntryDto);

            await _diaryEntryRepository.AddAsync(diaryEntry);
            return _mapper.Map<DiaryEntryDto>(diaryEntry);
        }

        public async Task<DiaryEntryDto?> GetDiaryEntryByIdAsync(long id)
        {
            var diaryEntry = await _diaryEntryRepository.GetAsync(
                predicate: d => d.Id == id,
                disableTracking: false);

            return _mapper.Map<DiaryEntryDto>(diaryEntry);
        }

        public async Task<PaginatedResult<DiaryEntryDto>> GetDiaryEntriesAsync(
            ClaimsPrincipal currentUser,
            long? studentId = null,
            string? mood = null,
            string? entryType = null,
            bool? isPrivate = null,
            string? sentiment = null,
            bool? needsFollowUp = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? search = null,
            string? sortBy = "EntryDate",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<DiaryEntry, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (studentId.HasValue || !string.IsNullOrEmpty(mood) || !string.IsNullOrEmpty(entryType) ||
                isPrivate.HasValue || !string.IsNullOrEmpty(sentiment) || needsFollowUp.HasValue ||
                fromDate.HasValue || toDate.HasValue || !string.IsNullOrEmpty(search))
            {
                Expression<Func<DiaryEntry, bool>> filterPredicate = d =>
                    (!studentId.HasValue || d.StudentId == studentId.Value) &&
                    (string.IsNullOrEmpty(mood) || d.Mood == mood) &&
                    (string.IsNullOrEmpty(entryType) || d.EntryType == entryType) &&
                    (!isPrivate.HasValue || d.IsPrivate == isPrivate.Value) &&
                    (string.IsNullOrEmpty(sentiment) || d.SentimentAnalysis == sentiment) &&
                    (!needsFollowUp.HasValue || d.NeedsFollowUp == needsFollowUp.Value) &&
                    (!fromDate.HasValue || d.EntryDate >= fromDate.Value) &&
                    (!toDate.HasValue || d.EntryDate <= toDate.Value) &&
                    (string.IsNullOrEmpty(search) ||
                     d.Title.Contains(search) ||
                     d.Content.Contains(search) ||
                     d.AiInsights.Contains(search));

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<DiaryEntry>, IOrderedQueryable<DiaryEntry>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _diaryEntryRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var diaryEntryDtos = _mapper.Map<List<DiaryEntryDto>>(paginatedResult.Data);

            return new PaginatedResult<DiaryEntryDto>(
                diaryEntryDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<DiaryEntry, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsStudent(user))
            {
                return d => d.Student != null && d.Student.UserId == userId;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return d => d.Student != null &&
                           d.Student.TeacherStudents != null &&
                           d.Student.TeacherStudents.Any(ts => ts.Teacher != null && ts.Teacher.UserId == userId) &&
                           !d.IsPrivate;
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return d => d.Student != null &&
                           d.Student.TutorStudents != null &&
                           d.Student.TutorStudents.Any(ts => ts.Tutor != null && ts.Tutor.UserId == userId) &&
                           !d.IsPrivate;
            }
            else if (UserContextHelper.IsParent(user))
            {
                return d => d.Student != null &&
                           d.Student.ParentStudents != null &&
                           d.Student.ParentStudents.Any(ps => ps.Parent != null && ps.Parent.UserId == userId) &&
                           !d.IsPrivate;
            }
            else if (UserContextHelper.IsAdmin(user))
            {
                return d => d.Student != null &&
                           d.Student.Admin != null &&
                           d.Student.Admin.UserId == userId &&
                           !d.IsPrivate;
            }
            else
            {
                return d => false;
            }
        }

        private Expression<Func<DiaryEntry, bool>> CombinePredicates(
            Expression<Func<DiaryEntry, bool>> left,
            Expression<Func<DiaryEntry, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(DiaryEntry), "d");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<DiaryEntry, bool>>(combined, parameter);
        }

        private Func<IQueryable<DiaryEntry>, IOrderedQueryable<DiaryEntry>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "title" => sortDescending
                    ? query => query.OrderByDescending(d => d.Title)
                    : query => query.OrderBy(d => d.Title),
                "entrydate" => sortDescending
                    ? query => query.OrderByDescending(d => d.EntryDate)
                    : query => query.OrderBy(d => d.EntryDate),
                "moodintensity" => sortDescending
                    ? query => query.OrderByDescending(d => d.MoodIntensity)
                    : query => query.OrderBy(d => d.MoodIntensity),
                "sentimentscore" => sortDescending
                    ? query => query.OrderByDescending(d => d.SentimentScore)
                    : query => query.OrderBy(d => d.SentimentScore),
                "createdat" => sortDescending
                    ? query => query.OrderByDescending(d => d.CreatedAt)
                    : query => query.OrderBy(d => d.CreatedAt),
                _ => sortDescending
                    ? query => query.OrderByDescending(d => d.Id)
                    : query => query.OrderBy(d => d.Id)
            };
        }

        public async Task<DiaryEntryDto> UpdateDiaryEntryAsync(long id, UpdateDiaryEntryDto updateDiaryEntryDto)
        {
            var existingDiaryEntry = await _diaryEntryRepository.GetAsync(
                predicate: d => d.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Diary entry with ID {id} not found");

            _mapper.Map(updateDiaryEntryDto, existingDiaryEntry);
            existingDiaryEntry.UpdatedAt = DateTime.UtcNow;

            await _diaryEntryRepository.UpdateAsync(existingDiaryEntry);
            return _mapper.Map<DiaryEntryDto>(existingDiaryEntry);
        }

        public async Task<bool> DeleteDiaryEntryAsync(long id)
        {
            var diaryEntry = await _diaryEntryRepository.GetAsync(
                predicate: d => d.Id == id,
                disableTracking: false);

            if (diaryEntry == null)
                return false;

            await _diaryEntryRepository.DeleteAsync(diaryEntry);
            return true;
        }

        public async Task<int> CountDiaryEntriesAsync(
            ClaimsPrincipal currentUser,
            long? studentId = null,
            string? mood = null,
            bool? needsFollowUp = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<DiaryEntry, bool>> filterPredicate = d =>
                (!studentId.HasValue || d.StudentId == studentId.Value) &&
                (string.IsNullOrEmpty(mood) || d.Mood == mood) &&
                (!needsFollowUp.HasValue || d.NeedsFollowUp == needsFollowUp.Value);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _diaryEntryRepository.CountAsync(predicate);
        }

        public async Task<bool> DiaryEntryExistsAsync(long id)
        {
            return await _diaryEntryRepository.ExistsAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<DiaryEntryDto>> GetDiaryEntriesByStudentAsync(long studentId)
        {
            var diaryEntries = await _diaryEntryRepository.GetManyAsync(
                predicate: d => d.StudentId == studentId,
                orderBy: query => query.OrderByDescending(d => d.EntryDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<DiaryEntryDto>>(diaryEntries);
        }

        public async Task<IEnumerable<DiaryEntryDto>> GetRecentDiaryEntriesAsync(long studentId, int count = 10)
        {
            var diaryEntries = await _diaryEntryRepository.GetManyAsync(
                predicate: d => d.StudentId == studentId,
                orderBy: query => query.OrderByDescending(d => d.EntryDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<DiaryEntryDto>>(diaryEntries.Take(count));
        }

        public async Task<IEnumerable<DiaryEntryDto>> GetDiaryEntriesNeedingFollowUpAsync(long studentId)
        {
            var diaryEntries = await _diaryEntryRepository.GetManyAsync(
                predicate: d => d.StudentId == studentId && d.NeedsFollowUp,
                orderBy: query => query.OrderByDescending(d => d.EntryDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<DiaryEntryDto>>(diaryEntries);
        }

        public async Task<Dictionary<string, int>> GetMoodStatisticsAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            Expression<Func<DiaryEntry, bool>> predicate = d => d.StudentId == studentId;

            if (fromDate.HasValue || toDate.HasValue)
            {
                predicate = d => d.StudentId == studentId &&
                    (!fromDate.HasValue || d.EntryDate >= fromDate.Value) &&
                    (!toDate.HasValue || d.EntryDate <= toDate.Value);
            }

            var diaryEntries = await _diaryEntryRepository.GetManyAsync(
                predicate: predicate,
                disableTracking: true);

            return diaryEntries
                .GroupBy(d => d.Mood)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<string, int>> GetEntryTypeStatisticsAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            Expression<Func<DiaryEntry, bool>> predicate = d => d.StudentId == studentId;

            if (fromDate.HasValue || toDate.HasValue)
            {
                predicate = d => d.StudentId == studentId &&
                    (!fromDate.HasValue || d.EntryDate >= fromDate.Value) &&
                    (!toDate.HasValue || d.EntryDate <= toDate.Value);
            }

            var diaryEntries = await _diaryEntryRepository.GetManyAsync(
                predicate: predicate,
                disableTracking: true);

            return diaryEntries
                .GroupBy(d => d.EntryType)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<double> GetAverageMoodIntensityAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            Expression<Func<DiaryEntry, bool>> predicate = d => d.StudentId == studentId;

            if (fromDate.HasValue || toDate.HasValue)
            {
                predicate = d => d.StudentId == studentId &&
                    (!fromDate.HasValue || d.EntryDate >= fromDate.Value) &&
                    (!toDate.HasValue || d.EntryDate <= toDate.Value);
            }

            var diaryEntries = await _diaryEntryRepository.GetManyAsync(
                predicate: predicate,
                disableTracking: true);

            if (!diaryEntries.Any())
                return 0;

            return diaryEntries.Average(d => d.MoodIntensity);
        }
    }
}