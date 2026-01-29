namespace Aptiverse.Api.Application.RewardFeatures.Dtos
{
    public record UpdateRewardFeatureDto
    {
        public int DurationDays { get; init; }
        public int FeatureWeight { get; init; }
    }
}