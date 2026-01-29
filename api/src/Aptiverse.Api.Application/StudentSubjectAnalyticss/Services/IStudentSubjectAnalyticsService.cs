using Aptiverse.Api.Application.StudentSubjectAnalyticss.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.StudentSubjectAnalyticss.Services
{
    public interface IStudentSubjectAnalyticsService
    {
        Task<StudentSubjectAnalyticsDto> CreateStudentSubjectAnalyticsAsync(CreateStudentSubjectAnalyticsDto createStudentSubjectAnalyticsDto);
        Task<StudentSubjectAnalyticsDto?> GetStudentSubjectAnalyticsByIdAsync(long id);
        Task<StudentSubjectAnalyticsDto?> GetStudentSubjectAnalyticsByStudentSubjectIdAsync(long studentSubjectId);
        Task<PaginatedResult<StudentSubjectAnalyticsDto>> GetStudentSubjectAnalyticsAsync(
            long? studentSubjectId = null,
            int? minConsistency = null,
            int? maxConsistency = null,
            double? minAttendanceRate = null,
            double? maxAttendanceRate = null,
            double? minMotivationLevel = null,
            double? maxMotivationLevel = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<StudentSubjectAnalyticsDto> UpdateStudentSubjectAnalyticsAsync(long id, UpdateStudentSubjectAnalyticsDto updateStudentSubjectAnalyticsDto);
        Task<bool> DeleteStudentSubjectAnalyticsAsync(long id);
        Task<int> CountStudentSubjectAnalyticsAsync(long? studentSubjectId = null);
        Task<bool> StudentSubjectAnalyticsExistsAsync(long id);
        Task<bool> StudentSubjectAnalyticsExistsForStudentSubjectAsync(long studentSubjectId);
    }
}