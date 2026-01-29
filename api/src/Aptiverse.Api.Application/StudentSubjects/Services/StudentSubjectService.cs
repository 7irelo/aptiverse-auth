using Aptiverse.Api.Application.StudentSubjects.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.StudentSubjects.Services
{
    public class StudentSubjectService(
        IRepository<StudentSubject> studentSubjectRepository,
        IMapper mapper) : IStudentSubjectService
    {
        private readonly IRepository<StudentSubject> _studentSubjectRepository = studentSubjectRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<StudentSubjectDto> CreateStudentSubjectAsync(CreateStudentSubjectDto createStudentSubjectDto)
        {
            ArgumentNullException.ThrowIfNull(createStudentSubjectDto);

            StudentSubject studentSubject = _mapper.Map<StudentSubject>(createStudentSubjectDto);
            await _studentSubjectRepository.AddAsync(studentSubject);
            return _mapper.Map<StudentSubjectDto>(studentSubject);
        }

        public async Task<StudentSubjectDto?> GetStudentSubjectByIdAsync(long id)
        {
            var studentSubject = await _studentSubjectRepository.GetAsync(
                predicate: ss => ss.Id == id,
                include: query => query
                    .Include(ss => ss.Student)
                    .Include(ss => ss.Subject)
                    .Include(ss => ss.Analytics)
                    .Include(ss => ss.WeeklyStudyHours)
                    .Include(ss => ss.TopicPerformances)
                    .Include(ss => ss.ImprovementTips)
                    .Include(ss => ss.KnowledgeGaps),
                disableTracking: false);

            if (studentSubject == null)
                return null;

            return _mapper.Map<StudentSubjectDto>(studentSubject);
        }

        public async Task<PaginatedResult<StudentSubjectDto>> GetStudentSubjectsAsync(
            long? studentId = null,
            string? subjectId = null,
            int? minProgress = null,
            int? maxProgress = null,
            double? minAverageScore = null,
            double? maxAverageScore = null,
            string? performanceTrend = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<StudentSubject, bool>>? predicate = BuildFilterPredicate(
                studentId, subjectId, minProgress, maxProgress, minAverageScore, maxAverageScore, performanceTrend);

            Func<IQueryable<StudentSubject>, IOrderedQueryable<StudentSubject>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _studentSubjectRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(ss => ss.Student)
                    .Include(ss => ss.Subject));

            var studentSubjectDtos = _mapper.Map<List<StudentSubjectDto>>(paginatedResult.Data);

            return new PaginatedResult<StudentSubjectDto>(
                studentSubjectDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<StudentSubject, bool>>? BuildFilterPredicate(
            long? studentId,
            string? subjectId,
            int? minProgress,
            int? maxProgress,
            double? minAverageScore,
            double? maxAverageScore,
            string? performanceTrend)
        {
            if (!studentId.HasValue && string.IsNullOrEmpty(subjectId) &&
                !minProgress.HasValue && !maxProgress.HasValue &&
                !minAverageScore.HasValue && !maxAverageScore.HasValue &&
                string.IsNullOrEmpty(performanceTrend))
                return null;

            return ss =>
                (!studentId.HasValue || ss.StudentId == studentId.Value) &&
                (string.IsNullOrEmpty(subjectId) || ss.SubjectId == subjectId) &&
                (!minProgress.HasValue || ss.Progress >= minProgress.Value) &&
                (!maxProgress.HasValue || ss.Progress <= maxProgress.Value) &&
                (!minAverageScore.HasValue || ss.AverageScore >= minAverageScore.Value) &&
                (!maxAverageScore.HasValue || ss.AverageScore <= maxAverageScore.Value) &&
                (string.IsNullOrEmpty(performanceTrend) || ss.PerformanceTrend == performanceTrend);
        }

        private Func<IQueryable<StudentSubject>, IOrderedQueryable<StudentSubject>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "progress" => sortDescending
                    ? query => query.OrderByDescending(ss => ss.Progress).ThenByDescending(ss => ss.Id)
                    : query => query.OrderBy(ss => ss.Progress).ThenBy(ss => ss.Id),
                "averagescore" => sortDescending
                    ? query => query.OrderByDescending(ss => ss.AverageScore).ThenByDescending(ss => ss.Id)
                    : query => query.OrderBy(ss => ss.AverageScore).ThenBy(ss => ss.Id),
                "studyhours" => sortDescending
                    ? query => query.OrderByDescending(ss => ss.StudyHours).ThenByDescending(ss => ss.Id)
                    : query => query.OrderBy(ss => ss.StudyHours).ThenBy(ss => ss.Id),
                "assignmentscompleted" => sortDescending
                    ? query => query.OrderByDescending(ss => ss.AssignmentsCompleted).ThenByDescending(ss => ss.Id)
                    : query => query.OrderBy(ss => ss.AssignmentsCompleted).ThenBy(ss => ss.Id),
                "lastactivity" => sortDescending
                    ? query => query.OrderByDescending(ss => ss.LastActivity).ThenByDescending(ss => ss.Id)
                    : query => query.OrderBy(ss => ss.LastActivity).ThenBy(ss => ss.Id),
                "predictedscore" => sortDescending
                    ? query => query.OrderByDescending(ss => ss.PredictedScore).ThenByDescending(ss => ss.Id)
                    : query => query.OrderBy(ss => ss.PredictedScore).ThenBy(ss => ss.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(ss => ss.Id)
                    : query => query.OrderBy(ss => ss.Id)
            };
        }

        public async Task<StudentSubjectDto> UpdateStudentSubjectAsync(long id, UpdateStudentSubjectDto updateStudentSubjectDto)
        {
            var existingStudentSubject = await _studentSubjectRepository.GetAsync(
                predicate: ss => ss.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"StudentSubject with ID {id} not found");

            _mapper.Map(updateStudentSubjectDto, existingStudentSubject);
            await _studentSubjectRepository.UpdateAsync(existingStudentSubject);
            return _mapper.Map<StudentSubjectDto>(existingStudentSubject);
        }

        public async Task<bool> DeleteStudentSubjectAsync(long id)
        {
            var studentSubject = await _studentSubjectRepository.GetAsync(
                predicate: ss => ss.Id == id,
                disableTracking: false);

            if (studentSubject == null)
                return false;

            await _studentSubjectRepository.DeleteAsync(studentSubject);
            return true;
        }

        public async Task<int> CountStudentSubjectsAsync(long? studentId = null, string? subjectId = null)
        {
            if (!studentId.HasValue && string.IsNullOrEmpty(subjectId))
                return await _studentSubjectRepository.CountAsync();

            Expression<Func<StudentSubject, bool>> predicate = ss =>
                (!studentId.HasValue || ss.StudentId == studentId.Value) &&
                (string.IsNullOrEmpty(subjectId) || ss.SubjectId == subjectId);

            return await _studentSubjectRepository.CountAsync(predicate);
        }

        public async Task<bool> StudentSubjectExistsAsync(long id)
        {
            return await _studentSubjectRepository.ExistsAsync(ss => ss.Id == id);
        }

        public async Task<bool> StudentSubjectExistsForStudentAndSubjectAsync(long studentId, string subjectId)
        {
            return await _studentSubjectRepository.ExistsAsync(ss => ss.StudentId == studentId && ss.SubjectId == subjectId);
        }
    }
}