namespace Aptiverse.Api.Application.AssessmentBreakdowns.Dtos
{
    public record CreateAssessmentBreakdownDto
    {
        public long StudentSubjectId { get; init; }
        public string AssessmentType { get; init; }
        public int Count { get; init; }
        public double Average { get; init; }
    }
}
