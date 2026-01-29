namespace Aptiverse.Api.Application.StudentSubjectTopics.Dtos
{
    public record StudentSubjectTopicDto
    {
        public long Id { get; init; }
        public long StudentSubjectId { get; init; }
        public long TopicId { get; init; }
        public double Score { get; init; }
        public string Trend { get; init; }
        public DateTime LastTested { get; init; }
    }
}