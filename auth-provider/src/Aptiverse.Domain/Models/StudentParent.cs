namespace Aptiverse.Domain.Models
{
    public class StudentParent
    {
        public long StudentId { get; set; }
        public Student? Student { get; set; }
        public long ParentId { get; set; }
        public Parent? Parent { get; set; }
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    }
}
