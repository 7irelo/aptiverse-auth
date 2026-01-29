namespace Aptiverse.Api.Domain.Models.Courses
{
    public class ModuleLesson
    {
        public long Id { get; set; }
        public long ModuleId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string VideoUrl { get; set; }
        public string ResourceUrls { get; set; }
        public int Order { get; set; }
        public decimal DurationMinutes { get; set; }
        public bool IsFreePreview { get; set; }

        public virtual CourseModule Module { get; set; }
    }
}
