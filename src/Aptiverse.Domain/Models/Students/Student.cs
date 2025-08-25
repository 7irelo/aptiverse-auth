using Aptiverse.Domain.Models.Junctions;
using Aptiverse.Domain.Models.Users;

namespace Aptiverse.Domain.Models.Students
{
    public class Student
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public virtual ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
        public virtual ICollection<StudentTeacher> StudentTeachers { get; set; } = new List<StudentTeacher>();
        public virtual ICollection<StudentAdmin> StudentAdmins { get; set; } = new List<StudentAdmin>();
    }
}
