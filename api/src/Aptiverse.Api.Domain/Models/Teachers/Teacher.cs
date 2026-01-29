using Aptiverse.Api.Domain.Models.Courses;
using Aptiverse.Api.Domain.Models.Resources;

namespace Aptiverse.Api.Domain.Models.Teachers
{
    public class Teacher
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string? Qualification { get; set; }
        public string? Specialization { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Bio { get; set; }
        public decimal? HourlyRate { get; set; }
        public bool? IsVerified { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<TeacherSubject>? TeacherSubjects { get; set; }
        public virtual ICollection<TeacherStudent>? TeacherStudents { get; set; }
        public virtual ICollection<Course>? Courses { get; set; }
        public virtual ICollection<Resource>? Resources { get; set; }
    }
}
