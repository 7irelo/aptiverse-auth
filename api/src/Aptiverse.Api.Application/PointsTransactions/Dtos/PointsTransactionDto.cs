namespace Aptiverse.Api.Application.PointsTransactions.Dtos
{
    public record PointsTransactionDto
    {
        public long Id { get; init; }
        public long StudentPointsId { get; init; }
        public int Points { get; init; }
        public string TransactionType { get; init; }
        public string Source { get; init; }
        public long? RelatedGoalId { get; init; }
        public long? RelatedRewardId { get; init; }
        public string Description { get; init; }
        public DateTime TransactionDate { get; init; }
    }
}