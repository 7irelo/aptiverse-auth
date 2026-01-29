namespace Aptiverse.Api.Domain.Models.Students
{
    public class PredictionMetrics
    {
        public long Id { get; set; }
        public long StudentSubjectId { get; set; }
        public int FinalGradeProbabilityA { get; set; }
        public int FinalGradeProbabilityB { get; set; }
        public int FinalGradeProbabilityC { get; set; }
        public int FinalGradeProbabilityD { get; set; }
        public string RiskLevel { get; set; } // "low", "medium", "high"
        public bool InterventionNeeded { get; set; }

        public virtual StudentSubject StudentSubject { get; set; }
    }
}
