namespace Aptiverse.Api.Domain.Models.Students
{
    public class StudentSubjectTopic
    {
        public long Id { get; set; }
        public long StudentSubjectId { get; set; }
        public long TopicId { get; set; }
        public double Score { get; set; }
        public string Trend { get; set; } // "up", "down", "stable"
        public DateTime LastTested { get; set; }

        public virtual StudentSubject StudentSubject { get; set; }
        public virtual Topic Topic { get; set; }
    }
}
