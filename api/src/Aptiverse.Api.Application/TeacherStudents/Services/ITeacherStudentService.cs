using Aptiverse.Api.Application.TeacherStudents.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.TeacherStudents.Services
{
    public interface ITeacherStudentService
    {
        Task<TeacherStudentDto> CreateTeacherStudentAsync(CreateTeacherStudentDto createTeacherStudentDto);
        Task<TeacherStudentDto?> GetTeacherStudentByIdAsync(long id);
        Task<PaginatedResult<TeacherStudentDto>> GetTeacherStudentsAsync(
            long? teacherId = null,
            long? studentId = null,
            bool? isActive = null,
            DateTime? assignedAfter = null,
            DateTime? assignedBefore = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<TeacherStudentDto> UpdateTeacherStudentAsync(long id, UpdateTeacherStudentDto updateTeacherStudentDto);
        Task<bool> DeleteTeacherStudentAsync(long id);
        Task<int> CountTeacherStudentsAsync(long? teacherId = null, long? studentId = null, bool? isActive = null);
        Task<bool> TeacherStudentExistsAsync(long id);
    }
}