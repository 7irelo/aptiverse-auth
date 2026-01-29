using Aptiverse.Api.Application.GoalMilestones.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.GoalMilestones.Services
{
    public interface IGoalMilestoneService
    {
        Task<GoalMilestoneDto> CreateMilestoneAsync(CreateGoalMilestoneDto createMilestoneDto);
        Task<GoalMilestoneDto?> GetMilestoneByIdAsync(long id);

        Task<PaginatedResult<GoalMilestoneDto>> GetMilestonesAsync(
            ClaimsPrincipal currentUser,
            long? goalId = null,
            bool? isCompleted = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);

        Task<GoalMilestoneDto> UpdateMilestoneAsync(long id, UpdateGoalMilestoneDto updateMilestoneDto);
        Task<bool> DeleteMilestoneAsync(long id);
        Task<int> CountMilestonesAsync(ClaimsPrincipal currentUser,
            long? goalId = null,
            bool? isCompleted = null);
        Task<bool> MilestoneExistsAsync(long id);

        Task<IEnumerable<GoalMilestoneDto>> GetMilestonesByGoalAsync(long goalId);
        Task<IEnumerable<GoalMilestoneDto>> GetCompletedMilestonesAsync(long goalId);
        Task<IEnumerable<GoalMilestoneDto>> GetPendingMilestonesAsync(long goalId);
        Task<GoalMilestoneDto> MarkMilestoneCompleteAsync(long id);
        Task<GoalMilestoneDto> MarkMilestoneIncompleteAsync(long id);
        Task<int> GetTotalRewardPointsAsync(long goalId);
    }
}