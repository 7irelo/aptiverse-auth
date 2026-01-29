namespace Aptiverse.Api.Application.PeerComparisons.Dtos
{
    public record PeerComparisonDto
    {
        public long Id { get; init; }
        public long StudentSubjectId { get; init; }
        public double ClassAverage { get; init; }
        public int Percentile { get; init; }
        public int Ranking { get; init; }
        public string TrendComparison { get; init; }
    }
}