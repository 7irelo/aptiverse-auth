namespace Aptiverse.Api.Domain.Models.Students
{
    public class PeerComparison
    {
        public long Id { get; set; }
        public long StudentSubjectId { get; set; }
        public double ClassAverage { get; set; }
        public int Percentile { get; set; }
        public int Ranking { get; set; }
        public string TrendComparison { get; set; } // "above_average", "average", "below_average"

        public virtual StudentSubject StudentSubject { get; set; }
    }
}
