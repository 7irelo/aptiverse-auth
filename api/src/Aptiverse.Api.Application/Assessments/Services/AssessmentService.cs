using Aptiverse.Api.Application.Assessments.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.Assessments.Services
{
    public class AssessmentService(
        IRepository<Assessment> assessmentRepository,
        IRepository<Student> studentRepository,
        IMapper mapper) : IAssessmentService
    {
        private readonly IRepository<Assessment> _assessmentRepository = assessmentRepository;
        private readonly IRepository<Student> _studentRepository = studentRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<AssessmentDto> CreateAssessmentAsync(CreateAssessmentDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto);

            var assessment = _mapper.Map<Assessment>(createDto);
            await _assessmentRepository.AddAsync(assessment);

            return _mapper.Map<AssessmentDto>(assessment);
        }

        public async Task<AssessmentDto?> GetAssessmentByIdAsync(long id)
        {
            var assessment = await _assessmentRepository.GetAsync(
                predicate: a => a.Id == id,
                include: query => query
                    .Include(a => a.Student)
                    .Include(a => a.Subject),
                disableTracking: false);

            if (assessment == null)
                return null;

            return _mapper.Map<AssessmentDto>(assessment);
        }

        public async Task<PaginatedResult<AssessmentDto>> GetAssessmentsAsync(
            ClaimsPrincipal currentUser,
            long? studentId = null,
            string? subjectId = null,
            string? type = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? sortBy = "DateTaken",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20)
        {
            // Start with role-based predicate
            Expression<Func<Assessment, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            // Combine with other filters
            if (studentId.HasValue || !string.IsNullOrEmpty(subjectId) ||
                !string.IsNullOrEmpty(type) || fromDate.HasValue || toDate.HasValue)
            {
                Expression<Func<Assessment, bool>> filterPredicate = a =>
                    (!studentId.HasValue || a.StudentId == studentId.Value) &&
                    (string.IsNullOrEmpty(subjectId) || a.SubjectId == subjectId) &&
                    (string.IsNullOrEmpty(type) || a.Type == type) &&
                    (!fromDate.HasValue || a.DateTaken >= fromDate.Value) &&
                    (!toDate.HasValue || a.DateTaken <= toDate.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            // Apply sorting
            Func<IQueryable<Assessment>, IOrderedQueryable<Assessment>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _assessmentRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(a => a.Student)
                    .Include(a => a.Subject));

            var assessmentDtos = _mapper.Map<List<AssessmentDto>>(paginatedResult.Data);

            return new PaginatedResult<AssessmentDto>(
                assessmentDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        public async Task<AssessmentDto> UpdateAssessmentAsync(long id, UpdateAssessmentDto updateDto)
        {
            var existingAssessment = await _assessmentRepository.GetAsync(
                predicate: a => a.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Assessment with ID {id} not found");

            _mapper.Map(updateDto, existingAssessment);
            await _assessmentRepository.UpdateAsync(existingAssessment);
            return _mapper.Map<AssessmentDto>(existingAssessment);
        }

        public async Task<bool> DeleteAssessmentAsync(long id)
        {
            var assessment = await _assessmentRepository.GetAsync(
                predicate: a => a.Id == id,
                disableTracking: false);

            if (assessment == null)
                return false;

            await _assessmentRepository.DeleteAsync(assessment);
            return true;
        }

        public async Task<double> GetStudentAverageScoreAsync(long studentId, string? subjectId = null)
        {
            Expression<Func<Assessment, bool>> predicate = a => a.StudentId == studentId;

            if (!string.IsNullOrEmpty(subjectId))
            {
                predicate = a => a.StudentId == studentId && a.SubjectId == subjectId;
            }

            var assessments = await _assessmentRepository.GetManyAsync(predicate: predicate);

            if (!assessments.Any())
                return 0;

            return assessments.Average(a => a.Score / a.MaxScore * 100);
        }

        public async Task<IEnumerable<AssessmentSummaryDto>> GetAssessmentSummaryAsync(long studentId)
        {
            var assessments = await _assessmentRepository.GetManyAsync(
                predicate: a => a.StudentId == studentId,
                include: query => query.Include(a => a.Subject));

            var summary = assessments
                .GroupBy(a => new { a.SubjectId, SubjectName = a.Subject?.Name ?? "Unknown" })
                .Select(g => new AssessmentSummaryDto
                {
                    SubjectId = g.Key.SubjectId,
                    SubjectName = g.Key.SubjectName,
                    TotalAssessments = g.Count(),
                    AverageScore = g.Average(a => a.Score / a.MaxScore * 100),
                    HighestScore = g.Max(a => a.Score / a.MaxScore * 100),
                    LowestScore = g.Min(a => a.Score / a.MaxScore * 100)
                })
                .ToList();

            return summary;
        }

        public async Task<IEnumerable<AssessmentTrendDto>> GetAssessmentTrendAsync(long studentId, string subjectId, string type)
        {
            var assessments = await _assessmentRepository.GetManyAsync(
                predicate: a => a.StudentId == studentId &&
                               a.SubjectId == subjectId &&
                               a.Type == type,
                orderBy: query => query.OrderBy(a => a.DateTaken));

            var trend = assessments
                .Select((a, index) => new AssessmentTrendDto
                {
                    Date = a.DateTaken,
                    Score = a.Score / a.MaxScore * 100,
                    AverageScore = assessments
                        .Take(index + 1)
                        .Average(x => x.Score / x.MaxScore * 100),
                    Type = a.Type
                })
                .ToList();

            return trend;
        }

        private Expression<Func<Assessment, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user) || UserContextHelper.IsAdmin(user))
            {
                // Superuser and Admin can see all assessments
                return null;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                // Teachers can see assessments of students they teach
                return a => a.Student.TeacherStudents.Any(ts => ts.Teacher.UserId == userId);
            }
            else if (UserContextHelper.IsTutor(user))
            {
                // Tutors can see assessments of students they tutor
                return a => a.Student.TutorStudents.Any(ts => ts.Tutor.UserId == userId);
            }
            else if (UserContextHelper.IsParent(user))
            {
                // Parents can see assessments of their children
                return a => a.Student.ParentStudents.Any(ps => ps.Parent.UserId == userId);
            }
            else if (UserContextHelper.IsStudent(user))
            {
                // Students can only see their own assessments
                return a => a.Student.UserId == userId;
            }

            return a => false;
        }

        private Expression<Func<Assessment, bool>> CombinePredicates(
            Expression<Func<Assessment, bool>> left,
            Expression<Func<Assessment, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(Assessment), "a");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<Assessment, bool>>(combined, parameter);
        }

        private Func<IQueryable<Assessment>, IOrderedQueryable<Assessment>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "score" => sortDescending
                    ? query => query.OrderByDescending(a => a.Score / a.MaxScore)
                    : query => query.OrderBy(a => a.Score / a.MaxScore),
                "datetaken" => sortDescending
                    ? query => query.OrderByDescending(a => a.DateTaken)
                    : query => query.OrderBy(a => a.DateTaken),
                "type" => sortDescending
                    ? query => query.OrderByDescending(a => a.Type)
                    : query => query.OrderBy(a => a.Type),
                _ => sortDescending
                    ? query => query.OrderByDescending(a => a.Id)
                    : query => query.OrderBy(a => a.Id)
            };
        }
    }
}