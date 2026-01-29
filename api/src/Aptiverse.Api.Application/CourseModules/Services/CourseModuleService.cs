using Aptiverse.Api.Application.CourseModules.Dtos;
using Aptiverse.Api.Domain.Models.Courses;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.CourseModules.Services
{
    public class CourseModuleService(
        IRepository<CourseModule> moduleRepository,
        IMapper mapper) : ICourseModuleService
    {
        private readonly IRepository<CourseModule> _moduleRepository = moduleRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<CourseModuleDto> CreateModuleAsync(CreateCourseModuleDto createModuleDto)
        {
            ArgumentNullException.ThrowIfNull(createModuleDto);

            var module = _mapper.Map<CourseModule>(createModuleDto);

            await _moduleRepository.AddAsync(module);
            return _mapper.Map<CourseModuleDto>(module);
        }

        public async Task<CourseModuleDto?> GetModuleByIdAsync(long id)
        {
            var module = await _moduleRepository.GetAsync(
                predicate: m => m.Id == id,
                disableTracking: false);

            if (module == null)
                return null;

            return _mapper.Map<CourseModuleDto>(module);
        }

        public async Task<PaginatedResult<CourseModuleDto>> GetModulesAsync(
            ClaimsPrincipal currentUser,
            long? courseId = null,
            string? search = null,
            string? sortBy = "Order",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<CourseModule, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (courseId.HasValue || !string.IsNullOrEmpty(search))
            {
                Expression<Func<CourseModule, bool>> filterPredicate = m =>
                    (!courseId.HasValue || m.CourseId == courseId.Value) &&
                    (string.IsNullOrEmpty(search) ||
                     m.Title.Contains(search) ||
                     m.Description.Contains(search));

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<CourseModule>, IOrderedQueryable<CourseModule>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _moduleRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var moduleDtos = _mapper.Map<List<CourseModuleDto>>(paginatedResult.Data);

            return new PaginatedResult<CourseModuleDto>(
                moduleDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<CourseModule, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return m => m.Course != null && m.Course.Teacher != null && m.Course.Teacher.UserId == userId;
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return m => m.Course != null && m.Course.Tutor != null && m.Course.Tutor.UserId == userId;
            }
            else if (UserContextHelper.IsStudent(user))
            {
                return m => m.Course != null &&
                           m.Course.Enrollments != null &&
                           m.Course.Enrollments.Any(e => e.Student != null && e.Student.UserId == userId);
            }
            else
            {
                return m => false;
            }
        }

        private Expression<Func<CourseModule, bool>> CombinePredicates(
            Expression<Func<CourseModule, bool>> left,
            Expression<Func<CourseModule, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(CourseModule), "m");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<CourseModule, bool>>(combined, parameter);
        }

        private Func<IQueryable<CourseModule>, IOrderedQueryable<CourseModule>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "title" => sortDescending
                    ? query => query.OrderByDescending(m => m.Title)
                    : query => query.OrderBy(m => m.Title),
                "order" => sortDescending
                    ? query => query.OrderByDescending(m => m.Order)
                    : query => query.OrderBy(m => m.Order),
                "durationhours" => sortDescending
                    ? query => query.OrderByDescending(m => m.DurationHours)
                    : query => query.OrderBy(m => m.DurationHours),
                _ => sortDescending
                    ? query => query.OrderByDescending(m => m.Id)
                    : query => query.OrderBy(m => m.Id)
            };
        }

        public async Task<CourseModuleDto> UpdateModuleAsync(long id, UpdateCourseModuleDto updateModuleDto)
        {
            var existingModule = await _moduleRepository.GetAsync(
                predicate: m => m.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Module with ID {id} not found");

            _mapper.Map(updateModuleDto, existingModule);

            await _moduleRepository.UpdateAsync(existingModule);
            return _mapper.Map<CourseModuleDto>(existingModule);
        }

        public async Task<bool> DeleteModuleAsync(long id)
        {
            var module = await _moduleRepository.GetAsync(
                predicate: m => m.Id == id,
                disableTracking: false);

            if (module == null)
                return false;

            await _moduleRepository.DeleteAsync(module);
            return true;
        }

        public async Task<int> CountModulesAsync(ClaimsPrincipal currentUser, long? courseId = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<CourseModule, bool>> filterPredicate = m =>
                (!courseId.HasValue || m.CourseId == courseId.Value);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _moduleRepository.CountAsync(predicate);
        }

        public async Task<bool> ModuleExistsAsync(long id)
        {
            return await _moduleRepository.ExistsAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<CourseModuleDto>> GetModulesByCourseAsync(long courseId)
        {
            var modules = await _moduleRepository.GetManyAsync(
                predicate: m => m.CourseId == courseId,
                orderBy: query => query.OrderBy(m => m.Order),
                disableTracking: true);

            return _mapper.Map<IEnumerable<CourseModuleDto>>(modules);
        }

        public async Task ReorderModulesAsync(long courseId, List<long> moduleIdsInOrder)
        {
            if (moduleIdsInOrder == null || !moduleIdsInOrder.Any())
                return;

            var modules = await _moduleRepository.GetManyAsync(
                predicate: m => m.CourseId == courseId,
                disableTracking: false);

            var moduleDict = modules.ToDictionary(m => m.Id);

            for (int i = 0; i < moduleIdsInOrder.Count; i++)
            {
                if (moduleDict.TryGetValue(moduleIdsInOrder[i], out var module))
                {
                    module.Order = i + 1;
                }
            }

            await _moduleRepository.SaveChangesAsync();
        }

        public async Task<decimal> GetCourseTotalDurationAsync(long courseId)
        {
            var modules = await _moduleRepository.GetManyAsync(
                predicate: m => m.CourseId == courseId,
                disableTracking: true);

            return modules.Sum(m => m.DurationHours);
        }
    }
}