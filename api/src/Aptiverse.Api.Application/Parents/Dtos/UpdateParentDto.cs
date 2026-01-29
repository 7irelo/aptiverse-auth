namespace Aptiverse.Api.Application.Parents.Dtos
{
    public record UpdateParentDto
    {
        public string ContactNumber { get; init; }
        public string Address { get; init; }
        public string Occupation { get; init; }
    }
}