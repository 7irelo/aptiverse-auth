namespace Aptiverse.Api.Application.Assessments.Dtos
{
    public record UpdateAssessmentDto
    {
        public string? Type { get; init; }
        public string? Title { get; init; }
        public double? Score { get; init; }
        public double? MaxScore { get; init; }
        public DateTime? DateTaken { get; init; }
        public string? Grade { get; init; }
    }
}
