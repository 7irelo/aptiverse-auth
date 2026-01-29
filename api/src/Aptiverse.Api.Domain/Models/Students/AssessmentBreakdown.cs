namespace Aptiverse.Api.Domain.Models.Students
{
    public class AssessmentBreakdown
    {
        public long Id { get; set; }
        public long StudentSubjectId { get; set; }
        public string AssessmentType { get; set; } // "tests", "quizzes", etc.
        public int Count { get; set; }
        public double Average { get; set; }

        public virtual StudentSubject StudentSubject { get; set; }
    }
}
