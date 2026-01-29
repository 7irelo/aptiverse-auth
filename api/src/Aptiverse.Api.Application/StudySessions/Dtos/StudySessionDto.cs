namespace Aptiverse.Api.Application.StudySessions.Dtos
{
    public record StudySessionDto
    {
        public long Id { get; init; }
        public long StudentId { get; init; }
        public string SubjectId { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
        public int DurationMinutes { get; init; }
        public string SessionType { get; init; }
        public string TopicsCovered { get; init; }
        public double EfficiencyScore { get; init; }
        public int ConcentrationLevel { get; init; }
        public string Notes { get; init; }
        public string ResourcesUsed { get; init; }
    }
}