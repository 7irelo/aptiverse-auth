using Aptiverse.Api.Domain.Models.Admins;

namespace Aptiverse.Api.Domain.Models.Teachers
{
    public class TeacherAdmin
    {
        public long Id { get; set; }
        public long TeacherId { get; set; }
        public long AdminId { get; set; }
        public DateTime AssociatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string Role { get; set; }

        public virtual Teacher Teacher { get; set; }
        public virtual Admin Admin { get; set; }
    }
}
