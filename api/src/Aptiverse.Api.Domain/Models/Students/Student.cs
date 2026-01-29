using Aptiverse.Api.Domain.Models.Admins;
using Aptiverse.Api.Domain.Models.Courses;
using Aptiverse.Api.Domain.Models.Parents;
using Aptiverse.Api.Domain.Models.Resources;
using Aptiverse.Api.Domain.Models.Teachers;
using Aptiverse.Api.Domain.Models.Tutors;

namespace Aptiverse.Api.Domain.Models.Students
{
    public class Student
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long? AdminId { get; set; }
        public string? Grade { get; set; }

        public virtual Admin? Admin { get; set; }
        public virtual ICollection<StudentSubject>? StudentSubjects { get; set; }
        public virtual ICollection<StudySession>? StudySessions { get; set; }
        public virtual ICollection<Assessment>? Assessments { get; set; }
        public virtual ICollection<ParentStudent>? ParentStudents { get; set; }
        public virtual ICollection<TeacherStudent>? TeacherStudents { get; set; }
        public virtual ICollection<TutorStudent>? TutorStudents { get; set; }
        public virtual ICollection<CourseEnrollment>? CourseEnrollments { get; set; }
        public virtual ICollection<ResourceDownload>? ResourceDownloads { get; set; }
    }
}
