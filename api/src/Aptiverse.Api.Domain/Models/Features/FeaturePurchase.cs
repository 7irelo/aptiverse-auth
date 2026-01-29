namespace Aptiverse.Api.Domain.Models.Features
{
    public class FeaturePurchase
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long FeatureId { get; set; }
        public decimal AmountPaid { get; set; }
        public string Currency { get; set; } = "ZAR";
        public string PaymentStatus { get; set; } // "Pending", "Completed", "Failed", "Refunded"
        public string BillingCycle { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        // Navigation properties
        public virtual Feature Feature { get; set; }
    }
}
