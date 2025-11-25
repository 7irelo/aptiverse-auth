namespace Aptiverse.Domain.Models
{
    public class StudentAdmin
    {
        public long StudentId { get; set; }
        public Student? Student { get; set; }
        public long AdminId { get; set; }
        public Admin? Admin { get; set; }
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    }
}
