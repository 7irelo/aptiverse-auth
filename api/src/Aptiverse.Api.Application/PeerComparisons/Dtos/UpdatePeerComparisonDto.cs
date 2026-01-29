namespace Aptiverse.Api.Application.PeerComparisons.Dtos
{
    public record UpdatePeerComparisonDto
    {
        public long? StudentSubjectId { get; init; }
        public double? ClassAverage { get; init; }
        public int? Percentile { get; init; }
        public int? Ranking { get; init; }
        public string? TrendComparison { get; init; }
    }
}