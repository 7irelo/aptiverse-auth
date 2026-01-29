using Aptiverse.Api.Domain.Models.Students;

namespace Aptiverse.Api.Domain.Models.Teachers
{
    public class TeacherSubject
    {
        public long Id { get; set; }
        public long TeacherId { get; set; }
        public string SubjectId { get; set; }
        public int ProficiencyLevel { get; set; } // 1-10 scale

        public virtual Teacher Teacher { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
