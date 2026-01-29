using Aptiverse.Api.Application.ParentStudents.Dtos;
using Aptiverse.Api.Domain.Models.Parents;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.ParentStudents.Services
{
    public class ParentStudentService(
        IRepository<ParentStudent> parentStudentRepository,
        IMapper mapper) : IParentStudentService
    {
        private readonly IRepository<ParentStudent> _parentStudentRepository = parentStudentRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ParentStudentDto> CreateParentStudentAsync(CreateParentStudentDto createParentStudentDto)
        {
            ArgumentNullException.ThrowIfNull(createParentStudentDto);

            var parentStudent = _mapper.Map<ParentStudent>(createParentStudentDto);

            if (createParentStudentDto.IsPrimaryContact)
            {
                await UpdatePrimaryContactStatus(createParentStudentDto.StudentId, createParentStudentDto.ParentId);
            }

            await _parentStudentRepository.AddAsync(parentStudent);
            return _mapper.Map<ParentStudentDto>(parentStudent);
        }

        public async Task<ParentStudentDto?> GetParentStudentByIdAsync(long id)
        {
            var parentStudent = await _parentStudentRepository.GetAsync(
                predicate: ps => ps.Id == id,
                disableTracking: false);

            return _mapper.Map<ParentStudentDto>(parentStudent);
        }

        public async Task<PaginatedResult<ParentStudentDto>> GetParentStudentsAsync(
            ClaimsPrincipal currentUser,
            long? parentId = null,
            long? studentId = null,
            string? relationship = null,
            bool? isPrimaryContact = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<ParentStudent, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (parentId.HasValue || studentId.HasValue || !string.IsNullOrEmpty(relationship) || isPrimaryContact.HasValue)
            {
                Expression<Func<ParentStudent, bool>> filterPredicate = ps =>
                    (!parentId.HasValue || ps.ParentId == parentId.Value) &&
                    (!studentId.HasValue || ps.StudentId == studentId.Value) &&
                    (string.IsNullOrEmpty(relationship) || ps.Relationship == relationship) &&
                    (!isPrimaryContact.HasValue || ps.IsPrimaryContact == isPrimaryContact.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<ParentStudent>, IOrderedQueryable<ParentStudent>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _parentStudentRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var parentStudentDtos = _mapper.Map<List<ParentStudentDto>>(paginatedResult.Data);

            return new PaginatedResult<ParentStudentDto>(
                parentStudentDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<ParentStudent, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
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
                return ps => ps.Parent != null && ps.Parent.UserId == userId;
            }
            else if (UserContextHelper.IsStudent(user))
            {
                return ps => ps.Student != null && ps.Student.UserId == userId;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return ps => ps.Student != null &&
                           ps.Student.TeacherStudents != null &&
                           ps.Student.TeacherStudents.Any(ts => ts.Teacher != null && ts.Teacher.UserId == userId);
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return ps => ps.Student != null &&
                           ps.Student.TutorStudents != null &&
                           ps.Student.TutorStudents.Any(ts => ts.Tutor != null && ts.Tutor.UserId == userId);
            }
            else
            {
                return ps => false;
            }
        }

        private Expression<Func<ParentStudent, bool>> CombinePredicates(
            Expression<Func<ParentStudent, bool>> left,
            Expression<Func<ParentStudent, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(ParentStudent), "ps");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<ParentStudent, bool>>(combined, parameter);
        }

        private Func<IQueryable<ParentStudent>, IOrderedQueryable<ParentStudent>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "relationship" => sortDescending
                    ? query => query.OrderByDescending(ps => ps.Relationship)
                    : query => query.OrderBy(ps => ps.Relationship),
                "isprimarycontact" => sortDescending
                    ? query => query.OrderByDescending(ps => ps.IsPrimaryContact)
                    : query => query.OrderBy(ps => ps.IsPrimaryContact),
                _ => sortDescending
                    ? query => query.OrderByDescending(ps => ps.Id)
                    : query => query.OrderBy(ps => ps.Id)
            };
        }

        public async Task<ParentStudentDto> UpdateParentStudentAsync(long id, UpdateParentStudentDto updateParentStudentDto)
        {
            var existingParentStudent = await _parentStudentRepository.GetAsync(
                predicate: ps => ps.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Parent student relationship with ID {id} not found");

            var wasPrimaryContact = existingParentStudent.IsPrimaryContact;

            _mapper.Map(updateParentStudentDto, existingParentStudent);

            if (updateParentStudentDto.IsPrimaryContact && !wasPrimaryContact)
            {
                await UpdatePrimaryContactStatus(existingParentStudent.StudentId, existingParentStudent.ParentId);
            }

            await _parentStudentRepository.UpdateAsync(existingParentStudent);
            return _mapper.Map<ParentStudentDto>(existingParentStudent);
        }

        public async Task<bool> DeleteParentStudentAsync(long id)
        {
            var parentStudent = await _parentStudentRepository.GetAsync(
                predicate: ps => ps.Id == id,
                disableTracking: false);

            if (parentStudent == null)
                return false;

            await _parentStudentRepository.DeleteAsync(parentStudent);
            return true;
        }

        public async Task<int> CountParentStudentsAsync(
            ClaimsPrincipal currentUser,
            long? parentId = null,
            long? studentId = null,
            string? relationship = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<ParentStudent, bool>> filterPredicate = ps =>
                (!parentId.HasValue || ps.ParentId == parentId.Value) &&
                (!studentId.HasValue || ps.StudentId == studentId.Value) &&
                (string.IsNullOrEmpty(relationship) || ps.Relationship == relationship);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _parentStudentRepository.CountAsync(predicate);
        }

        public async Task<bool> ParentStudentExistsAsync(long id)
        {
            return await _parentStudentRepository.ExistsAsync(ps => ps.Id == id);
        }

        public async Task<IEnumerable<ParentStudentDto>> GetParentStudentsByParentAsync(long parentId)
        {
            var parentStudents = await _parentStudentRepository.GetManyAsync(
                predicate: ps => ps.ParentId == parentId,
                orderBy: query => query.OrderBy(ps => ps.Relationship),
                disableTracking: true);

            return _mapper.Map<IEnumerable<ParentStudentDto>>(parentStudents);
        }

        public async Task<IEnumerable<ParentStudentDto>> GetParentStudentsByStudentAsync(long studentId)
        {
            var parentStudents = await _parentStudentRepository.GetManyAsync(
                predicate: ps => ps.StudentId == studentId,
                orderBy: query => query.OrderByDescending(ps => ps.IsPrimaryContact).ThenBy(ps => ps.Relationship),
                disableTracking: true);

            return _mapper.Map<IEnumerable<ParentStudentDto>>(parentStudents);
        }

        public async Task<ParentStudentDto?> GetPrimaryContactForStudentAsync(long studentId)
        {
            var parentStudent = await _parentStudentRepository.GetAsync(
                predicate: ps => ps.StudentId == studentId && ps.IsPrimaryContact,
                disableTracking: true);

            return _mapper.Map<ParentStudentDto>(parentStudent);
        }

        public async Task<bool> ExistsAsync(long parentId, long studentId)
        {
            return await _parentStudentRepository.ExistsAsync(ps =>
                ps.ParentId == parentId && ps.StudentId == studentId);
        }

        private async Task UpdatePrimaryContactStatus(long studentId, long newPrimaryParentId)
        {
            var currentPrimaryContacts = await _parentStudentRepository.GetManyAsync(
                predicate: ps => ps.StudentId == studentId && ps.IsPrimaryContact && ps.ParentId != newPrimaryParentId,
                disableTracking: false);

            foreach (var contact in currentPrimaryContacts)
            {
                contact.IsPrimaryContact = false;
                await _parentStudentRepository.UpdateAsync(contact);
            }
        }
    }
}