namespace Aptiverse.Api.Application.CourseModules.Dtos
{
    public record UpdateCourseModuleDto
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public int Order { get; init; }
        public decimal DurationHours { get; init; }
    }
}