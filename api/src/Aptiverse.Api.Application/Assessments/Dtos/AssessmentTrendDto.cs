namespace Aptiverse.Api.Application.Assessments.Dtos
{
    public record AssessmentTrendDto
    {
        public DateTime Date { get; init; }
        public double Score { get; init; }
        public double AverageScore { get; init; }
        public string Type { get; init; }
    }
}
