using Aptiverse.Api.Application.Goals.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.Goals.Services
{
    public interface IGoalService
    {
        Task<GoalDto> CreateGoalAsync(CreateGoalDto createGoalDto);
        Task<GoalDto?> GetGoalByIdAsync(long id);

        Task<PaginatedResult<GoalDto>> GetGoalsAsync(
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
            int pageSize = 20);

        Task<GoalDto> UpdateGoalAsync(long id, UpdateGoalDto updateGoalDto);
        Task<bool> DeleteGoalAsync(long id);
        Task<int> CountGoalsAsync(ClaimsPrincipal currentUser,
            long? studentId = null,
            string? goalType = null,
            string? status = null);
        Task<bool> GoalExistsAsync(long id);

        Task<IEnumerable<GoalDto>> GetGoalsByStudentAsync(long studentId);
        Task<IEnumerable<GoalDto>> GetActiveGoalsAsync(long studentId);
        Task<IEnumerable<GoalDto>> GetCompletedGoalsAsync(long studentId);
        Task<IEnumerable<GoalDto>> GetOverdueGoalsAsync(long studentId);
        Task<GoalDto> UpdateGoalProgressAsync(long id, decimal newCurrentValue);
        Task<GoalDto> UpdateGoalStatusAsync(long id, string newStatus);
        Task<decimal> GetStudentOverallProgressAsync(long studentId);
    }
}