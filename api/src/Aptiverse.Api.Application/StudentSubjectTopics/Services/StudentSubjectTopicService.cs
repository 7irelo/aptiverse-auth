using Aptiverse.Api.Application.StudentSubjectTopics.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.StudentSubjectTopics.Services
{
    public class StudentSubjectTopicService(
        IRepository<StudentSubjectTopic> studentSubjectTopicRepository,
        IMapper mapper) : IStudentSubjectTopicService
    {
        private readonly IRepository<StudentSubjectTopic> _studentSubjectTopicRepository = studentSubjectTopicRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<StudentSubjectTopicDto> CreateStudentSubjectTopicAsync(CreateStudentSubjectTopicDto createStudentSubjectTopicDto)
        {
            ArgumentNullException.ThrowIfNull(createStudentSubjectTopicDto);

            StudentSubjectTopic studentSubjectTopic = _mapper.Map<StudentSubjectTopic>(createStudentSubjectTopicDto);
            await _studentSubjectTopicRepository.AddAsync(studentSubjectTopic);
            return _mapper.Map<StudentSubjectTopicDto>(studentSubjectTopic);
        }

        public async Task<StudentSubjectTopicDto?> GetStudentSubjectTopicByIdAsync(long id)
        {
            var studentSubjectTopic = await _studentSubjectTopicRepository.GetAsync(
                predicate: sst => sst.Id == id,
                include: query => query
                    .Include(sst => sst.StudentSubject)
                    .Include(sst => sst.Topic),
                disableTracking: false);

            if (studentSubjectTopic == null)
                return null;

            return _mapper.Map<StudentSubjectTopicDto>(studentSubjectTopic);
        }

        public async Task<PaginatedResult<StudentSubjectTopicDto>> GetStudentSubjectTopicsAsync(
            long? studentSubjectId = null,
            long? topicId = null,
            double? minScore = null,
            double? maxScore = null,
            string? trend = null,
            DateTime? lastTestedAfter = null,
            DateTime? lastTestedBefore = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<StudentSubjectTopic, bool>>? predicate = BuildFilterPredicate(
                studentSubjectId, topicId, minScore, maxScore, trend, lastTestedAfter, lastTestedBefore);

            Func<IQueryable<StudentSubjectTopic>, IOrderedQueryable<StudentSubjectTopic>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _studentSubjectTopicRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(sst => sst.StudentSubject)
                    .Include(sst => sst.Topic));

            var studentSubjectTopicDtos = _mapper.Map<List<StudentSubjectTopicDto>>(paginatedResult.Data);

            return new PaginatedResult<StudentSubjectTopicDto>(
                studentSubjectTopicDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<StudentSubjectTopic, bool>>? BuildFilterPredicate(
            long? studentSubjectId,
            long? topicId,
            double? minScore,
            double? maxScore,
            string? trend,
            DateTime? lastTestedAfter,
            DateTime? lastTestedBefore)
        {
            if (!studentSubjectId.HasValue && !topicId.HasValue &&
                !minScore.HasValue && !maxScore.HasValue &&
                string.IsNullOrEmpty(trend) &&
                !lastTestedAfter.HasValue && !lastTestedBefore.HasValue)
                return null;

            return sst =>
                (!studentSubjectId.HasValue || sst.StudentSubjectId == studentSubjectId.Value) &&
                (!topicId.HasValue || sst.TopicId == topicId.Value) &&
                (!minScore.HasValue || sst.Score >= minScore.Value) &&
                (!maxScore.HasValue || sst.Score <= maxScore.Value) &&
                (string.IsNullOrEmpty(trend) || sst.Trend == trend) &&
                (!lastTestedAfter.HasValue || sst.LastTested >= lastTestedAfter.Value) &&
                (!lastTestedBefore.HasValue || sst.LastTested <= lastTestedBefore.Value);
        }

        private Func<IQueryable<StudentSubjectTopic>, IOrderedQueryable<StudentSubjectTopic>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "score" => sortDescending
                    ? query => query.OrderByDescending(sst => sst.Score).ThenByDescending(sst => sst.Id)
                    : query => query.OrderBy(sst => sst.Score).ThenBy(sst => sst.Id),
                "trend" => sortDescending
                    ? query => query.OrderByDescending(sst => sst.Trend).ThenByDescending(sst => sst.Id)
                    : query => query.OrderBy(sst => sst.Trend).ThenBy(sst => sst.Id),
                "lasttested" => sortDescending
                    ? query => query.OrderByDescending(sst => sst.LastTested).ThenByDescending(sst => sst.Id)
                    : query => query.OrderBy(sst => sst.LastTested).ThenBy(sst => sst.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(sst => sst.Id)
                    : query => query.OrderBy(sst => sst.Id)
            };
        }

        public async Task<StudentSubjectTopicDto> UpdateStudentSubjectTopicAsync(long id, UpdateStudentSubjectTopicDto updateStudentSubjectTopicDto)
        {
            var existingStudentSubjectTopic = await _studentSubjectTopicRepository.GetAsync(
                predicate: sst => sst.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"StudentSubjectTopic with ID {id} not found");

            _mapper.Map(updateStudentSubjectTopicDto, existingStudentSubjectTopic);
            await _studentSubjectTopicRepository.UpdateAsync(existingStudentSubjectTopic);
            return _mapper.Map<StudentSubjectTopicDto>(existingStudentSubjectTopic);
        }

        public async Task<bool> DeleteStudentSubjectTopicAsync(long id)
        {
            var studentSubjectTopic = await _studentSubjectTopicRepository.GetAsync(
                predicate: sst => sst.Id == id,
                disableTracking: false);

            if (studentSubjectTopic == null)
                return false;

            await _studentSubjectTopicRepository.DeleteAsync(studentSubjectTopic);
            return true;
        }

        public async Task<int> CountStudentSubjectTopicsAsync(long? studentSubjectId = null, long? topicId = null, string? trend = null)
        {
            if (!studentSubjectId.HasValue && !topicId.HasValue && string.IsNullOrEmpty(trend))
                return await _studentSubjectTopicRepository.CountAsync();

            Expression<Func<StudentSubjectTopic, bool>> predicate = sst =>
                (!studentSubjectId.HasValue || sst.StudentSubjectId == studentSubjectId.Value) &&
                (!topicId.HasValue || sst.TopicId == topicId.Value) &&
                (string.IsNullOrEmpty(trend) || sst.Trend == trend);

            return await _studentSubjectTopicRepository.CountAsync(predicate);
        }

        public async Task<bool> StudentSubjectTopicExistsAsync(long id)
        {
            return await _studentSubjectTopicRepository.ExistsAsync(sst => sst.Id == id);
        }

        public async Task<bool> StudentSubjectTopicExistsForStudentSubjectAndTopicAsync(long studentSubjectId, long topicId)
        {
            return await _studentSubjectTopicRepository.ExistsAsync(sst => sst.StudentSubjectId == studentSubjectId && sst.TopicId == topicId);
        }
    }
}