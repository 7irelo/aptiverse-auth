using Aptiverse.Api.Application.TutorStudents.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.TutorStudents.Services
{
    public interface ITutorStudentService
    {
        Task<TutorStudentDto> CreateTutorStudentAsync(CreateTutorStudentDto createTutorStudentDto);
        Task<TutorStudentDto?> GetTutorStudentByIdAsync(long id);
        Task<PaginatedResult<TutorStudentDto>> GetTutorStudentsAsync(
            long? tutorId = null,
            long? studentId = null,
            bool? isActive = null,
            int? minSessionsPerWeek = null,
            int? maxSessionsPerWeek = null,
            DateTime? startedAfter = null,
            DateTime? startedBefore = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<TutorStudentDto> UpdateTutorStudentAsync(long id, UpdateTutorStudentDto updateTutorStudentDto);
        Task<bool> DeleteTutorStudentAsync(long id);
        Task<int> CountTutorStudentsAsync(long? tutorId = null, long? studentId = null, bool? isActive = null);
        Task<bool> TutorStudentExistsAsync(long id);
    }
}