namespace Aptiverse.Api.Application.Subjects.Dtos
{
    public record CreateSubjectDto
    {
        public string Name { get; init; }
        public string Code { get; init; }
        public string Description { get; init; }
        public string Color { get; init; }
        public string TextColor { get; init; }
        public string BorderColor { get; init; }
    }
}