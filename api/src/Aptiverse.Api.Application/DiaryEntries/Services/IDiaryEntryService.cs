using Aptiverse.Api.Application.DiaryEntries.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.DiaryEntries.Services
{
    public interface IDiaryEntryService
    {
        Task<DiaryEntryDto> CreateDiaryEntryAsync(CreateDiaryEntryDto createDiaryEntryDto);
        Task<DiaryEntryDto?> GetDiaryEntryByIdAsync(long id);

        Task<PaginatedResult<DiaryEntryDto>> GetDiaryEntriesAsync(
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
            int pageSize = 20);

        Task<DiaryEntryDto> UpdateDiaryEntryAsync(long id, UpdateDiaryEntryDto updateDiaryEntryDto);
        Task<bool> DeleteDiaryEntryAsync(long id);
        Task<int> CountDiaryEntriesAsync(ClaimsPrincipal currentUser,
            long? studentId = null,
            string? mood = null,
            bool? needsFollowUp = null);
        Task<bool> DiaryEntryExistsAsync(long id);

        Task<IEnumerable<DiaryEntryDto>> GetDiaryEntriesByStudentAsync(long studentId);
        Task<IEnumerable<DiaryEntryDto>> GetRecentDiaryEntriesAsync(long studentId, int count = 10);
        Task<IEnumerable<DiaryEntryDto>> GetDiaryEntriesNeedingFollowUpAsync(long studentId);
        Task<Dictionary<string, int>> GetMoodStatisticsAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<Dictionary<string, int>> GetEntryTypeStatisticsAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<double> GetAverageMoodIntensityAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null);
    }
}