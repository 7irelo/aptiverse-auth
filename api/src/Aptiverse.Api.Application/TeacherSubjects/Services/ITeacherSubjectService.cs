using Aptiverse.Api.Application.TeacherSubjects.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.TeacherSubjects.Services
{
    public interface ITeacherSubjectService
    {
        Task<TeacherSubjectDto> CreateTeacherSubjectAsync(CreateTeacherSubjectDto createTeacherSubjectDto);
        Task<TeacherSubjectDto?> GetTeacherSubjectByIdAsync(long id);
        Task<PaginatedResult<TeacherSubjectDto>> GetTeacherSubjectsAsync(
            long? teacherId = null,
            string? subjectId = null,
            int? minProficiencyLevel = null,
            int? maxProficiencyLevel = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<TeacherSubjectDto> UpdateTeacherSubjectAsync(long id, UpdateTeacherSubjectDto updateTeacherSubjectDto);
        Task<bool> DeleteTeacherSubjectAsync(long id);
        Task<int> CountTeacherSubjectsAsync(long? teacherId = null, string? subjectId = null);
        Task<bool> TeacherSubjectExistsAsync(long id);
    }
}