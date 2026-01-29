namespace Aptiverse.Api.Application.Topics.Dtos
{
    public record TopicDto
    {
        public long Id { get; init; }
        public string SubjectId { get; init; }
        public string Name { get; init; }
    }
}