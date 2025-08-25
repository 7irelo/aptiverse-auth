using Aptiverse.Domain.Models.Junctions;
using Aptiverse.Domain.Models.Users;

namespace Aptiverse.Domain.Models.Teachers
{
    public class Teacher
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public virtual ICollection<StudentTeacher> StudentTeachers { get; set; } = new List<StudentTeacher>();
    }
}
