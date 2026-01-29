using Aptiverse.Api.Application.StudentPointss.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.StudentPointss.Services
{
    public interface IStudentPointsService
    {
        Task<StudentPointsDto> CreateStudentPointsAsync(CreateStudentPointsDto createStudentPointsDto);
        Task<StudentPointsDto?> GetStudentPointsByIdAsync(long id);
        Task<StudentPointsDto?> GetStudentPointsByStudentIdAsync(long studentId);
        Task<PaginatedResult<StudentPointsDto>> GetStudentPointsAsync(
            long? studentId = null,
            int? minLevel = null,
            int? maxLevel = null,
            string? rank = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<StudentPointsDto> UpdateStudentPointsAsync(long id, UpdateStudentPointsDto updateStudentPointsDto);
        Task<bool> DeleteStudentPointsAsync(long id);
        Task<int> CountStudentPointsAsync(long? studentId = null, int? minLevel = null, string? rank = null);
        Task<bool> StudentPointsExistsAsync(long id);
        Task<bool> StudentPointsExistsForStudentAsync(long studentId);
    }
}