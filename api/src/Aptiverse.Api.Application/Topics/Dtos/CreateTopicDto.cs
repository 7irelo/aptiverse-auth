namespace Aptiverse.Api.Application.Topics.Dtos
{
    public record CreateTopicDto
    {
        public string SubjectId { get; init; }
        public string Name { get; init; }
    }
}