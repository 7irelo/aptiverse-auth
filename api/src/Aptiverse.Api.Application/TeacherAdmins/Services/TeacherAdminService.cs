using Aptiverse.Api.Application.TeacherAdmins.Dtos;
using Aptiverse.Api.Domain.Models.Teachers;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.TeacherAdmins.Services
{
    public class TeacherAdminService(
        IRepository<TeacherAdmin> teacherAdminRepository,
        IMapper mapper) : ITeacherAdminService
    {
        private readonly IRepository<TeacherAdmin> _teacherAdminRepository = teacherAdminRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<TeacherAdminDto> CreateTeacherAdminAsync(CreateTeacherAdminDto createTeacherAdminDto)
        {
            ArgumentNullException.ThrowIfNull(createTeacherAdminDto);

            TeacherAdmin teacherAdmin = _mapper.Map<TeacherAdmin>(createTeacherAdminDto);
            await _teacherAdminRepository.AddAsync(teacherAdmin);
            return _mapper.Map<TeacherAdminDto>(teacherAdmin);
        }

        public async Task<TeacherAdminDto?> GetTeacherAdminByIdAsync(long id)
        {
            var teacherAdmin = await _teacherAdminRepository.GetAsync(
                predicate: ta => ta.Id == id,
                include: query => query
                    .Include(ta => ta.Teacher)
                    .Include(ta => ta.Admin),
                disableTracking: false);

            if (teacherAdmin == null)
                return null;

            return _mapper.Map<TeacherAdminDto>(teacherAdmin);
        }

        public async Task<PaginatedResult<TeacherAdminDto>> GetTeacherAdminsAsync(
            long? teacherId = null,
            long? adminId = null,
            bool? isActive = null,
            string? role = null,
            DateTime? associatedAfter = null,
            DateTime? associatedBefore = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<TeacherAdmin, bool>>? predicate = BuildFilterPredicate(
                teacherId, adminId, isActive, role, associatedAfter, associatedBefore);

            Func<IQueryable<TeacherAdmin>, IOrderedQueryable<TeacherAdmin>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _teacherAdminRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(ta => ta.Teacher)
                    .Include(ta => ta.Admin));

            var teacherAdminDtos = _mapper.Map<List<TeacherAdminDto>>(paginatedResult.Data);

            return new PaginatedResult<TeacherAdminDto>(
                teacherAdminDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<TeacherAdmin, bool>>? BuildFilterPredicate(
            long? teacherId,
            long? adminId,
            bool? isActive,
            string? role,
            DateTime? associatedAfter,
            DateTime? associatedBefore)
        {
            if (!teacherId.HasValue && !adminId.HasValue &&
                !isActive.HasValue && string.IsNullOrEmpty(role) &&
                !associatedAfter.HasValue && !associatedBefore.HasValue)
                return null;

            return ta =>
                (!teacherId.HasValue || ta.TeacherId == teacherId.Value) &&
                (!adminId.HasValue || ta.AdminId == adminId.Value) &&
                (!isActive.HasValue || ta.IsActive == isActive.Value) &&
                (string.IsNullOrEmpty(role) || ta.Role == role) &&
                (!associatedAfter.HasValue || ta.AssociatedAt >= associatedAfter.Value) &&
                (!associatedBefore.HasValue || ta.AssociatedAt <= associatedBefore.Value);
        }

        private Func<IQueryable<TeacherAdmin>, IOrderedQueryable<TeacherAdmin>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "associatedat" => sortDescending
                    ? query => query.OrderByDescending(ta => ta.AssociatedAt).ThenByDescending(ta => ta.Id)
                    : query => query.OrderBy(ta => ta.AssociatedAt).ThenBy(ta => ta.Id),
                "teacherid" => sortDescending
                    ? query => query.OrderByDescending(ta => ta.TeacherId).ThenByDescending(ta => ta.Id)
                    : query => query.OrderBy(ta => ta.TeacherId).ThenBy(ta => ta.Id),
                "adminid" => sortDescending
                    ? query => query.OrderByDescending(ta => ta.AdminId).ThenByDescending(ta => ta.Id)
                    : query => query.OrderBy(ta => ta.AdminId).ThenBy(ta => ta.Id),
                "role" => sortDescending
                    ? query => query.OrderByDescending(ta => ta.Role).ThenByDescending(ta => ta.Id)
                    : query => query.OrderBy(ta => ta.Role).ThenBy(ta => ta.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(ta => ta.Id)
                    : query => query.OrderBy(ta => ta.Id)
            };
        }

        public async Task<TeacherAdminDto> UpdateTeacherAdminAsync(long id, UpdateTeacherAdminDto updateTeacherAdminDto)
        {
            var existingTeacherAdmin = await _teacherAdminRepository.GetAsync(
                predicate: ta => ta.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"TeacherAdmin with ID {id} not found");

            _mapper.Map(updateTeacherAdminDto, existingTeacherAdmin);
            await _teacherAdminRepository.UpdateAsync(existingTeacherAdmin);
            return _mapper.Map<TeacherAdminDto>(existingTeacherAdmin);
        }

        public async Task<bool> DeleteTeacherAdminAsync(long id)
        {
            var teacherAdmin = await _teacherAdminRepository.GetAsync(
                predicate: ta => ta.Id == id,
                disableTracking: false);

            if (teacherAdmin == null)
                return false;

            await _teacherAdminRepository.DeleteAsync(teacherAdmin);
            return true;
        }

        public async Task<int> CountTeacherAdminsAsync(long? teacherId = null, long? adminId = null, bool? isActive = null)
        {
            if (!teacherId.HasValue && !adminId.HasValue && !isActive.HasValue)
                return await _teacherAdminRepository.CountAsync();

            Expression<Func<TeacherAdmin, bool>> predicate = ta =>
                (!teacherId.HasValue || ta.TeacherId == teacherId.Value) &&
                (!adminId.HasValue || ta.AdminId == adminId.Value) &&
                (!isActive.HasValue || ta.IsActive == isActive.Value);

            return await _teacherAdminRepository.CountAsync(predicate);
        }

        public async Task<bool> TeacherAdminExistsAsync(long id)
        {
            return await _teacherAdminRepository.ExistsAsync(ta => ta.Id == id);
        }
    }
}