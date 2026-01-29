namespace Aptiverse.Api.Application.CourseModules.Dtos
{
    public record CourseModuleDto
    {
        public long Id { get; init; }
        public long CourseId { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public int Order { get; init; }
        public decimal DurationHours { get; init; }
    }
}