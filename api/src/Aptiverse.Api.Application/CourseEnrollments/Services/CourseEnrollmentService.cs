using Aptiverse.Api.Application.CourseEnrollments.Dtos;
using Aptiverse.Api.Domain.Models.Courses;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.CourseEnrollments.Services
{
    public class CourseEnrollmentService(
        IRepository<CourseEnrollment> enrollmentRepository,
        IMapper mapper) : ICourseEnrollmentService
    {
        private readonly IRepository<CourseEnrollment> _enrollmentRepository = enrollmentRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<CourseEnrollmentDto> CreateEnrollmentAsync(CreateCourseEnrollmentDto createEnrollmentDto)
        {
            ArgumentNullException.ThrowIfNull(createEnrollmentDto);

            var enrollment = _mapper.Map<CourseEnrollment>(createEnrollmentDto);

            await _enrollmentRepository.AddAsync(enrollment);
            return _mapper.Map<CourseEnrollmentDto>(enrollment);
        }

        public async Task<CourseEnrollmentDto?> GetEnrollmentByIdAsync(long id)
        {
            var enrollment = await _enrollmentRepository.GetAsync(
                predicate: e => e.Id == id,
                disableTracking: false);

            if (enrollment == null)
                return null;

            return _mapper.Map<CourseEnrollmentDto>(enrollment);
        }

        public async Task<PaginatedResult<CourseEnrollmentDto>> GetEnrollmentsAsync(
            ClaimsPrincipal currentUser,
            long? courseId = null,
            long? studentId = null,
            string? paymentStatus = null,
            decimal? minProgress = null,
            decimal? maxProgress = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<CourseEnrollment, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (courseId.HasValue || studentId.HasValue || !string.IsNullOrEmpty(paymentStatus) ||
                minProgress.HasValue || maxProgress.HasValue)
            {
                Expression<Func<CourseEnrollment, bool>> filterPredicate = e =>
                    (!courseId.HasValue || e.CourseId == courseId.Value) &&
                    (!studentId.HasValue || e.StudentId == studentId.Value) &&
                    (string.IsNullOrEmpty(paymentStatus) || e.PaymentStatus == paymentStatus) &&
                    (!minProgress.HasValue || e.Progress >= minProgress.Value) &&
                    (!maxProgress.HasValue || e.Progress <= maxProgress.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<CourseEnrollment>, IOrderedQueryable<CourseEnrollment>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _enrollmentRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var enrollmentDtos = _mapper.Map<List<CourseEnrollmentDto>>(paginatedResult.Data);

            return new PaginatedResult<CourseEnrollmentDto>(
                enrollmentDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<CourseEnrollment, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null; // Can see all enrollments
            }
            else if (UserContextHelper.IsStudent(user))
            {
                return e => e.Student != null && e.Student.UserId == userId;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return e => e.Course != null && e.Course.Teacher != null && e.Course.Teacher.UserId == userId;
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return e => e.Course != null && e.Course.Tutor != null && e.Course.Tutor.UserId == userId;
            }
            else if (UserContextHelper.IsAdmin(user))
            {
                return null;
            }
            else
            {
                return e => false;
            }
        }

        private Expression<Func<CourseEnrollment, bool>> CombinePredicates(
            Expression<Func<CourseEnrollment, bool>> left,
            Expression<Func<CourseEnrollment, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(CourseEnrollment), "e");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<CourseEnrollment, bool>>(combined, parameter);
        }

        private Func<IQueryable<CourseEnrollment>, IOrderedQueryable<CourseEnrollment>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "enrolledat" => sortDescending
                    ? query => query.OrderByDescending(e => e.EnrolledAt)
                    : query => query.OrderBy(e => e.EnrolledAt),
                "progress" => sortDescending
                    ? query => query.OrderByDescending(e => e.Progress)
                    : query => query.OrderBy(e => e.Progress),
                "amountpaid" => sortDescending
                    ? query => query.OrderByDescending(e => e.AmountPaid)
                    : query => query.OrderBy(e => e.AmountPaid),
                "completedat" => sortDescending
                    ? query => query.OrderByDescending(e => e.CompletedAt)
                    : query => query.OrderBy(e => e.CompletedAt),
                _ => sortDescending
                    ? query => query.OrderByDescending(e => e.Id)
                    : query => query.OrderBy(e => e.Id)
            };
        }

        public async Task<CourseEnrollmentDto> UpdateEnrollmentAsync(long id, UpdateCourseEnrollmentDto updateEnrollmentDto)
        {
            var existingEnrollment = await _enrollmentRepository.GetAsync(
                predicate: e => e.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Enrollment with ID {id} not found");

            _mapper.Map(updateEnrollmentDto, existingEnrollment);

            await _enrollmentRepository.UpdateAsync(existingEnrollment);
            return _mapper.Map<CourseEnrollmentDto>(existingEnrollment);
        }

        public async Task<bool> DeleteEnrollmentAsync(long id)
        {
            var enrollment = await _enrollmentRepository.GetAsync(
                predicate: e => e.Id == id,
                disableTracking: false);

            if (enrollment == null)
                return false;

            await _enrollmentRepository.DeleteAsync(enrollment);
            return true;
        }

        public async Task<int> CountEnrollmentsAsync(
            ClaimsPrincipal currentUser,
            long? courseId = null,
            long? studentId = null,
            string? paymentStatus = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<CourseEnrollment, bool>> filterPredicate = e =>
                (!courseId.HasValue || e.CourseId == courseId.Value) &&
                (!studentId.HasValue || e.StudentId == studentId.Value) &&
                (string.IsNullOrEmpty(paymentStatus) || e.PaymentStatus == paymentStatus);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _enrollmentRepository.CountAsync(predicate);
        }

        public async Task<bool> EnrollmentExistsAsync(long id)
        {
            return await _enrollmentRepository.ExistsAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<CourseEnrollmentDto>> GetEnrollmentsByStudentAsync(long studentId)
        {
            var enrollments = await _enrollmentRepository.GetManyAsync(
                predicate: e => e.StudentId == studentId,
                orderBy: query => query.OrderByDescending(e => e.EnrolledAt),
                disableTracking: true);

            return _mapper.Map<IEnumerable<CourseEnrollmentDto>>(enrollments);
        }

        public async Task<IEnumerable<CourseEnrollmentDto>> GetEnrollmentsByCourseAsync(long courseId)
        {
            var enrollments = await _enrollmentRepository.GetManyAsync(
                predicate: e => e.CourseId == courseId,
                orderBy: query => query.OrderByDescending(e => e.EnrolledAt),
                disableTracking: true);

            return _mapper.Map<IEnumerable<CourseEnrollmentDto>>(enrollments);
        }

        public async Task<decimal> GetStudentProgressAsync(long studentId, long courseId)
        {
            var enrollment = await _enrollmentRepository.GetAsync(
                predicate: e => e.StudentId == studentId && e.CourseId == courseId,
                disableTracking: true);

            return enrollment?.Progress ?? 0;
        }

        public async Task<bool> IsStudentEnrolledAsync(long studentId, long courseId)
        {
            return await _enrollmentRepository.ExistsAsync(e =>
                e.StudentId == studentId && e.CourseId == courseId);
        }
    }
}