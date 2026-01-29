namespace Aptiverse.Api.Application.TeacherAdmins.Dtos
{
    public record TeacherAdminDto
    {
        public long Id { get; init; }
        public long TeacherId { get; init; }
        public long AdminId { get; init; }
        public DateTime AssociatedAt { get; init; }
        public bool IsActive { get; init; }
        public string Role { get; init; }
    }
}