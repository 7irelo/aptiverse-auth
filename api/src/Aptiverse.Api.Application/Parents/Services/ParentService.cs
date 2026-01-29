using Aptiverse.Api.Application.Parents.Dtos;
using Aptiverse.Api.Domain.Models.Parents;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.Parents.Services
{
    public class ParentService(
        IRepository<Parent> parentRepository,
        IMapper mapper) : IParentService
    {
        private readonly IRepository<Parent> _parentRepository = parentRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ParentDto> CreateParentAsync(CreateParentDto createParentDto)
        {
            ArgumentNullException.ThrowIfNull(createParentDto);

            var parent = _mapper.Map<Parent>(createParentDto);

            await _parentRepository.AddAsync(parent);
            return _mapper.Map<ParentDto>(parent);
        }

        public async Task<ParentDto?> GetParentByIdAsync(long id)
        {
            var parent = await _parentRepository.GetAsync(
                predicate: p => p.Id == id,
                disableTracking: false);

            return _mapper.Map<ParentDto>(parent);
        }

        public async Task<ParentDto?> GetParentByUserIdAsync(string userId)
        {
            var parent = await _parentRepository.GetAsync(
                predicate: p => p.UserId == userId,
                disableTracking: false);

            return _mapper.Map<ParentDto>(parent);
        }

        public async Task<PaginatedResult<ParentDto>> GetParentsAsync(
            ClaimsPrincipal currentUser,
            string? search = null,
            string? occupation = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<Parent, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (!string.IsNullOrEmpty(search) || !string.IsNullOrEmpty(occupation))
            {
                Expression<Func<Parent, bool>> filterPredicate = p =>
                    (string.IsNullOrEmpty(search) ||
                     p.UserId.Contains(search) ||
                     p.ContactNumber.Contains(search) ||
                     p.Address.Contains(search) ||
                     p.Occupation.Contains(search)) &&
                    (string.IsNullOrEmpty(occupation) || p.Occupation == occupation);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<Parent>, IOrderedQueryable<Parent>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _parentRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var parentDtos = _mapper.Map<List<ParentDto>>(paginatedResult.Data);

            return new PaginatedResult<ParentDto>(
                parentDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<Parent, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsAdmin(user))
            {
                return null;
            }
            else if (UserContextHelper.IsParent(user))
            {
                return p => p.UserId == userId;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return p => p.ParentStudents != null &&
                           p.ParentStudents.Any(ps => ps.Student != null &&
                           ps.Student.TeacherStudents != null &&
                           ps.Student.TeacherStudents.Any(ts => ts.Teacher != null && ts.Teacher.UserId == userId));
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return p => p.ParentStudents != null &&
                           p.ParentStudents.Any(ps => ps.Student != null &&
                           ps.Student.TutorStudents != null &&
                           ps.Student.TutorStudents.Any(ts => ts.Tutor != null && ts.Tutor.UserId == userId));
            }
            else
            {
                return p => false;
            }
        }

        private Expression<Func<Parent, bool>> CombinePredicates(
            Expression<Func<Parent, bool>> left,
            Expression<Func<Parent, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(Parent), "p");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<Parent, bool>>(combined, parameter);
        }

        private Func<IQueryable<Parent>, IOrderedQueryable<Parent>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "userid" => sortDescending
                    ? query => query.OrderByDescending(p => p.UserId)
                    : query => query.OrderBy(p => p.UserId),
                "occupation" => sortDescending
                    ? query => query.OrderByDescending(p => p.Occupation)
                    : query => query.OrderBy(p => p.Occupation),
                "createdat" => sortDescending
                    ? query => query.OrderByDescending(p => p.CreatedAt)
                    : query => query.OrderBy(p => p.CreatedAt),
                _ => sortDescending
                    ? query => query.OrderByDescending(p => p.Id)
                    : query => query.OrderBy(p => p.Id)
            };
        }

        public async Task<ParentDto> UpdateParentAsync(long id, UpdateParentDto updateParentDto)
        {
            var existingParent = await _parentRepository.GetAsync(
                predicate: p => p.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Parent with ID {id} not found");

            _mapper.Map(updateParentDto, existingParent);

            await _parentRepository.UpdateAsync(existingParent);
            return _mapper.Map<ParentDto>(existingParent);
        }

        public async Task<bool> DeleteParentAsync(long id)
        {
            var parent = await _parentRepository.GetAsync(
                predicate: p => p.Id == id,
                disableTracking: false);

            if (parent == null)
                return false;

            await _parentRepository.DeleteAsync(parent);
            return true;
        }

        public async Task<int> CountParentsAsync(ClaimsPrincipal currentUser, string? occupation = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<Parent, bool>> filterPredicate = p =>
                (string.IsNullOrEmpty(occupation) || p.Occupation == occupation);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _parentRepository.CountAsync(predicate);
        }

        public async Task<bool> ParentExistsAsync(long id)
        {
            return await _parentRepository.ExistsAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<ParentDto>> GetParentsByStudentAsync(long studentId)
        {
            var parents = await _parentRepository.GetManyAsync(
                predicate: p => p.ParentStudents != null && p.ParentStudents.Any(ps => ps.StudentId == studentId),
                orderBy: query => query.OrderBy(p => p.CreatedAt),
                include: query => query.Include(p => p.ParentStudents),
                disableTracking: true);

            return _mapper.Map<IEnumerable<ParentDto>>(parents);
        }

        public async Task<int> GetStudentCountAsync(long parentId)
        {
            var parent = await _parentRepository.GetAsync(
                predicate: p => p.Id == parentId,
                include: query => query.Include(p => p.ParentStudents),
                disableTracking: true);

            return parent?.ParentStudents?.Count ?? 0;
        }
    }
}