using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Models.Teachers;

namespace Aptiverse.Api.Domain.Models.Admins
{
    public class Admin
    {
        public long Id { get; set; }
        public string UserId { get; set; } 
        public string? SchoolName { get; set; }
        public string? SchoolCode { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Student>? Students { get; set; }
        public virtual ICollection<Teacher>? Teachers { get; set; }
    }
}
