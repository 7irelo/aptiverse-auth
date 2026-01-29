namespace Aptiverse.Domain.Models
{
    public class Student
    {
        public long Id { get; set; }
        public required string UserId { get; set; }
        public User? User { get; set; }
        public virtual ICollection<StudentParent> StudentParents { get; set; } = [];
        public virtual ICollection<StudentTeacher> StudentTeachers { get; set; } = [];
        public virtual ICollection<StudentAdmin> StudentAdmins { get; set; } = [];
    }
}
