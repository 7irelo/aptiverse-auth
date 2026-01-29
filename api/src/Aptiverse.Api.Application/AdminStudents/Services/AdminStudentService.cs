using Aptiverse.Api.Application.AdminStudents.Dtos;
using Aptiverse.Api.Domain.Models.Admins;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.AdminStudents.Services
{
    public class AdminStudentService(
        IRepository<AdminStudent> adminStudentRepository,
        IRepository<Admin> adminRepository,
        IRepository<Student> studentRepository,
        IMapper mapper) : IAdminStudentService
    {
        private readonly IRepository<AdminStudent> _adminStudentRepository = adminStudentRepository;
        private readonly IRepository<Admin> _adminRepository = adminRepository;
        private readonly IRepository<Student> _studentRepository = studentRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<AdminStudentDto> CreateAdminStudentAsync(CreateAdminStudentDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto);

            // Validate admin exists
            var adminExists = await _adminRepository.ExistsAsync(a => a.Id == createDto.SchoolAdminId);
            if (!adminExists)
            {
                throw new KeyNotFoundException($"Admin with ID {createDto.SchoolAdminId} not found");
            }

            // Validate student exists
            var studentExists = await _studentRepository.ExistsAsync(s => s.Id == createDto.StudentId);
            if (!studentExists)
            {
                throw new KeyNotFoundException($"Student with ID {createDto.StudentId} not found");
            }

            // Check if student is already enrolled with this admin
            var existingEnrollment = await _adminStudentRepository.GetAsync(
                predicate: adminStudent => adminStudent.SchoolAdminId == createDto.SchoolAdminId &&
                                         adminStudent.StudentId == createDto.StudentId);

            if (existingEnrollment != null)
            {
                throw new InvalidOperationException("Student is already enrolled with this admin");
            }

            var adminStudent = _mapper.Map<AdminStudent>(createDto);
            await _adminStudentRepository.AddAsync(adminStudent);

            return _mapper.Map<AdminStudentDto>(adminStudent);
        }

        public async Task<AdminStudentDto?> GetAdminStudentByIdAsync(long id)
        {
            var adminStudent = await _adminStudentRepository.GetAsync(
                predicate: adminStudent => adminStudent.Id == id,
                include: query => query
                    .Include(adminStudent => adminStudent.SchoolAdmin)
                    .Include(adminStudent => adminStudent.Student),
                disableTracking: false);

            if (adminStudent == null)
                return null;

            return _mapper.Map<AdminStudentDto>(adminStudent);
        }

        public async Task<AdminStudentDto?> GetAdminStudentByAdminAndStudentAsync(long adminId, long studentId)
        {
            var adminStudent = await _adminStudentRepository.GetAsync(
                predicate: adminStudent => adminStudent.SchoolAdminId == adminId &&
                                         adminStudent.StudentId == studentId,
                include: query => query
                    .Include(adminStudent => adminStudent.SchoolAdmin)
                    .Include(adminStudent => adminStudent.Student),
                disableTracking: false);

            if (adminStudent == null)
                return null;

            return _mapper.Map<AdminStudentDto>(adminStudent);
        }

        public async Task<PaginatedResult<AdminStudentDto>> GetAdminStudentsAsync(
            ClaimsPrincipal currentUser,
            long? adminId = null,
            long? studentId = null,
            string? enrollmentStatus = null,
            string? sortBy = "EnrolledDate",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20)
        {
            // Build predicate with role-based filtering
            Expression<Func<AdminStudent, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            // Add filter conditions
            if (adminId.HasValue || studentId.HasValue || !string.IsNullOrEmpty(enrollmentStatus))
            {
                Expression<Func<AdminStudent, bool>> filterPredicate = adminStudent =>
                    (!adminId.HasValue || adminStudent.SchoolAdminId == adminId.Value) &&
                    (!studentId.HasValue || adminStudent.StudentId == studentId.Value) &&
                    (string.IsNullOrEmpty(enrollmentStatus) || adminStudent.EnrollmentStatus == enrollmentStatus);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            // Apply sorting
            Func<IQueryable<AdminStudent>, IOrderedQueryable<AdminStudent>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _adminStudentRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(adminStudent => adminStudent.SchoolAdmin)
                    .Include(adminStudent => adminStudent.Student));

            var adminStudentDtos = _mapper.Map<List<AdminStudentDto>>(paginatedResult.Data);

            return new PaginatedResult<AdminStudentDto>(
                adminStudentDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        public async Task<AdminStudentDto> UpdateAdminStudentAsync(long id, UpdateAdminStudentDto updateDto)
        {
            var existingAdminStudent = await _adminStudentRepository.GetAsync(
                predicate: adminStudent => adminStudent.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"AdminStudent with ID {id} not found");

            _mapper.Map(updateDto, existingAdminStudent);
            await _adminStudentRepository.UpdateAsync(existingAdminStudent);
            return _mapper.Map<AdminStudentDto>(existingAdminStudent);
        }

        public async Task<bool> DeleteAdminStudentAsync(long id)
        {
            var adminStudent = await _adminStudentRepository.GetAsync(
                predicate: adminStudent => adminStudent.Id == id,
                disableTracking: false);

            if (adminStudent == null)
                return false;

            await _adminStudentRepository.DeleteAsync(adminStudent);
            return true;
        }

        public async Task<bool> DeleteAdminStudentByAdminAndStudentAsync(long adminId, long studentId)
        {
            var adminStudent = await _adminStudentRepository.GetAsync(
                predicate: adminStudent => adminStudent.SchoolAdminId == adminId &&
                                         adminStudent.StudentId == studentId,
                disableTracking: false);

            if (adminStudent == null)
                return false;

            await _adminStudentRepository.DeleteAsync(adminStudent);
            return true;
        }

        public async Task<IEnumerable<AdminStudentEnrollmentDto>> GetStudentEnrollmentsAsync(long studentId)
        {
            var enrollments = await _adminStudentRepository.GetManyAsync(
                predicate: adminStudent => adminStudent.StudentId == studentId,
                orderBy: query => query.OrderByDescending(adminStudent => adminStudent.EnrolledDate),
                include: query => query
                    .Include(adminStudent => adminStudent.SchoolAdmin)
                    .Include(adminStudent => adminStudent.Student));

            return enrollments.Select(e => new AdminStudentEnrollmentDto
            {
                AdminId = e.SchoolAdminId,
                SchoolName = e.SchoolAdmin?.SchoolName ?? "Unknown",
                StudentId = e.StudentId,
                StudentName = e.Student?.UserId ?? "Unknown",
                EnrolledDate = e.EnrolledDate,
                Status = e.EnrollmentStatus
            }).ToList();
        }

        public async Task<bool> IsStudentEnrolledAsync(long adminId, long studentId)
        {
            return await _adminStudentRepository.ExistsAsync(
                predicate: adminStudent => adminStudent.SchoolAdminId == adminId &&
                                         adminStudent.StudentId == studentId &&
                                         adminStudent.EnrollmentStatus == "Active");
        }

        public async Task<int> CountAdminStudentsAsync(long adminId, string? status = null)
        {
            Expression<Func<AdminStudent, bool>> predicate = adminStudent => adminStudent.SchoolAdminId == adminId;

            if (!string.IsNullOrEmpty(status))
            {
                predicate = adminStudent => adminStudent.SchoolAdminId == adminId &&
                                          adminStudent.EnrollmentStatus == status;
            }

            return await _adminStudentRepository.CountAsync(predicate);
        }

        public async Task TransferStudentAsync(long fromAdminId, long toAdminId, long studentId)
        {
            // Validate both admins exist
            var fromAdminExists = await _adminRepository.ExistsAsync(a => a.Id == fromAdminId);
            var toAdminExists = await _adminRepository.ExistsAsync(a => a.Id == toAdminId);

            if (!fromAdminExists || !toAdminExists)
            {
                throw new KeyNotFoundException($"One or both admins not found");
            }

            // Get current enrollment
            var currentEnrollment = await _adminStudentRepository.GetAsync(
                predicate: adminStudent => adminStudent.SchoolAdminId == fromAdminId &&
                                         adminStudent.StudentId == studentId,
                disableTracking: false);

            if (currentEnrollment == null)
            {
                throw new KeyNotFoundException($"Student is not enrolled with admin {fromAdminId}");
            }

            // Check if student is already enrolled with target admin
            var existingEnrollment = await _adminStudentRepository.GetAsync(
                predicate: adminStudent => adminStudent.SchoolAdminId == toAdminId &&
                                         adminStudent.StudentId == studentId);

            if (existingEnrollment != null)
            {
                // Update existing enrollment status
                existingEnrollment.EnrollmentStatus = "Active";
                existingEnrollment.EnrolledDate = DateTime.UtcNow;
                await _adminStudentRepository.UpdateAsync(existingEnrollment);
            }
            else
            {
                // Create new enrollment
                var newEnrollment = new AdminStudent
                {
                    SchoolAdminId = toAdminId,
                    StudentId = studentId,
                    EnrollmentStatus = "Active",
                    EnrolledDate = DateTime.UtcNow
                };
                await _adminStudentRepository.AddAsync(newEnrollment);
            }

            // Deactivate old enrollment
            currentEnrollment.EnrollmentStatus = "Transferred";
            await _adminStudentRepository.UpdateAsync(currentEnrollment);
        }

        private Expression<Func<AdminStudent, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsAdmin(user))
            {
                // Admin can only see enrollments for their school
                return adminStudent => adminStudent.SchoolAdmin.UserId == userId;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                // Teachers can see enrollments for students they teach
                return adminStudent => adminStudent.Student.TeacherStudents
                    .Any(ts => ts.Teacher.UserId == userId);
            }
            else if (UserContextHelper.IsTutor(user))
            {
                // Tutors can see enrollments for students they tutor
                return adminStudent => adminStudent.Student.TutorStudents
                    .Any(ts => ts.Tutor.UserId == userId);
            }
            else if (UserContextHelper.IsParent(user))
            {
                // Parents can see enrollments for their children
                return adminStudent => adminStudent.Student.ParentStudents
                    .Any(ps => ps.Parent.UserId == userId);
            }
            else if (UserContextHelper.IsStudent(user))
            {
                // Students can only see their own enrollments
                return adminStudent => adminStudent.Student.UserId == userId;
            }

            return adminStudent => false;
        }

        private Expression<Func<AdminStudent, bool>> CombinePredicates(
            Expression<Func<AdminStudent, bool>> left,
            Expression<Func<AdminStudent, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(AdminStudent), "adminStudent");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<AdminStudent, bool>>(combined, parameter);
        }

        private Func<IQueryable<AdminStudent>, IOrderedQueryable<AdminStudent>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "enrolleddate" => sortDescending
                    ? query => query.OrderByDescending(adminStudent => adminStudent.EnrolledDate)
                    : query => query.OrderBy(adminStudent => adminStudent.EnrolledDate),
                "enrollmentstatus" => sortDescending
                    ? query => query.OrderByDescending(adminStudent => adminStudent.EnrollmentStatus)
                    : query => query.OrderBy(adminStudent => adminStudent.EnrollmentStatus),
                "schoolname" => sortDescending
                    ? query => query.OrderByDescending(adminStudent => adminStudent.SchoolAdmin.SchoolName)
                    : query => query.OrderBy(adminStudent => adminStudent.SchoolAdmin.SchoolName),
                "studentname" => sortDescending
                    ? query => query.OrderByDescending(adminStudent => adminStudent.Student.UserId)
                    : query => query.OrderBy(adminStudent => adminStudent.Student.UserId),
                _ => sortDescending
                    ? query => query.OrderByDescending(adminStudent => adminStudent.Id)
                    : query => query.OrderBy(adminStudent => adminStudent.Id)
            };
        }
    }
}