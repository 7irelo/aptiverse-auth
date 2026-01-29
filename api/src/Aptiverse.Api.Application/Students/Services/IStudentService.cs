using Aptiverse.Api.Application.Students.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.Students.Services
{
    public interface IStudentService
    {
        Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto);
        Task<StudentDto?> GetStudentByIdAsync(long id);

        Task<PaginatedResult<StudentDto>> GetStudentsAsync(
            ClaimsPrincipal currentUser,
            string? search = null,
            string? grade = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);

        Task<StudentDto> UpdateStudentAsync(long id, UpdateStudentDto updateStudentDto);
        Task<bool> DeleteStudentAsync(long id);
        Task<int> CountStudentsAsync(ClaimsPrincipal currentUser, string? grade = null);
        Task<bool> StudentExistsAsync(long id);
    }
}