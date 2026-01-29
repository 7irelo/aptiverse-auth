using Aptiverse.Api.Application.Admins.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.Admins.Services
{
    public interface IAdminService
    {
        Task<AdminDto> CreateAdminAsync(CreateAdminDto createDto);
        Task<AdminDto?> GetAdminByIdAsync(long id);
        Task<AdminDto?> GetAdminByUserIdAsync(string userId);
        Task<PaginatedResult<AdminDto>> GetAdminsAsync(
            ClaimsPrincipal currentUser,
            string? search = null,
            bool? isActive = null,
            string? sortBy = "SchoolName",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<AdminDto> UpdateAdminAsync(long id, UpdateAdminDto updateDto);
        Task<bool> DeleteAdminAsync(long id);
        Task<AdminSummaryDto> GetAdminSummaryAsync(long adminId);
        Task<int> GetAdminStudentCountAsync(long adminId);
        Task<int> GetAdminTeacherCountAsync(long adminId);
        Task<bool> AdminExistsAsync(long id);
        Task<bool> IsUserAdminAsync(string userId);
    }
}