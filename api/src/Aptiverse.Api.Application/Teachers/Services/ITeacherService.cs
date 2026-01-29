using Aptiverse.Api.Application.Teachers.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.Teachers.Services
{
    public interface ITeacherService
    {
        Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto createTeacherDto);
        Task<TeacherDto?> GetTeacherByIdAsync(long id);
        Task<PaginatedResult<TeacherDto>> GetTeachersAsync(
            string? search = null,
            string? specialization = null,
            int? minYearsOfExperience = null,
            bool? isVerified = null,
            decimal? minHourlyRate = null,
            decimal? maxHourlyRate = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<TeacherDto> UpdateTeacherAsync(long id, UpdateTeacherDto updateTeacherDto);
        Task<bool> DeleteTeacherAsync(long id);
        Task<int> CountTeachersAsync(string? specialization = null, bool? isVerified = null);
        Task<bool> TeacherExistsAsync(long id);
    }
}