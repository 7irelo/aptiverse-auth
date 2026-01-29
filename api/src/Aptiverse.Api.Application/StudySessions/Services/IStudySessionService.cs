using Aptiverse.Api.Application.StudySessions.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.StudySessions.Services
{
    public interface IStudySessionService
    {
        Task<StudySessionDto> CreateStudySessionAsync(CreateStudySessionDto createStudySessionDto);
        Task<StudySessionDto?> GetStudySessionByIdAsync(long id);
        Task<PaginatedResult<StudySessionDto>> GetStudySessionsAsync(
            long? studentId = null,
            string? subjectId = null,
            string? sessionType = null,
            DateTime? startAfter = null,
            DateTime? startBefore = null,
            int? minDurationMinutes = null,
            int? maxDurationMinutes = null,
            double? minEfficiencyScore = null,
            double? maxEfficiencyScore = null,
            int? minConcentrationLevel = null,
            int? maxConcentrationLevel = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<StudySessionDto> UpdateStudySessionAsync(long id, UpdateStudySessionDto updateStudySessionDto);
        Task<bool> DeleteStudySessionAsync(long id);
        Task<int> CountStudySessionsAsync(long? studentId = null, string? subjectId = null, string? sessionType = null);
        Task<bool> StudySessionExistsAsync(long id);
    }
}