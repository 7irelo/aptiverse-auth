using Aptiverse.Domain.Models.Admins;
using Aptiverse.Domain.Models.Students;

namespace Aptiverse.Domain.Models.Junctions
{
    public class StudentAdmin
    {
        public long StudentId { get; set; }
        public Student Student { get; set; }
        public long AdminId { get; set; }
        public Admin Admin { get; set; }
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    }
}
