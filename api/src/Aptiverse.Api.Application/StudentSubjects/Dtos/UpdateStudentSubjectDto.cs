namespace Aptiverse.Api.Application.StudentSubjects.Dtos
{
    public record UpdateStudentSubjectDto
    {
        public long? StudentId { get; init; }
        public string? SubjectId { get; init; }
        public int? Progress { get; init; }
        public int? Target { get; init; }
        public double? AverageScore { get; init; }
        public int? StudyHours { get; init; }
        public int? AssignmentsCompleted { get; init; }
        public int? UpcomingDeadlines { get; init; }
        public string? Strength { get; init; }
        public string? Weakness { get; init; }
        public DateTime? LastActivity { get; init; }
        public string? PerformanceTrend { get; init; }
        public double? StudyEfficiency { get; init; }
        public double? PredictedScore { get; init; }
        public double? DifficultyLevel { get; init; }
        public double? ConfidenceLevel { get; init; }
        public double? LearningVelocity { get; init; }
        public double? RetentionRate { get; init; }
    }
}