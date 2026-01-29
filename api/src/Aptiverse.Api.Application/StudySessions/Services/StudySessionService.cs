using Aptiverse.Api.Application.StudySessions.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.StudySessions.Services
{
    public class StudySessionService(
        IRepository<StudySession> studySessionRepository,
        IMapper mapper) : IStudySessionService
    {
        private readonly IRepository<StudySession> _studySessionRepository = studySessionRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<StudySessionDto> CreateStudySessionAsync(CreateStudySessionDto createStudySessionDto)
        {
            ArgumentNullException.ThrowIfNull(createStudySessionDto);

            StudySession studySession = _mapper.Map<StudySession>(createStudySessionDto);
            await _studySessionRepository.AddAsync(studySession);
            return _mapper.Map<StudySessionDto>(studySession);
        }

        public async Task<StudySessionDto?> GetStudySessionByIdAsync(long id)
        {
            var studySession = await _studySessionRepository.GetAsync(
                predicate: ss => ss.Id == id,
                include: query => query
                    .Include(ss => ss.Student)
                    .Include(ss => ss.Subject),
                disableTracking: false);

            if (studySession == null)
                return null;

            return _mapper.Map<StudySessionDto>(studySession);
        }

        public async Task<PaginatedResult<StudySessionDto>> GetStudySessionsAsync(
            long? studentId = null,
            string? subjectId = null,
            string? sessionType = null,
            DateTime? startAfter = null,
            DateTime? startBefore = null,
            int? minDurationMinutes = null,
            int? maxDurationMinutes = null,
            double? minEfficiencyScore = null,
            double? maxEfficiencyScore = null,
            int? minConcentrationLevel = null,
            int? maxConcentrationLevel = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<StudySession, bool>>? predicate = BuildFilterPredicate(
                studentId, subjectId, sessionType, startAfter, startBefore,
                minDurationMinutes, maxDurationMinutes, minEfficiencyScore,
                maxEfficiencyScore, minConcentrationLevel, maxConcentrationLevel);

            Func<IQueryable<StudySession>, IOrderedQueryable<StudySession>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _studySessionRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(ss => ss.Student)
                    .Include(ss => ss.Subject));

            var studySessionDtos = _mapper.Map<List<StudySessionDto>>(paginatedResult.Data);

            return new PaginatedResult<StudySessionDto>(
                studySessionDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<StudySession, bool>>? BuildFilterPredicate(
            long? studentId,
            string? subjectId,
            string? sessionType,
            DateTime? startAfter,
            DateTime? startBefore,
            int? minDurationMinutes,
            int? maxDurationMinutes,
            double? minEfficiencyScore,
            double? maxEfficiencyScore,
            int? minConcentrationLevel,
            int? maxConcentrationLevel)
        {
            if (!studentId.HasValue && string.IsNullOrEmpty(subjectId) &&
                string.IsNullOrEmpty(sessionType) && !startAfter.HasValue &&
                !startBefore.HasValue && !minDurationMinutes.HasValue &&
                !maxDurationMinutes.HasValue && !minEfficiencyScore.HasValue &&
                !maxEfficiencyScore.HasValue && !minConcentrationLevel.HasValue &&
                !maxConcentrationLevel.HasValue)
                return null;

            return ss =>
                (!studentId.HasValue || ss.StudentId == studentId.Value) &&
                (string.IsNullOrEmpty(subjectId) || ss.SubjectId == subjectId) &&
                (string.IsNullOrEmpty(sessionType) || ss.SessionType == sessionType) &&
                (!startAfter.HasValue || ss.StartTime >= startAfter.Value) &&
                (!startBefore.HasValue || ss.StartTime <= startBefore.Value) &&
                (!minDurationMinutes.HasValue || ss.DurationMinutes >= minDurationMinutes.Value) &&
                (!maxDurationMinutes.HasValue || ss.DurationMinutes <= maxDurationMinutes.Value) &&
                (!minEfficiencyScore.HasValue || ss.EfficiencyScore >= minEfficiencyScore.Value) &&
                (!maxEfficiencyScore.HasValue || ss.EfficiencyScore <= maxEfficiencyScore.Value) &&
                (!minConcentrationLevel.HasValue || ss.ConcentrationLevel >= minConcentrationLevel.Value) &&
                (!maxConcentrationLevel.HasValue || ss.ConcentrationLevel <= maxConcentrationLevel.Value);
        }

        private Func<IQueryable<StudySession>, IOrderedQueryable<StudySession>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "starttime" => sortDescending
                    ? query => query.OrderByDescending(ss => ss.StartTime).ThenByDescending(ss => ss.Id)
                    : query => query.OrderBy(ss => ss.StartTime).ThenBy(ss => ss.Id),
                "endtime" => sortDescending
                    ? query => query.OrderByDescending(ss => ss.EndTime).ThenByDescending(ss => ss.Id)
                    : query => query.OrderBy(ss => ss.EndTime).ThenBy(ss => ss.Id),
                "durationminutes" => sortDescending
                    ? query => query.OrderByDescending(ss => ss.DurationMinutes).ThenByDescending(ss => ss.Id)
                    : query => query.OrderBy(ss => ss.DurationMinutes).ThenBy(ss => ss.Id),
                "efficiencyscore" => sortDescending
                    ? query => query.OrderByDescending(ss => ss.EfficiencyScore).ThenByDescending(ss => ss.Id)
                    : query => query.OrderBy(ss => ss.EfficiencyScore).ThenBy(ss => ss.Id),
                "concentrationlevel" => sortDescending
                    ? query => query.OrderByDescending(ss => ss.ConcentrationLevel).ThenByDescending(ss => ss.Id)
                    : query => query.OrderBy(ss => ss.ConcentrationLevel).ThenBy(ss => ss.Id),
                "sessiontype" => sortDescending
                    ? query => query.OrderByDescending(ss => ss.SessionType).ThenByDescending(ss => ss.Id)
                    : query => query.OrderBy(ss => ss.SessionType).ThenBy(ss => ss.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(ss => ss.Id)
                    : query => query.OrderBy(ss => ss.Id)
            };
        }

        public async Task<StudySessionDto> UpdateStudySessionAsync(long id, UpdateStudySessionDto updateStudySessionDto)
        {
            var existingStudySession = await _studySessionRepository.GetAsync(
                predicate: ss => ss.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"StudySession with ID {id} not found");

            _mapper.Map(updateStudySessionDto, existingStudySession);
            await _studySessionRepository.UpdateAsync(existingStudySession);
            return _mapper.Map<StudySessionDto>(existingStudySession);
        }

        public async Task<bool> DeleteStudySessionAsync(long id)
        {
            var studySession = await _studySessionRepository.GetAsync(
                predicate: ss => ss.Id == id,
                disableTracking: false);

            if (studySession == null)
                return false;

            await _studySessionRepository.DeleteAsync(studySession);
            return true;
        }

        public async Task<int> CountStudySessionsAsync(long? studentId = null, string? subjectId = null, string? sessionType = null)
        {
            if (!studentId.HasValue && string.IsNullOrEmpty(subjectId) && string.IsNullOrEmpty(sessionType))
                return await _studySessionRepository.CountAsync();

            Expression<Func<StudySession, bool>> predicate = ss =>
                (!studentId.HasValue || ss.StudentId == studentId.Value) &&
                (string.IsNullOrEmpty(subjectId) || ss.SubjectId == subjectId) &&
                (string.IsNullOrEmpty(sessionType) || ss.SessionType == sessionType);

            return await _studySessionRepository.CountAsync(predicate);
        }

        public async Task<bool> StudySessionExistsAsync(long id)
        {
            return await _studySessionRepository.ExistsAsync(ss => ss.Id == id);
        }
    }
}