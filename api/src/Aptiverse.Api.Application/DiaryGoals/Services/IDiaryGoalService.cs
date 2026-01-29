using Aptiverse.Api.Application.DiaryGoals.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.DiaryGoals.Services
{
    public interface IDiaryGoalService
    {
        Task<DiaryGoalDto> CreateDiaryGoalAsync(CreateDiaryGoalDto createDiaryGoalDto);
        Task<DiaryGoalDto?> GetDiaryGoalByIdAsync(long id);

        Task<PaginatedResult<DiaryGoalDto>> GetDiaryGoalsAsync(
            ClaimsPrincipal currentUser,
            long? diaryEntryId = null,
            long? goalId = null,
            string? connectionType = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);

        Task<DiaryGoalDto> UpdateDiaryGoalAsync(long id, UpdateDiaryGoalDto updateDiaryGoalDto);
        Task<bool> DeleteDiaryGoalAsync(long id);
        Task<int> CountDiaryGoalsAsync(ClaimsPrincipal currentUser,
            long? diaryEntryId = null,
            long? goalId = null,
            string? connectionType = null);
        Task<bool> DiaryGoalExistsAsync(long id);

        Task<IEnumerable<DiaryGoalDto>> GetDiaryGoalsByDiaryEntryAsync(long diaryEntryId);
        Task<IEnumerable<DiaryGoalDto>> GetDiaryGoalsByGoalAsync(long goalId);
        Task<IEnumerable<DiaryGoalDto>> GetDiaryGoalsByStudentAsync(long studentId);
        Task<bool> ExistsAsync(long diaryEntryId, long goalId);
    }
}