using Aptiverse.Api.Application.Topics.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.Topics.Services
{
    public interface ITopicService
    {
        Task<TopicDto> CreateTopicAsync(CreateTopicDto createTopicDto);
        Task<TopicDto?> GetTopicByIdAsync(long id);
        Task<PaginatedResult<TopicDto>> GetTopicsAsync(
            string? subjectId = null,
            string? search = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<TopicDto> UpdateTopicAsync(long id, UpdateTopicDto updateTopicDto);
        Task<bool> DeleteTopicAsync(long id);
        Task<int> CountTopicsAsync(string? subjectId = null, string? search = null);
        Task<bool> TopicExistsAsync(long id);
    }
}