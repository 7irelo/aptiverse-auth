namespace Aptiverse.Api.Application.Parents.Dtos
{
    public record ParentDto
    {
        public long Id { get; init; }
        public string UserId { get; init; }
        public string ContactNumber { get; init; }
        public string Address { get; init; }
        public string Occupation { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}