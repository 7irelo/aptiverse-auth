namespace Aptiverse.Api.Domain.Models.Students
{
    public class Topic
    {
        public long Id { get; set; }
        public string SubjectId { get; set; }
        public string Name { get; set; }

        public virtual Subject Subject { get; set; }
        public virtual ICollection<StudentSubjectTopic> StudentSubjectTopics { get; set; }
    }
}
