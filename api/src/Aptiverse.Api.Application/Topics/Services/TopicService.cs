using Aptiverse.Api.Application.Topics.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.Topics.Services
{
    public class TopicService(
        IRepository<Topic> topicRepository,
        IMapper mapper) : ITopicService
    {
        private readonly IRepository<Topic> _topicRepository = topicRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<TopicDto> CreateTopicAsync(CreateTopicDto createTopicDto)
        {
            ArgumentNullException.ThrowIfNull(createTopicDto);

            Topic topic = _mapper.Map<Topic>(createTopicDto);
            await _topicRepository.AddAsync(topic);
            return _mapper.Map<TopicDto>(topic);
        }

        public async Task<TopicDto?> GetTopicByIdAsync(long id)
        {
            var topic = await _topicRepository.GetAsync(
                predicate: t => t.Id == id,
                include: query => query
                    .Include(t => t.Subject)
                    .Include(t => t.StudentSubjectTopics),
                disableTracking: false);

            if (topic == null)
                return null;

            return _mapper.Map<TopicDto>(topic);
        }

        public async Task<PaginatedResult<TopicDto>> GetTopicsAsync(
            string? subjectId = null,
            string? search = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<Topic, bool>>? predicate = BuildFilterPredicate(subjectId, search);
            Func<IQueryable<Topic>, IOrderedQueryable<Topic>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _topicRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(t => t.Subject)
                    .Include(t => t.StudentSubjectTopics));

            var topicDtos = _mapper.Map<List<TopicDto>>(paginatedResult.Data);

            return new PaginatedResult<TopicDto>(
                topicDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<Topic, bool>>? BuildFilterPredicate(string? subjectId, string? search)
        {
            if (string.IsNullOrEmpty(subjectId) && string.IsNullOrEmpty(search))
                return null;

            return t =>
                (string.IsNullOrEmpty(subjectId) || t.SubjectId == subjectId) &&
                (string.IsNullOrEmpty(search) || t.Name.Contains(search));
        }

        private Func<IQueryable<Topic>, IOrderedQueryable<Topic>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "name" => sortDescending
                    ? query => query.OrderByDescending(t => t.Name).ThenByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.Name).ThenBy(t => t.Id),
                "subjectid" => sortDescending
                    ? query => query.OrderByDescending(t => t.SubjectId).ThenByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.SubjectId).ThenBy(t => t.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.Id)
            };
        }

        public async Task<TopicDto> UpdateTopicAsync(long id, UpdateTopicDto updateTopicDto)
        {
            var existingTopic = await _topicRepository.GetAsync(
                predicate: t => t.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Topic with ID {id} not found");

            _mapper.Map(updateTopicDto, existingTopic);
            await _topicRepository.UpdateAsync(existingTopic);
            return _mapper.Map<TopicDto>(existingTopic);
        }

        public async Task<bool> DeleteTopicAsync(long id)
        {
            var topic = await _topicRepository.GetAsync(
                predicate: t => t.Id == id,
                disableTracking: false);

            if (topic == null)
                return false;

            await _topicRepository.DeleteAsync(topic);
            return true;
        }

        public async Task<int> CountTopicsAsync(string? subjectId = null, string? search = null)
        {
            if (string.IsNullOrEmpty(subjectId) && string.IsNullOrEmpty(search))
                return await _topicRepository.CountAsync();

            Expression<Func<Topic, bool>> predicate = t =>
                (string.IsNullOrEmpty(subjectId) || t.SubjectId == subjectId) &&
                (string.IsNullOrEmpty(search) || t.Name.Contains(search));

            return await _topicRepository.CountAsync(predicate);
        }

        public async Task<bool> TopicExistsAsync(long id)
        {
            return await _topicRepository.ExistsAsync(t => t.Id == id);
        }
    }
}