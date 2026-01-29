namespace Aptiverse.Api.Application.RewardFeatures.Dtos
{
    public record RewardFeatureDto
    {
        public long Id { get; init; }
        public long RewardId { get; init; }
        public long FeatureId { get; init; }
        public int DurationDays { get; init; }
        public int FeatureWeight { get; init; }
    }
}