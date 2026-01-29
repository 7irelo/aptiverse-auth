namespace Aptiverse.Api.Domain.Models.Students
{
    public class Subject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
        public string BorderColor { get; set; }
        public virtual ICollection<StudentSubject> StudentSubjects { get; set; }
        public virtual ICollection<Topic> Topics { get; set; }
    }
}
