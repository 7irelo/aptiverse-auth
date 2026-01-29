using Aptiverse.Api.Application.CourseEnrollments.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.CourseEnrollments.Services
{
    public interface ICourseEnrollmentService
    {
        Task<CourseEnrollmentDto> CreateEnrollmentAsync(CreateCourseEnrollmentDto createEnrollmentDto);
        Task<CourseEnrollmentDto?> GetEnrollmentByIdAsync(long id);

        Task<PaginatedResult<CourseEnrollmentDto>> GetEnrollmentsAsync(
            ClaimsPrincipal currentUser,
            long? courseId = null,
            long? studentId = null,
            string? paymentStatus = null,
            decimal? minProgress = null,
            decimal? maxProgress = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);

        Task<CourseEnrollmentDto> UpdateEnrollmentAsync(long id, UpdateCourseEnrollmentDto updateEnrollmentDto);
        Task<bool> DeleteEnrollmentAsync(long id);
        Task<int> CountEnrollmentsAsync(ClaimsPrincipal currentUser,
            long? courseId = null,
            long? studentId = null,
            string? paymentStatus = null);
        Task<bool> EnrollmentExistsAsync(long id);

        Task<IEnumerable<CourseEnrollmentDto>> GetEnrollmentsByStudentAsync(long studentId);
        Task<IEnumerable<CourseEnrollmentDto>> GetEnrollmentsByCourseAsync(long courseId);
        Task<decimal> GetStudentProgressAsync(long studentId, long courseId);
        Task<bool> IsStudentEnrolledAsync(long studentId, long courseId);
    }
}