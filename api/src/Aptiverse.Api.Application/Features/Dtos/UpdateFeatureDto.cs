namespace Aptiverse.Api.Application.Features.Dtos
{
    public record UpdateFeatureDto
    {
        public string Name { get; init; }
        public string Code { get; init; }
        public string Description { get; init; }
        public string Category { get; init; }
        public decimal BasePrice { get; init; }
        public string PriceCurrency { get; init; }
        public string BillingCycle { get; init; }
        public int ComplexityWeight { get; init; }
        public bool IsActive { get; init; }
    }
}