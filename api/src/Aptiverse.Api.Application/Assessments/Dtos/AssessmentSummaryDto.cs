namespace Aptiverse.Api.Application.Assessments.Dtos
{
    public record AssessmentSummaryDto
    {
        public string SubjectId { get; init; }
        public string SubjectName { get; init; }
        public int TotalAssessments { get; init; }
        public double AverageScore { get; init; }
        public double HighestScore { get; init; }
        public double LowestScore { get; init; }
    }
}
