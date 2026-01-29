using Aptiverse.Api.Application.Subjects.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.Subjects.Services
{
    public interface ISubjectService
    {
        Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createSubjectDto);
        Task<SubjectDto?> GetSubjectByIdAsync(string id);
        Task<PaginatedResult<SubjectDto>> GetSubjectsAsync(
            string? search = null,
            string? code = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<SubjectDto> UpdateSubjectAsync(string id, UpdateSubjectDto updateSubjectDto);
        Task<bool> DeleteSubjectAsync(string id);
        Task<int> CountSubjectsAsync(string? search = null);
        Task<bool> SubjectExistsAsync(string id);
    }
}