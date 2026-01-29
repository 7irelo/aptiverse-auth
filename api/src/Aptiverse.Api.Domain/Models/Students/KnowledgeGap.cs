namespace Aptiverse.Api.Domain.Models.Students
{
    public class KnowledgeGap
    {
        public long Id { get; set; }
        public long StudentSubjectId { get; set; }
        public string Concept { get; set; }
        public string Severity { get; set; } // "high", "medium", "low"
        public DateTime LastTested { get; set; }

        public virtual StudentSubject StudentSubject { get; set; }
    }
}
