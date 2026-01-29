using Aptiverse.Api.Application.Students.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.Students.Services
{
    public class StudentService(
        IRepository<Student> studentRepository,
        IMapper mapper) : IStudentService
    {
        private readonly IRepository<Student> _studentRepository = studentRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto)
        {
            ArgumentNullException.ThrowIfNull(createStudentDto);

            Student student = _mapper.Map<Student>(createStudentDto);

            await _studentRepository.AddAsync(student);
            return _mapper.Map<StudentDto>(student);
        }

        public async Task<StudentDto?> GetStudentByIdAsync(long id)
        {
            var student = await _studentRepository.GetAsync(
                predicate: s => s.Id == id,
                include: query => query
                    .Include(s => s.StudentSubjects)
                    .ThenInclude(ss => ss.Subject)
                    .Include(s => s.Admin)
                    .Include(s => s.ParentStudents)
                        .ThenInclude(ps => ps.Parent)
                    .Include(s => s.TeacherStudents)
                        .ThenInclude(ts => ts.Teacher)
                    .Include(s => s.TutorStudents)
                        .ThenInclude(ts => ts.Tutor),
                disableTracking: false);

            if (student == null)
                return null;

            return _mapper.Map<StudentDto>(student);
        }

        public async Task<PaginatedResult<StudentDto>> GetStudentsAsync(
            ClaimsPrincipal currentUser,
            string? search = null,
            string? grade = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<Student, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (!string.IsNullOrEmpty(search) || !string.IsNullOrEmpty(grade))
            {
                Expression<Func<Student, bool>> searchPredicate = s =>
                    (string.IsNullOrEmpty(search) || s.UserId.Contains(search)) &&
                    (string.IsNullOrEmpty(grade) || s.Grade == grade);

                predicate = predicate == null
                    ? searchPredicate
                    : CombinePredicates(predicate, searchPredicate);
            }

            Func<IQueryable<Student>, IOrderedQueryable<Student>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _studentRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(s => s.StudentSubjects)
                    .Include(s => s.Admin));

            var studentDtos = _mapper.Map<List<StudentDto>>(paginatedResult.Data);

            return new PaginatedResult<StudentDto>(
                studentDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<Student, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsAdmin(user))
            {
                return s => s.Admin != null && s.Admin.UserId == userId;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return s => s.TeacherStudents.Any(ts => ts.Teacher.UserId == userId);
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return s => s.TutorStudents.Any(ts => ts.Tutor.UserId == userId);
            }
            else if (UserContextHelper.IsParent(user))
            {
                return s => s.ParentStudents.Any(ps => ps.Parent.UserId == userId);
            }
            else if (UserContextHelper.IsStudent(user))
            {
                return s => s.UserId == userId;
            }

            return s => false;
        }

        private Expression<Func<Student, bool>> CombinePredicates(
            Expression<Func<Student, bool>> left,
            Expression<Func<Student, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(Student), "s");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<Student, bool>>(combined, parameter);
        }

        private Func<IQueryable<Student>, IOrderedQueryable<Student>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "grade" => sortDescending
                    ? query => query.OrderByDescending(s => s.Grade).ThenByDescending(s => s.Id)
                    : query => query.OrderBy(s => s.Grade).ThenBy(s => s.Id),
                "userid" => sortDescending
                    ? query => query.OrderByDescending(s => s.UserId)
                    : query => query.OrderBy(s => s.UserId),
                _ => sortDescending
                    ? query => query.OrderByDescending(s => s.Id)
                    : query => query.OrderBy(s => s.Id)
            };
        }

        public async Task<StudentDto> UpdateStudentAsync(long id, UpdateStudentDto updateStudentDto)
        {
            var existingStudent = await _studentRepository.GetAsync(
                predicate: s => s.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Student with ID {id} not found");

            _mapper.Map(updateStudentDto, existingStudent);
            await _studentRepository.UpdateAsync(existingStudent);
            return _mapper.Map<StudentDto>(existingStudent);
        }

        public async Task<bool> DeleteStudentAsync(long id)
        {
            var student = await _studentRepository.GetAsync(
                predicate: s => s.Id == id,
                disableTracking: false);

            if (student == null)
                return false;

            await _studentRepository.DeleteAsync(student);
            return true;
        }

        public async Task<int> CountStudentsAsync(ClaimsPrincipal currentUser, string? grade = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            if (!string.IsNullOrEmpty(grade))
            {
                Expression<Func<Student, bool>> gradePredicate = s => s.Grade == grade;
                predicate = predicate == null
                    ? gradePredicate
                    : CombinePredicates(predicate, gradePredicate);
            }

            return await _studentRepository.CountAsync(predicate);
        }

        public async Task<bool> StudentExistsAsync(long id)
        {
            return await _studentRepository.ExistsAsync(s => s.Id == id);
        }

        public async Task<PaginatedResult<StudentDto>> GetStudentsAsync(
            string? search = null,
            string? grade = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int? page = null,
            int? pageSize = null)
        {
            throw new NotImplementedException("Use the method with ClaimsPrincipal parameter");
        }
    }
}