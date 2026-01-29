using Aptiverse.Api.Application.Courses.Dtos;
using Aptiverse.Api.Domain.Models.Courses;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.Courses.Services
{
    public class CourseService(
        IRepository<Course> courseRepository,
        IMapper mapper) : ICourseService
    {
        private readonly IRepository<Course> _courseRepository = courseRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto)
        {
            ArgumentNullException.ThrowIfNull(createCourseDto);

            var course = _mapper.Map<Course>(createCourseDto);

            await _courseRepository.AddAsync(course);
            return await GetCourseByIdAsync(course.Id);
        }

        public async Task<CourseDto?> GetCourseByIdAsync(long id)
        {
            var course = await _courseRepository.GetAsync(
                predicate: c => c.Id == id,
                include: query => query
                    .Include(c => c.Subject)
                    .Include(c => c.Teacher)
                    .Include(c => c.Tutor)
                    .Include(c => c.Modules)
                    .Include(c => c.Enrollments)
                    .Include(c => c.Resources),
                disableTracking: false);

            if (course == null)
                return null;

            return _mapper.Map<CourseDto>(course);
        }

        public async Task<PaginatedResult<CourseDto>> GetCoursesAsync(
            ClaimsPrincipal currentUser,
            string? search = null,
            string? subjectId = null,
            string? level = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? isPublished = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<Course, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (!string.IsNullOrEmpty(search) || !string.IsNullOrEmpty(subjectId) ||
                !string.IsNullOrEmpty(level) || minPrice.HasValue || maxPrice.HasValue ||
                isPublished.HasValue)
            {
                Expression<Func<Course, bool>> filterPredicate = c =>
                    (string.IsNullOrEmpty(search) ||
                     c.Title.Contains(search) ||
                     c.Description.Contains(search)) &&
                    (string.IsNullOrEmpty(subjectId) || c.SubjectId == subjectId) &&
                    (string.IsNullOrEmpty(level) || c.Level == level) &&
                    (!minPrice.HasValue || c.Price >= minPrice.Value) &&
                    (!maxPrice.HasValue || c.Price <= maxPrice.Value) &&
                    (!isPublished.HasValue || c.IsPublished == isPublished.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<Course>, IOrderedQueryable<Course>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _courseRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(c => c.Subject)
                    .Include(c => c.Teacher)
                    .Include(c => c.Tutor));

            var courseDtos = _mapper.Map<List<CourseDto>>(paginatedResult.Data);

            return new PaginatedResult<CourseDto>(
                courseDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<Course, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return c => c.Teacher != null && c.Teacher.UserId == userId;
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return c => c.Tutor != null && c.Tutor.UserId == userId;
            }
            else if (UserContextHelper.IsAdmin(user))
            {
                return null;
            }
            else if (UserContextHelper.IsStudent(user))
            {
                return c => c.IsPublished;
            }
            else
            {
                return c => c.IsPublished;
            }
        }

        private Expression<Func<Course, bool>> CombinePredicates(
            Expression<Func<Course, bool>> left,
            Expression<Func<Course, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(Course), "c");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<Course, bool>>(combined, parameter);
        }

        private Func<IQueryable<Course>, IOrderedQueryable<Course>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "title" => sortDescending
                    ? query => query.OrderByDescending(c => c.Title)
                    : query => query.OrderBy(c => c.Title),
                "price" => sortDescending
                    ? query => query.OrderByDescending(c => c.Price)
                    : query => query.OrderBy(c => c.Price),
                "rating" => sortDescending
                    ? query => query.OrderByDescending(c => c.Rating)
                    : query => query.OrderBy(c => c.Rating),
                "createdat" => sortDescending
                    ? query => query.OrderByDescending(c => c.CreatedAt)
                    : query => query.OrderBy(c => c.CreatedAt),
                "totalstudents" => sortDescending
                    ? query => query.OrderByDescending(c => c.TotalStudents)
                    : query => query.OrderBy(c => c.TotalStudents),
                _ => sortDescending
                    ? query => query.OrderByDescending(c => c.Id)
                    : query => query.OrderBy(c => c.Id)
            };
        }

        public async Task<CourseDto> UpdateCourseAsync(long id, UpdateCourseDto updateCourseDto)
        {
            var existingCourse = await _courseRepository.GetAsync(
                predicate: c => c.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Course with ID {id} not found");

            _mapper.Map(updateCourseDto, existingCourse);
            existingCourse.UpdatedAt = DateTime.UtcNow;

            await _courseRepository.UpdateAsync(existingCourse);
            return await GetCourseByIdAsync(id);
        }

        public async Task<bool> DeleteCourseAsync(long id)
        {
            var course = await _courseRepository.GetAsync(
                predicate: c => c.Id == id,
                disableTracking: false);

            if (course == null)
                return false;

            await _courseRepository.DeleteAsync(course);
            return true;
        }

        public async Task<int> CountCoursesAsync(
            ClaimsPrincipal currentUser,
            string? subjectId = null,
            string? level = null,
            bool? isPublished = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<Course, bool>> filterPredicate = c =>
                (string.IsNullOrEmpty(subjectId) || c.SubjectId == subjectId) &&
                (string.IsNullOrEmpty(level) || c.Level == level) &&
                (!isPublished.HasValue || c.IsPublished == isPublished.Value);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _courseRepository.CountAsync(predicate);
        }

        public async Task<bool> CourseExistsAsync(long id)
        {
            return await _courseRepository.ExistsAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CourseDto>> GetPopularCoursesAsync(int count = 10)
        {
            var courses = await _courseRepository.GetManyAsync(
                predicate: c => c.IsPublished,
                orderBy: query => query
                    .OrderByDescending(c => c.TotalStudents)
                    .ThenByDescending(c => c.Rating),
                include: query => query
                    .Include(c => c.Subject)
                    .Include(c => c.Teacher),
                disableTracking: true);

            return _mapper.Map<IEnumerable<CourseDto>>(courses.Take(count));
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByTeacherAsync(long teacherId)
        {
            var courses = await _courseRepository.GetManyAsync(
                predicate: c => c.TeacherId == teacherId,
                include: query => query
                    .Include(c => c.Subject)
                    .Include(c => c.Teacher),
                disableTracking: true);

            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByTutorAsync(long tutorId)
        {
            var courses = await _courseRepository.GetManyAsync(
                predicate: c => c.TutorId == tutorId,
                include: query => query
                    .Include(c => c.Subject)
                    .Include(c => c.Tutor),
                disableTracking: true);

            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }

        public async Task<IEnumerable<CourseDto>> GetRecommendedCoursesAsync(string userId)
        {
            return await GetPopularCoursesAsync(10);
        }
    }
}