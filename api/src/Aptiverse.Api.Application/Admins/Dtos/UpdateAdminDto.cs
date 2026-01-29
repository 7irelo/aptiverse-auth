namespace Aptiverse.Api.Application.Admins.Dtos
{
    public record UpdateAdminDto
    {
        public string? SchoolName { get; init; }
        public string? SchoolCode { get; init; }
        public string? ContactNumber { get; init; }
        public string? Address { get; init; }
        public bool? IsActive { get; init; }
    }
}
