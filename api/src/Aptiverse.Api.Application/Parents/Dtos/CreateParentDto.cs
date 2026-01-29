namespace Aptiverse.Api.Application.Parents.Dtos
{
    public record CreateParentDto
    {
        public string UserId { get; init; }
        public string ContactNumber { get; init; }
        public string Address { get; init; }
        public string Occupation { get; init; }
    }
}