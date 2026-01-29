using Aptiverse.Api.Application.DiaryGoals.Dtos;
using Aptiverse.Api.Domain.Models.Psychology;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.DiaryGoals.Services
{
    public class DiaryGoalService(
        IRepository<DiaryGoal> diaryGoalRepository,
        IMapper mapper) : IDiaryGoalService
    {
        private readonly IRepository<DiaryGoal> _diaryGoalRepository = diaryGoalRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<DiaryGoalDto> CreateDiaryGoalAsync(CreateDiaryGoalDto createDiaryGoalDto)
        {
            ArgumentNullException.ThrowIfNull(createDiaryGoalDto);

            var diaryGoal = _mapper.Map<DiaryGoal>(createDiaryGoalDto);

            await _diaryGoalRepository.AddAsync(diaryGoal);
            return _mapper.Map<DiaryGoalDto>(diaryGoal);
        }

        public async Task<DiaryGoalDto?> GetDiaryGoalByIdAsync(long id)
        {
            var diaryGoal = await _diaryGoalRepository.GetAsync(
                predicate: dg => dg.Id == id,
                disableTracking: false);

            return _mapper.Map<DiaryGoalDto>(diaryGoal);
        }

        public async Task<PaginatedResult<DiaryGoalDto>> GetDiaryGoalsAsync(
            ClaimsPrincipal currentUser,
            long? diaryEntryId = null,
            long? goalId = null,
            string? connectionType = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<DiaryGoal, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (diaryEntryId.HasValue || goalId.HasValue || !string.IsNullOrEmpty(connectionType))
            {
                Expression<Func<DiaryGoal, bool>> filterPredicate = dg =>
                    (!diaryEntryId.HasValue || dg.DiaryEntryId == diaryEntryId.Value) &&
                    (!goalId.HasValue || dg.GoalId == goalId.Value) &&
                    (string.IsNullOrEmpty(connectionType) || dg.ConnectionType == connectionType);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<DiaryGoal>, IOrderedQueryable<DiaryGoal>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _diaryGoalRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var diaryGoalDtos = _mapper.Map<List<DiaryGoalDto>>(paginatedResult.Data);

            return new PaginatedResult<DiaryGoalDto>(
                diaryGoalDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<DiaryGoal, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsStudent(user))
            {
                return dg => dg.DiaryEntry != null &&
                            dg.DiaryEntry.Student != null &&
                            dg.DiaryEntry.Student.UserId == userId;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return dg => dg.DiaryEntry != null &&
                            dg.DiaryEntry.Student != null &&
                            dg.DiaryEntry.Student.TeacherStudents != null &&
                            dg.DiaryEntry.Student.TeacherStudents.Any(ts => ts.Teacher != null && ts.Teacher.UserId == userId) &&
                            !dg.DiaryEntry.IsPrivate;
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return dg => dg.DiaryEntry != null &&
                            dg.DiaryEntry.Student != null &&
                            dg.DiaryEntry.Student.TutorStudents != null &&
                            dg.DiaryEntry.Student.TutorStudents.Any(ts => ts.Tutor != null && ts.Tutor.UserId == userId) &&
                            !dg.DiaryEntry.IsPrivate;
            }
            else if (UserContextHelper.IsParent(user))
            {
                return dg => dg.DiaryEntry != null &&
                            dg.DiaryEntry.Student != null &&
                            dg.DiaryEntry.Student.ParentStudents != null &&
                            dg.DiaryEntry.Student.ParentStudents.Any(ps => ps.Parent != null && ps.Parent.UserId == userId) &&
                            !dg.DiaryEntry.IsPrivate;
            }
            else if (UserContextHelper.IsAdmin(user))
            {
                return dg => dg.DiaryEntry != null &&
                            dg.DiaryEntry.Student != null &&
                            dg.DiaryEntry.Student.Admin != null &&
                            dg.DiaryEntry.Student.Admin.UserId == userId &&
                            !dg.DiaryEntry.IsPrivate;
            }
            else
            {
                return dg => false;
            }
        }

        private Expression<Func<DiaryGoal, bool>> CombinePredicates(
            Expression<Func<DiaryGoal, bool>> left,
            Expression<Func<DiaryGoal, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(DiaryGoal), "dg");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<DiaryGoal, bool>>(combined, parameter);
        }

        private Func<IQueryable<DiaryGoal>, IOrderedQueryable<DiaryGoal>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "connectiontype" => sortDescending
                    ? query => query.OrderByDescending(dg => dg.ConnectionType)
                    : query => query.OrderBy(dg => dg.ConnectionType),
                _ => sortDescending
                    ? query => query.OrderByDescending(dg => dg.Id)
                    : query => query.OrderBy(dg => dg.Id)
            };
        }

        public async Task<DiaryGoalDto> UpdateDiaryGoalAsync(long id, UpdateDiaryGoalDto updateDiaryGoalDto)
        {
            var existingDiaryGoal = await _diaryGoalRepository.GetAsync(
                predicate: dg => dg.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Diary goal connection with ID {id} not found");

            _mapper.Map(updateDiaryGoalDto, existingDiaryGoal);

            await _diaryGoalRepository.UpdateAsync(existingDiaryGoal);
            return _mapper.Map<DiaryGoalDto>(existingDiaryGoal);
        }

        public async Task<bool> DeleteDiaryGoalAsync(long id)
        {
            var diaryGoal = await _diaryGoalRepository.GetAsync(
                predicate: dg => dg.Id == id,
                disableTracking: false);

            if (diaryGoal == null)
                return false;

            await _diaryGoalRepository.DeleteAsync(diaryGoal);
            return true;
        }

        public async Task<int> CountDiaryGoalsAsync(
            ClaimsPrincipal currentUser,
            long? diaryEntryId = null,
            long? goalId = null,
            string? connectionType = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<DiaryGoal, bool>> filterPredicate = dg =>
                (!diaryEntryId.HasValue || dg.DiaryEntryId == diaryEntryId.Value) &&
                (!goalId.HasValue || dg.GoalId == goalId.Value) &&
                (string.IsNullOrEmpty(connectionType) || dg.ConnectionType == connectionType);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _diaryGoalRepository.CountAsync(predicate);
        }

        public async Task<bool> DiaryGoalExistsAsync(long id)
        {
            return await _diaryGoalRepository.ExistsAsync(dg => dg.Id == id);
        }

        public async Task<IEnumerable<DiaryGoalDto>> GetDiaryGoalsByDiaryEntryAsync(long diaryEntryId)
        {
            var diaryGoals = await _diaryGoalRepository.GetManyAsync(
                predicate: dg => dg.DiaryEntryId == diaryEntryId,
                orderBy: query => query.OrderBy(dg => dg.ConnectionType),
                disableTracking: true);

            return _mapper.Map<IEnumerable<DiaryGoalDto>>(diaryGoals);
        }

        public async Task<IEnumerable<DiaryGoalDto>> GetDiaryGoalsByGoalAsync(long goalId)
        {
            var diaryGoals = await _diaryGoalRepository.GetManyAsync(
                predicate: dg => dg.GoalId == goalId,
                orderBy: query => query.OrderByDescending(dg => dg.DiaryEntry != null ? dg.DiaryEntry.EntryDate : DateTime.MinValue),
                disableTracking: true);

            return _mapper.Map<IEnumerable<DiaryGoalDto>>(diaryGoals);
        }

        public async Task<IEnumerable<DiaryGoalDto>> GetDiaryGoalsByStudentAsync(long studentId)
        {
            var diaryGoals = await _diaryGoalRepository.GetManyAsync(
                predicate: dg => dg.DiaryEntry != null && dg.DiaryEntry.StudentId == studentId,
                orderBy: query => query.OrderByDescending(dg => dg.DiaryEntry != null ? dg.DiaryEntry.EntryDate : DateTime.MinValue),
                disableTracking: true);

            return _mapper.Map<IEnumerable<DiaryGoalDto>>(diaryGoals);
        }

        public async Task<bool> ExistsAsync(long diaryEntryId, long goalId)
        {
            return await _diaryGoalRepository.ExistsAsync(dg =>
                dg.DiaryEntryId == diaryEntryId && dg.GoalId == goalId);
        }
    }
}