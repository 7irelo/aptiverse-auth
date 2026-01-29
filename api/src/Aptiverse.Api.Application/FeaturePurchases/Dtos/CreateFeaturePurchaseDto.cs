namespace Aptiverse.Api.Application.FeaturePurchases.Dtos
{
    public record CreateFeaturePurchaseDto
    {
        public string UserId { get; init; }
        public long FeatureId { get; init; }
        public decimal AmountPaid { get; init; }
        public string Currency { get; init; }
        public string PaymentStatus { get; init; }
        public string BillingCycle { get; init; }
        public DateTime? ActivationDate { get; init; }
        public DateTime? ExpiryDate { get; init; }
    }
}