namespace Aptiverse.Api.Application.TeacherAdmins.Dtos
{
    public record CreateTeacherAdminDto
    {
        public long TeacherId { get; init; }
        public long AdminId { get; init; }
        public DateTime AssociatedAt { get; init; }
        public bool IsActive { get; init; }
        public string Role { get; init; }
    }
}