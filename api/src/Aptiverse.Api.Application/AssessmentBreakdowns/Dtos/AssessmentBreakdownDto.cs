namespace Aptiverse.Api.Application.AssessmentBreakdowns.Dtos
{
    public record AssessmentBreakdownDto
    {
        public long Id { get; init; }
        public long StudentSubjectId { get; init; }
        public string AssessmentType { get; init; }
        public int Count { get; init; }
        public double Average { get; init; }
        public string? StudentName { get; init; }
        public string? SubjectName { get; init; }
    }
}
