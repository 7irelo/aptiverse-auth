using Aptiverse.Api.Application.TeacherAdmins.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.TeacherAdmins.Services
{
    public interface ITeacherAdminService
    {
        Task<TeacherAdminDto> CreateTeacherAdminAsync(CreateTeacherAdminDto createTeacherAdminDto);
        Task<TeacherAdminDto?> GetTeacherAdminByIdAsync(long id);
        Task<PaginatedResult<TeacherAdminDto>> GetTeacherAdminsAsync(
            long? teacherId = null,
            long? adminId = null,
            bool? isActive = null,
            string? role = null,
            DateTime? associatedAfter = null,
            DateTime? associatedBefore = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<TeacherAdminDto> UpdateTeacherAdminAsync(long id, UpdateTeacherAdminDto updateTeacherAdminDto);
        Task<bool> DeleteTeacherAdminAsync(long id);
        Task<int> CountTeacherAdminsAsync(long? teacherId = null, long? adminId = null, bool? isActive = null);
        Task<bool> TeacherAdminExistsAsync(long id);
    }
}