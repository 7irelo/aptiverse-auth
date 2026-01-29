namespace Aptiverse.Api.Application.GradeDistributions.Dtos
{
    public record GradeDistributionDto
    {
        public long Id { get; init; }
        public long StudentSubjectId { get; init; }
        public string Grade { get; init; }
        public int Count { get; init; }
    }
}