using Aptiverse.Api.Application.ParentStudents.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.ParentStudents.Services
{
    public interface IParentStudentService
    {
        Task<ParentStudentDto> CreateParentStudentAsync(CreateParentStudentDto createParentStudentDto);
        Task<ParentStudentDto?> GetParentStudentByIdAsync(long id);

        Task<PaginatedResult<ParentStudentDto>> GetParentStudentsAsync(
            ClaimsPrincipal currentUser,
            long? parentId = null,
            long? studentId = null,
            string? relationship = null,
            bool? isPrimaryContact = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);

        Task<ParentStudentDto> UpdateParentStudentAsync(long id, UpdateParentStudentDto updateParentStudentDto);
        Task<bool> DeleteParentStudentAsync(long id);
        Task<int> CountParentStudentsAsync(ClaimsPrincipal currentUser,
            long? parentId = null,
            long? studentId = null,
            string? relationship = null);
        Task<bool> ParentStudentExistsAsync(long id);

        Task<IEnumerable<ParentStudentDto>> GetParentStudentsByParentAsync(long parentId);
        Task<IEnumerable<ParentStudentDto>> GetParentStudentsByStudentAsync(long studentId);
        Task<ParentStudentDto?> GetPrimaryContactForStudentAsync(long studentId);
        Task<bool> ExistsAsync(long parentId, long studentId);
    }
}