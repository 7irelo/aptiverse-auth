namespace Aptiverse.Api.Domain.Models.Courses
{
    public class CourseModule
    {
        public long Id { get; set; }
        public long CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public decimal DurationHours { get; set; }

        public virtual Course Course { get; set; }
        public virtual ICollection<ModuleLesson> Lessons { get; set; }
    }
}
