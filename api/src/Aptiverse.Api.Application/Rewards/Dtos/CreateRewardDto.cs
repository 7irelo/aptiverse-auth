namespace Aptiverse.Api.Application.Rewards.Dtos
{
    public record CreateRewardDto
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public string RewardType { get; init; }
        public int PointsCost { get; init; }
        public int DifficultyTier { get; init; }
        public bool IsActive { get; init; } = true;
        public int StockQuantity { get; init; } = -1;
    }
}