namespace Aptiverse.Api.Domain.Models.Students
{
    public class Assessment
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public string SubjectId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public double Score { get; set; }
        public double MaxScore { get; set; }
        public DateTime DateTaken { get; set; }
        public string Grade { get; set; }

        public virtual Student Student { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
