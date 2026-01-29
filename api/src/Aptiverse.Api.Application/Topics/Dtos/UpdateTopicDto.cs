namespace Aptiverse.Api.Application.Topics.Dtos
{
    public record UpdateTopicDto
    {
        public string? SubjectId { get; init; }
        public string? Name { get; init; }
    }
}