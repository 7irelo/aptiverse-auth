using Aptiverse.Api.Application.TutorSubjects.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.TutorSubjects.Services
{
    public interface ITutorSubjectService
    {
        Task<TutorSubjectDto> CreateTutorSubjectAsync(CreateTutorSubjectDto createTutorSubjectDto);
        Task<TutorSubjectDto?> GetTutorSubjectByIdAsync(long id);
        Task<PaginatedResult<TutorSubjectDto>> GetTutorSubjectsAsync(
            long? tutorId = null,
            string? subjectId = null,
            int? minProficiencyLevel = null,
            int? maxProficiencyLevel = null,
            decimal? minHourlyRate = null,
            decimal? maxHourlyRate = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<TutorSubjectDto> UpdateTutorSubjectAsync(long id, UpdateTutorSubjectDto updateTutorSubjectDto);
        Task<bool> DeleteTutorSubjectAsync(long id);
        Task<int> CountTutorSubjectsAsync(long? tutorId = null, string? subjectId = null);
        Task<bool> TutorSubjectExistsAsync(long id);
    }
}