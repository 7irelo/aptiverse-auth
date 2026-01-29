using Aptiverse.Api.Application.AssessmentBreakdowns.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.AssessmentBreakdowns.Services
{
    public class AssessmentBreakdownService(
        IRepository<AssessmentBreakdown> breakdownRepository,
        IRepository<Assessment> assessmentRepository,
        IRepository<StudentSubject> studentSubjectRepository,
        IMapper mapper) : IAssessmentBreakdownService
    {
        private readonly IRepository<AssessmentBreakdown> _breakdownRepository = breakdownRepository;
        private readonly IRepository<Assessment> _assessmentRepository = assessmentRepository;
        private readonly IRepository<StudentSubject> _studentSubjectRepository = studentSubjectRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<AssessmentBreakdownDto> CreateAssessmentBreakdownAsync(CreateAssessmentBreakdownDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto);

            var breakdown = _mapper.Map<AssessmentBreakdown>(createDto);
            await _breakdownRepository.AddAsync(breakdown);

            return _mapper.Map<AssessmentBreakdownDto>(breakdown);
        }

        public async Task<AssessmentBreakdownDto?> GetAssessmentBreakdownByIdAsync(long id)
        {
            var breakdown = await _breakdownRepository.GetAsync(
                predicate: b => b.Id == id,
                include: query => query
                    .Include(b => b.StudentSubject)
                        .ThenInclude(ss => ss.Student)
                    .Include(b => b.StudentSubject)
                        .ThenInclude(ss => ss.Subject),
                disableTracking: false);

            if (breakdown == null)
                return null;

            return _mapper.Map<AssessmentBreakdownDto>(breakdown);
        }

        public async Task<PaginatedResult<AssessmentBreakdownDto>> GetAssessmentBreakdownsAsync(
            ClaimsPrincipal currentUser,
            long? studentSubjectId = null,
            string? assessmentType = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            // Start with role-based predicate
            Expression<Func<AssessmentBreakdown, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            // Combine with other filters
            if (studentSubjectId.HasValue || !string.IsNullOrEmpty(assessmentType))
            {
                Expression<Func<AssessmentBreakdown, bool>> filterPredicate = b =>
                    (!studentSubjectId.HasValue || b.StudentSubjectId == studentSubjectId.Value) &&
                    (string.IsNullOrEmpty(assessmentType) || b.AssessmentType == assessmentType);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            // Apply sorting
            Func<IQueryable<AssessmentBreakdown>, IOrderedQueryable<AssessmentBreakdown>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _breakdownRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(b => b.StudentSubject)
                        .ThenInclude(ss => ss.Student)
                    .Include(b => b.StudentSubject)
                        .ThenInclude(ss => ss.Subject));

            var breakdownDtos = _mapper.Map<List<AssessmentBreakdownDto>>(paginatedResult.Data);

            return new PaginatedResult<AssessmentBreakdownDto>(
                breakdownDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        public async Task<AssessmentBreakdownDto> UpdateAssessmentBreakdownAsync(long id, UpdateAssessmentBreakdownDto updateDto)
        {
            var existingBreakdown = await _breakdownRepository.GetAsync(
                predicate: b => b.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Assessment breakdown with ID {id} not found");

            _mapper.Map(updateDto, existingBreakdown);
            await _breakdownRepository.UpdateAsync(existingBreakdown);
            return _mapper.Map<AssessmentBreakdownDto>(existingBreakdown);
        }

        public async Task<bool> DeleteAssessmentBreakdownAsync(long id)
        {
            var breakdown = await _breakdownRepository.GetAsync(
                predicate: b => b.Id == id,
                disableTracking: false);

            if (breakdown == null)
                return false;

            await _breakdownRepository.DeleteAsync(breakdown);
            return true;
        }

        public async Task<IEnumerable<AssessmentBreakdownDto>> GetBreakdownsByStudentSubjectAsync(long studentSubjectId)
        {
            var breakdowns = await _breakdownRepository.GetManyAsync(
                predicate: b => b.StudentSubjectId == studentSubjectId,
                orderBy: query => query.OrderBy(b => b.AssessmentType),
                include: query => query
                    .Include(b => b.StudentSubject)
                        .ThenInclude(ss => ss.Student)
                    .Include(b => b.StudentSubject)
                        .ThenInclude(ss => ss.Subject));

            return _mapper.Map<List<AssessmentBreakdownDto>>(breakdowns);
        }

        public async Task RecalculateBreakdownAsync(long studentSubjectId)
        {
            // First, get the StudentSubject to get StudentId and SubjectId
            var studentSubject = await _studentSubjectRepository.GetAsync(
                predicate: ss => ss.Id == studentSubjectId,
                include: query => query
                    .Include(ss => ss.Student)
                    .Include(ss => ss.Subject));

            if (studentSubject == null)
            {
                throw new KeyNotFoundException($"StudentSubject with ID {studentSubjectId} not found");
            }

            // Get all assessments for this student and subject combination
            var assessments = await _assessmentRepository.GetManyAsync(
                predicate: a => a.StudentId == studentSubject.StudentId &&
                               a.SubjectId == studentSubject.SubjectId);

            if (!assessments.Any())
            {
                // Remove any existing breakdowns if no assessments
                await _breakdownRepository.DeleteAsync(b => b.StudentSubjectId == studentSubjectId);
                return;
            }

            // Group assessments by type and calculate breakdown
            var assessmentGroups = assessments
                .GroupBy(a => a.Type)
                .Select(g => new
                {
                    Type = g.Key,
                    Count = g.Count(),
                    Average = g.Average(a => a.Score / a.MaxScore * 100)
                })
                .ToList();

            // Get existing breakdowns
            var existingBreakdowns = await _breakdownRepository.GetManyAsync(
                predicate: b => b.StudentSubjectId == studentSubjectId);

            // Update or create breakdowns
            foreach (var group in assessmentGroups)
            {
                var existing = existingBreakdowns.FirstOrDefault(b => b.AssessmentType == group.Type);

                if (existing != null)
                {
                    existing.Count = group.Count;
                    existing.Average = group.Average;
                    await _breakdownRepository.UpdateAsync(existing);
                }
                else
                {
                    var newBreakdown = new AssessmentBreakdown
                    {
                        StudentSubjectId = studentSubjectId,
                        AssessmentType = group.Type,
                        Count = group.Count,
                        Average = group.Average
                    };
                    await _breakdownRepository.AddAsync(newBreakdown);
                }
            }

            // Remove breakdowns for types that no longer exist
            var typesToRemove = existingBreakdowns
                .Where(b => !assessmentGroups.Any(g => g.Type == b.AssessmentType))
                .ToList();

            foreach (var breakdown in typesToRemove)
            {
                await _breakdownRepository.DeleteAsync(breakdown);
            }
        }

        public async Task RecalculateAllBreakdownsAsync()
        {
            // Get all student subjects
            var studentSubjects = await _studentSubjectRepository.GetManyAsync();

            foreach (var studentSubject in studentSubjects)
            {
                await RecalculateBreakdownAsync(studentSubject.Id);
            }
        }

        private Expression<Func<AssessmentBreakdown, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user) || UserContextHelper.IsAdmin(user))
            {
                return null;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return b => b.StudentSubject.Student.TeacherStudents.Any(ts => ts.Teacher.UserId == userId);
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return b => b.StudentSubject.Student.TutorStudents.Any(ts => ts.Tutor.UserId == userId);
            }
            else if (UserContextHelper.IsParent(user))
            {
                return b => b.StudentSubject.Student.ParentStudents.Any(ps => ps.Parent.UserId == userId);
            }
            else if (UserContextHelper.IsStudent(user))
            {
                return b => b.StudentSubject.Student.UserId == userId;
            }

            return b => false;
        }

        private Expression<Func<AssessmentBreakdown, bool>> CombinePredicates(
            Expression<Func<AssessmentBreakdown, bool>> left,
            Expression<Func<AssessmentBreakdown, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(AssessmentBreakdown), "b");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<AssessmentBreakdown, bool>>(combined, parameter);
        }

        private Func<IQueryable<AssessmentBreakdown>, IOrderedQueryable<AssessmentBreakdown>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "assessmenttype" => sortDescending
                    ? query => query.OrderByDescending(b => b.AssessmentType)
                    : query => query.OrderBy(b => b.AssessmentType),
                "average" => sortDescending
                    ? query => query.OrderByDescending(b => b.Average)
                    : query => query.OrderBy(b => b.Average),
                "count" => sortDescending
                    ? query => query.OrderByDescending(b => b.Count)
                    : query => query.OrderBy(b => b.Count),
                _ => sortDescending
                    ? query => query.OrderByDescending(b => b.Id)
                    : query => query.OrderBy(b => b.Id)
            };
        }
    }
}