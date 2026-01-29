namespace Aptiverse.Api.Application.StudentSubjectTopics.Dtos
{
    public record CreateStudentSubjectTopicDto
    {
        public long StudentSubjectId { get; init; }
        public long TopicId { get; init; }
        public double Score { get; init; }
        public string Trend { get; init; }
        public DateTime LastTested { get; init; }
    }
}