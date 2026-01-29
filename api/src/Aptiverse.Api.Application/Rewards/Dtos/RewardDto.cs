namespace Aptiverse.Api.Application.Rewards.Dtos
{
    public record RewardDto
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string RewardType { get; init; }
        public int PointsCost { get; init; }
        public int DifficultyTier { get; init; }
        public bool IsActive { get; init; }
        public int StockQuantity { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}