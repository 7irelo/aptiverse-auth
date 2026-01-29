using Aptiverse.Api.Application.AdminStudents.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.AdminStudents.Services
{
    public interface IAdminStudentService
    {
        Task<AdminStudentDto> CreateAdminStudentAsync(CreateAdminStudentDto createDto);
        Task<AdminStudentDto?> GetAdminStudentByIdAsync(long id);
        Task<AdminStudentDto?> GetAdminStudentByAdminAndStudentAsync(long adminId, long studentId);
        Task<PaginatedResult<AdminStudentDto>> GetAdminStudentsAsync(
            ClaimsPrincipal currentUser,
            long? adminId = null,
            long? studentId = null,
            string? enrollmentStatus = null,
            string? sortBy = "EnrolledDate",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20);
        Task<AdminStudentDto> UpdateAdminStudentAsync(long id, UpdateAdminStudentDto updateDto);
        Task<bool> DeleteAdminStudentAsync(long id);
        Task<bool> DeleteAdminStudentByAdminAndStudentAsync(long adminId, long studentId);
        Task<IEnumerable<AdminStudentEnrollmentDto>> GetStudentEnrollmentsAsync(long studentId);
        Task<bool> IsStudentEnrolledAsync(long adminId, long studentId);
        Task<int> CountAdminStudentsAsync(long adminId, string? status = null);
        Task TransferStudentAsync(long fromAdminId, long toAdminId, long studentId);
    }
}