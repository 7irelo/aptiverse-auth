using Aptiverse.Api.Application.Goals.Dtos;
using Aptiverse.Api.Domain.Models.Goals;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.Goals.Services
{
    public class GoalService(
        IRepository<Goal> goalRepository,
        IMapper mapper) : IGoalService
    {
        private readonly IRepository<Goal> _goalRepository = goalRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<GoalDto> CreateGoalAsync(CreateGoalDto createGoalDto)
        {
            ArgumentNullException.ThrowIfNull(createGoalDto);

            var goal = _mapper.Map<Goal>(createGoalDto);

            await _goalRepository.AddAsync(goal);
            return _mapper.Map<GoalDto>(goal);
        }

        public async Task<GoalDto?> GetGoalByIdAsync(long id)
        {
            var goal = await _goalRepository.GetAsync(
                predicate: g => g.Id == id,
                disableTracking: false);

            return _mapper.Map<GoalDto>(goal);
        }

        public async Task<PaginatedResult<GoalDto>> GetGoalsAsync(
            ClaimsPrincipal currentUser,
            long? studentId = null,
            string? goalType = null,
            string? subjectId = null,
            string? status = null,
            int? priority = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? sortBy = "TargetDate",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<Goal, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (studentId.HasValue || !string.IsNullOrEmpty(goalType) || !string.IsNullOrEmpty(subjectId) ||
                !string.IsNullOrEmpty(status) || priority.HasValue || startDate.HasValue || endDate.HasValue)
            {
                Expression<Func<Goal, bool>> filterPredicate = g =>
                    (!studentId.HasValue || g.StudentId == studentId.Value) &&
                    (string.IsNullOrEmpty(goalType) || g.GoalType == goalType) &&
                    (string.IsNullOrEmpty(subjectId) || g.SubjectId == subjectId) &&
                    (string.IsNullOrEmpty(status) || g.Status == status) &&
                    (!priority.HasValue || g.Priority == priority.Value) &&
                    (!startDate.HasValue || g.StartDate >= startDate.Value) &&
                    (!endDate.HasValue || g.TargetDate <= endDate.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<Goal>, IOrderedQueryable<Goal>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _goalRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var goalDtos = _mapper.Map<List<GoalDto>>(paginatedResult.Data);

            return new PaginatedResult<GoalDto>(
                goalDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<Goal, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsStudent(user))
            {
                return g => g.Student != null && g.Student.UserId == userId;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return g => g.Student != null &&
                           g.Student.TeacherStudents != null &&
                           g.Student.TeacherStudents.Any(ts => ts.Teacher != null && ts.Teacher.UserId == userId);
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return g => g.Student != null &&
                           g.Student.TutorStudents != null &&
                           g.Student.TutorStudents.Any(ts => ts.Tutor != null && ts.Tutor.UserId == userId);
            }
            else if (UserContextHelper.IsParent(user))
            {
                return g => g.Student != null &&
                           g.Student.ParentStudents != null &&
                           g.Student.ParentStudents.Any(ps => ps.Parent != null && ps.Parent.UserId == userId);
            }
            else if (UserContextHelper.IsAdmin(user))
            {
                return g => g.Student != null &&
                           g.Student.Admin != null &&
                           g.Student.Admin.UserId == userId;
            }
            else
            {
                return g => false;
            }
        }

        private Expression<Func<Goal, bool>> CombinePredicates(
            Expression<Func<Goal, bool>> left,
            Expression<Func<Goal, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(Goal), "g");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<Goal, bool>>(combined, parameter);
        }

        private Func<IQueryable<Goal>, IOrderedQueryable<Goal>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "title" => sortDescending
                    ? query => query.OrderByDescending(g => g.Title)
                    : query => query.OrderBy(g => g.Title),
                "targetdate" => sortDescending
                    ? query => query.OrderByDescending(g => g.TargetDate)
                    : query => query.OrderBy(g => g.TargetDate),
                "startdate" => sortDescending
                    ? query => query.OrderByDescending(g => g.StartDate)
                    : query => query.OrderBy(g => g.StartDate),
                "priority" => sortDescending
                    ? query => query.OrderByDescending(g => g.Priority)
                    : query => query.OrderBy(g => g.Priority),
                "progresspercentage" => sortDescending
                    ? query => query.OrderByDescending(g => g.ProgressPercentage)
                    : query => query.OrderBy(g => g.ProgressPercentage),
                "difficultyweight" => sortDescending
                    ? query => query.OrderByDescending(g => g.DifficultyWeight)
                    : query => query.OrderBy(g => g.DifficultyWeight),
                "createdat" => sortDescending
                    ? query => query.OrderByDescending(g => g.CreatedAt)
                    : query => query.OrderBy(g => g.CreatedAt),
                _ => sortDescending
                    ? query => query.OrderByDescending(g => g.Id)
                    : query => query.OrderBy(g => g.Id)
            };
        }

        public async Task<GoalDto> UpdateGoalAsync(long id, UpdateGoalDto updateGoalDto)
        {
            var existingGoal = await _goalRepository.GetAsync(
                predicate: g => g.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Goal with ID {id} not found");

            _mapper.Map(updateGoalDto, existingGoal);
            existingGoal.UpdatedAt = DateTime.UtcNow;

            await _goalRepository.UpdateAsync(existingGoal);
            return _mapper.Map<GoalDto>(existingGoal);
        }

        public async Task<bool> DeleteGoalAsync(long id)
        {
            var goal = await _goalRepository.GetAsync(
                predicate: g => g.Id == id,
                disableTracking: false);

            if (goal == null)
                return false;

            await _goalRepository.DeleteAsync(goal);
            return true;
        }

        public async Task<int> CountGoalsAsync(
            ClaimsPrincipal currentUser,
            long? studentId = null,
            string? goalType = null,
            string? status = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<Goal, bool>> filterPredicate = g =>
                (!studentId.HasValue || g.StudentId == studentId.Value) &&
                (string.IsNullOrEmpty(goalType) || g.GoalType == goalType) &&
                (string.IsNullOrEmpty(status) || g.Status == status);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _goalRepository.CountAsync(predicate);
        }

        public async Task<bool> GoalExistsAsync(long id)
        {
            return await _goalRepository.ExistsAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<GoalDto>> GetGoalsByStudentAsync(long studentId)
        {
            var goals = await _goalRepository.GetManyAsync(
                predicate: g => g.StudentId == studentId,
                orderBy: query => query.OrderByDescending(g => g.Priority).ThenBy(g => g.TargetDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<GoalDto>>(goals);
        }

        public async Task<IEnumerable<GoalDto>> GetActiveGoalsAsync(long studentId)
        {
            var goals = await _goalRepository.GetManyAsync(
                predicate: g => g.StudentId == studentId && g.Status == "InProgress",
                orderBy: query => query.OrderByDescending(g => g.Priority).ThenBy(g => g.TargetDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<GoalDto>>(goals);
        }

        public async Task<IEnumerable<GoalDto>> GetCompletedGoalsAsync(long studentId)
        {
            var goals = await _goalRepository.GetManyAsync(
                predicate: g => g.StudentId == studentId && g.Status == "Completed",
                orderBy: query => query.OrderByDescending(g => g.TargetDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<GoalDto>>(goals);
        }

        public async Task<IEnumerable<GoalDto>> GetOverdueGoalsAsync(long studentId)
        {
            var now = DateTime.UtcNow;
            var goals = await _goalRepository.GetManyAsync(
                predicate: g => g.StudentId == studentId &&
                               g.Status != "Completed" &&
                               g.TargetDate < now,
                orderBy: query => query.OrderBy(g => g.TargetDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<GoalDto>>(goals);
        }

        public async Task<GoalDto> UpdateGoalProgressAsync(long id, decimal newCurrentValue)
        {
            var existingGoal = await _goalRepository.GetAsync(
                predicate: g => g.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Goal with ID {id} not found");

            existingGoal.CurrentValue = newCurrentValue;
            existingGoal.UpdatedAt = DateTime.UtcNow;

            if (newCurrentValue >= existingGoal.TargetValue)
            {
                existingGoal.Status = "Completed";
            }
            else if (newCurrentValue > 0)
            {
                existingGoal.Status = "InProgress";
            }

            await _goalRepository.UpdateAsync(existingGoal);
            return _mapper.Map<GoalDto>(existingGoal);
        }

        public async Task<GoalDto> UpdateGoalStatusAsync(long id, string newStatus)
        {
            var existingGoal = await _goalRepository.GetAsync(
                predicate: g => g.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Goal with ID {id} not found");

            existingGoal.Status = newStatus;
            existingGoal.UpdatedAt = DateTime.UtcNow;

            await _goalRepository.UpdateAsync(existingGoal);
            return _mapper.Map<GoalDto>(existingGoal);
        }

        public async Task<decimal> GetStudentOverallProgressAsync(long studentId)
        {
            var goals = await _goalRepository.GetManyAsync(
                predicate: g => g.StudentId == studentId && g.TargetValue > 0,
                disableTracking: true);

            if (!goals.Any())
                return 0;

            var totalProgress = goals.Sum(g => g.ProgressPercentage);
            return totalProgress / goals.Count();
        }
    }
}