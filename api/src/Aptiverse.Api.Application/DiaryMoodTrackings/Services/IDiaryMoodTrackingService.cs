using Aptiverse.Api.Application.DiaryMoodTrackings.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.DiaryMoodTrackings.Services
{
    public interface IDiaryMoodTrackingService
    {
        Task<DiaryMoodTrackingDto> CreateMoodTrackingAsync(CreateDiaryMoodTrackingDto createMoodTrackingDto);
        Task<DiaryMoodTrackingDto?> GetMoodTrackingByIdAsync(long id);

        Task<PaginatedResult<DiaryMoodTrackingDto>> GetMoodTrackingsAsync(
            ClaimsPrincipal currentUser,
            long? studentId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? overallMood = null,
            string? sortBy = "TrackingDate",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20);

        Task<DiaryMoodTrackingDto> UpdateMoodTrackingAsync(long id, UpdateDiaryMoodTrackingDto updateMoodTrackingDto);
        Task<bool> DeleteMoodTrackingAsync(long id);
        Task<int> CountMoodTrackingsAsync(ClaimsPrincipal currentUser,
            long? studentId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);
        Task<bool> MoodTrackingExistsAsync(long id);

        Task<IEnumerable<DiaryMoodTrackingDto>> GetMoodTrackingsByStudentAsync(long studentId);
        Task<IEnumerable<DiaryMoodTrackingDto>> GetMoodTrackingsByDateRangeAsync(long studentId, DateTime startDate, DateTime endDate);
        Task<DiaryMoodTrackingDto?> GetMoodTrackingByDateAsync(long studentId, DateTime date);
        Task<Dictionary<string, int>> GetMoodStatisticsAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<Dictionary<string, double>> GetAverageLevelsAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<DiaryMoodTrackingDto>> GetRecentMoodTrackingsAsync(long studentId, int count = 10);
    }
}