namespace Aptiverse.Api.Application.ModuleLessons.Dtos
{
    public record UpdateModuleLessonDto
    {
        public string Title { get; init; }
        public string Content { get; init; }
        public string VideoUrl { get; init; }
        public string ResourceUrls { get; init; }
        public int Order { get; init; }
        public decimal DurationMinutes { get; init; }
        public bool IsFreePreview { get; init; }
    }
}