namespace Aptiverse.Api.Application.PredictionMetricss.Dtos
{
    public record CreatePredictionMetricsDto
    {
        public long StudentSubjectId { get; init; }
        public int FinalGradeProbabilityA { get; init; }
        public int FinalGradeProbabilityB { get; init; }
        public int FinalGradeProbabilityC { get; init; }
        public int FinalGradeProbabilityD { get; init; }
        public string RiskLevel { get; init; }
        public bool InterventionNeeded { get; init; }
    }
}