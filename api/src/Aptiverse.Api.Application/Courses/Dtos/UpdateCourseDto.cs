namespace Aptiverse.Api.Application.Courses.Dtos
{
    public record UpdateCourseDto
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public string SubjectId { get; init; }
        public long? TeacherId { get; init; }
        public long? TutorId { get; init; }
        public decimal Price { get; init; }
        public string Currency { get; init; }
        public string Level { get; init; }
        public string ThumbnailUrl { get; init; }
        public string PreviewVideoUrl { get; init; }
        public double Rating { get; init; }
        public int TotalStudents { get; init; }
        public int TotalLessons { get; init; }
        public decimal TotalHours { get; init; }
        public bool IsPublished { get; init; }
    }
}