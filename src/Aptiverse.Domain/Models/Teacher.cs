namespace Aptiverse.Domain.Models
{
    public class Teacher
    {
        public long Id { get; set; }
        public required string UserId { get; set; }
        public User? User { get; set; }
        public virtual ICollection<StudentTeacher> StudentTeachers { get; set; } = new List<StudentTeacher>();
    }
}
