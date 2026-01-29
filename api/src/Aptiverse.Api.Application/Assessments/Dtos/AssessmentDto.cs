namespace Aptiverse.Api.Application.Assessments.Dtos
{
    public record AssessmentDto
    {
        public long Id { get; init; }
        public long StudentId { get; init; }
        public string SubjectId { get; init; }
        public string Type { get; init; }
        public string Title { get; init; }
        public double Score { get; init; }
        public double MaxScore { get; init; }
        public double Percentage => MaxScore > 0 ? (Score / MaxScore) * 100 : 0;
        public DateTime DateTaken { get; init; }
        public string Grade { get; init; }
        public string? StudentName { get; init; }
        public string? SubjectName { get; init; }
    }
}
