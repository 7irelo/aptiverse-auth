using Aptiverse.Api.Application.Admins.Dtos;
using Aptiverse.Api.Domain.Models.Admins;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.Admins.Services
{
    public class AdminService(
        IRepository<Admin> adminRepository,
        IRepository<AdminStudent> adminStudentRepository,
        IMapper mapper) : IAdminService
    {
        private readonly IRepository<Admin> _adminRepository = adminRepository;
        private readonly IRepository<AdminStudent> _adminStudentRepository = adminStudentRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<AdminDto> CreateAdminAsync(CreateAdminDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto);

            var admin = _mapper.Map<Admin>(createDto);
            await _adminRepository.AddAsync(admin);

            return _mapper.Map<AdminDto>(admin);
        }

        public async Task<AdminDto?> GetAdminByIdAsync(long id)
        {
            var admin = await _adminRepository.GetAsync(
                predicate: a => a.Id == id,
                include: query => query
                    .Include(a => a.Students)
                    .Include(a => a.Teachers),
                disableTracking: false);

            if (admin == null)
                return null;

            return _mapper.Map<AdminDto>(admin);
        }

        public async Task<AdminDto?> GetAdminByUserIdAsync(string userId)
        {
            var admin = await _adminRepository.GetAsync(
                predicate: a => a.UserId == userId,
                include: query => query
                    .Include(a => a.Students)
                    .Include(a => a.Teachers),
                disableTracking: false);

            if (admin == null)
                return null;

            return _mapper.Map<AdminDto>(admin);
        }

        public async Task<PaginatedResult<AdminDto>> GetAdminsAsync(
            ClaimsPrincipal currentUser,
            string? search = null,
            bool? isActive = null,
            string? sortBy = "SchoolName",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<Admin, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (!string.IsNullOrEmpty(search) || isActive.HasValue)
            {
                Expression<Func<Admin, bool>> filterPredicate = a =>
                    (string.IsNullOrEmpty(search) ||
                     a.SchoolName.Contains(search) ||
                     a.SchoolCode.Contains(search) ||
                     a.ContactNumber.Contains(search)) &&
                    (!isActive.HasValue || a.IsActive == isActive.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<Admin>, IOrderedQueryable<Admin>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _adminRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(a => a.Students)
                    .Include(a => a.Teachers));

            var adminDtos = _mapper.Map<List<AdminDto>>(paginatedResult.Data);

            return new PaginatedResult<AdminDto>(
                adminDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        public async Task<AdminDto> UpdateAdminAsync(long id, UpdateAdminDto updateDto)
        {
            var existingAdmin = await _adminRepository.GetAsync(
                predicate: a => a.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Admin with ID {id} not found");

            _mapper.Map(updateDto, existingAdmin);
            await _adminRepository.UpdateAsync(existingAdmin);
            return _mapper.Map<AdminDto>(existingAdmin);
        }

        public async Task<bool> DeleteAdminAsync(long id)
        {
            var admin = await _adminRepository.GetAsync(
                predicate: a => a.Id == id,
                disableTracking: false);

            if (admin == null)
                return false;

            var hasStudents = await _adminStudentRepository.AnyAsync(adminStudent => adminStudent.SchoolAdminId == id);
            if (hasStudents || admin.Teachers?.Any() == true)
            {
                throw new InvalidOperationException("Cannot delete admin with assigned students or teachers");
            }

            await _adminRepository.DeleteAsync(admin);
            return true;
        }

        public async Task<AdminSummaryDto> GetAdminSummaryAsync(long adminId)
        {
            var admin = await _adminRepository.GetAsync(
                predicate: a => a.Id == adminId,
                include: query => query
                    .Include(a => a.Students)
                    .Include(a => a.Teachers),
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Admin with ID {adminId} not found");

            var totalStudents = admin.Students?.Count ?? 0;
            var totalTeachers = admin.Teachers?.Count ?? 0;

            var activeStudents = totalStudents;
            var activeTeachers = totalTeachers;

            var lastEnrollment = await _adminStudentRepository.GetManyAsync(
                predicate: adminStudent => adminStudent.SchoolAdminId == adminId,
                orderBy: query => query.OrderByDescending(adminStudent => adminStudent.EnrolledDate));

            return new AdminSummaryDto
            {
                Id = admin.Id,
                SchoolName = admin.SchoolName,
                SchoolCode = admin.SchoolCode,
                TotalStudents = totalStudents,
                TotalTeachers = totalTeachers,
                ActiveStudents = activeStudents,
                ActiveTeachers = activeTeachers,
                LastEnrollmentDate = lastEnrollment.FirstOrDefault()?.EnrolledDate ?? admin.CreatedAt
            };
        }

        public async Task<int> GetAdminStudentCountAsync(long adminId)
        {
            var count = await _adminStudentRepository.CountAsync(
                predicate: adminStudent => adminStudent.SchoolAdminId == adminId && adminStudent.EnrollmentStatus == "Active");

            return count;
        }

        public async Task<int> GetAdminTeacherCountAsync(long adminId)
        {
            var admin = await _adminRepository.GetAsync(
                predicate: a => a.Id == adminId,
                include: query => query.Include(a => a.Teachers));

            return admin?.Teachers?.Count ?? 0;
        }

        public async Task<bool> AdminExistsAsync(long id)
        {
            return await _adminRepository.ExistsAsync(a => a.Id == id);
        }

        public async Task<bool> IsUserAdminAsync(string userId)
        {
            return await _adminRepository.ExistsAsync(a => a.UserId == userId && a.IsActive);
        }

        private Expression<Func<Admin, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsAdmin(user))
            {
                return a => a.UserId == userId;
            }
            return a => false;
        }

        private Expression<Func<Admin, bool>> CombinePredicates(
            Expression<Func<Admin, bool>> left,
            Expression<Func<Admin, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(Admin), "a");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<Admin, bool>>(combined, parameter);
        }

        private Func<IQueryable<Admin>, IOrderedQueryable<Admin>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "schoolname" => sortDescending
                    ? query => query.OrderByDescending(a => a.SchoolName)
                    : query => query.OrderBy(a => a.SchoolName),
                "schoolcode" => sortDescending
                    ? query => query.OrderByDescending(a => a.SchoolCode)
                    : query => query.OrderBy(a => a.SchoolCode),
                "createdat" => sortDescending
                    ? query => query.OrderByDescending(a => a.CreatedAt)
                    : query => query.OrderBy(a => a.CreatedAt),
                "isactive" => sortDescending
                    ? query => query.OrderByDescending(a => a.IsActive)
                    : query => query.OrderBy(a => a.IsActive),
                _ => sortDescending
                    ? query => query.OrderByDescending(a => a.Id)
                    : query => query.OrderBy(a => a.Id)
            };
        }
    }
}