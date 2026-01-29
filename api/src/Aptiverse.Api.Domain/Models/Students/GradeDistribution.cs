namespace Aptiverse.Api.Domain.Models.Students
{
    public class GradeDistribution
    {
        public long Id { get; set; }
        public long StudentSubjectId { get; set; }
        public string Grade { get; set; } // "A", "B", "C", "D", "F"
        public int Count { get; set; }

        public virtual StudentSubject StudentSubject { get; set; }
    }
}
