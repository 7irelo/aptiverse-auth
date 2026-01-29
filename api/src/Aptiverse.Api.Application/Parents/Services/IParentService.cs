using Aptiverse.Api.Application.Parents.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.Parents.Services
{
    public interface IParentService
    {
        Task<ParentDto> CreateParentAsync(CreateParentDto createParentDto);
        Task<ParentDto?> GetParentByIdAsync(long id);
        Task<ParentDto?> GetParentByUserIdAsync(string userId);

        Task<PaginatedResult<ParentDto>> GetParentsAsync(
            ClaimsPrincipal currentUser,
            string? search = null,
            string? occupation = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);

        Task<ParentDto> UpdateParentAsync(long id, UpdateParentDto updateParentDto);
        Task<bool> DeleteParentAsync(long id);
        Task<int> CountParentsAsync(ClaimsPrincipal currentUser, string? occupation = null);
        Task<bool> ParentExistsAsync(long id);

        Task<IEnumerable<ParentDto>> GetParentsByStudentAsync(long studentId);
        Task<int> GetStudentCountAsync(long parentId);
    }
}