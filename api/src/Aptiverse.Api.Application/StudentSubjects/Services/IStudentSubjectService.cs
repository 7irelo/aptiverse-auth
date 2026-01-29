using Aptiverse.Api.Application.StudentSubjects.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.StudentSubjects.Services
{
    public interface IStudentSubjectService
    {
        Task<StudentSubjectDto> CreateStudentSubjectAsync(CreateStudentSubjectDto createStudentSubjectDto);
        Task<StudentSubjectDto?> GetStudentSubjectByIdAsync(long id);
        Task<PaginatedResult<StudentSubjectDto>> GetStudentSubjectsAsync(
            long? studentId = null,
            string? subjectId = null,
            int? minProgress = null,
            int? maxProgress = null,
            double? minAverageScore = null,
            double? maxAverageScore = null,
            string? performanceTrend = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<StudentSubjectDto> UpdateStudentSubjectAsync(long id, UpdateStudentSubjectDto updateStudentSubjectDto);
        Task<bool> DeleteStudentSubjectAsync(long id);
        Task<int> CountStudentSubjectsAsync(long? studentId = null, string? subjectId = null);
        Task<bool> StudentSubjectExistsAsync(long id);
        Task<bool> StudentSubjectExistsForStudentAndSubjectAsync(long studentId, string subjectId);
    }
}