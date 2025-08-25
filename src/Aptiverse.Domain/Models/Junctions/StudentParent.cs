using Aptiverse.Domain.Models.Parents;
using Aptiverse.Domain.Models.Students;

namespace Aptiverse.Domain.Models.Junctions
{
    public class StudentParent
    {
        public long StudentId { get; set; }
        public Student Student { get; set; }
        public long ParentId { get; set; }
        public Parent Parent { get; set; }
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    }
}
