namespace Aptiverse.Api.Application.Admins.Dtos
{
    public record AdminDto
    {
        public long Id { get; init; }
        public string UserId { get; init; }
        public string SchoolName { get; init; }
        public string SchoolCode { get; init; }
        public string ContactNumber { get; init; }
        public string Address { get; init; }
        public DateTime CreatedAt { get; init; }
        public bool IsActive { get; init; }
        public int StudentCount { get; init; }
        public int TeacherCount { get; init; }
    }
}
