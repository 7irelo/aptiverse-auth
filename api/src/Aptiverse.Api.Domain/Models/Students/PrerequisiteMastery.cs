namespace Aptiverse.Api.Domain.Models.Students
{
    public class PrerequisiteMastery
    {
        public long Id { get; set; }
        public long StudentSubjectId { get; set; }
        public string Prerequisite { get; set; }
        public int MasteryLevel { get; set; }

        public virtual StudentSubject StudentSubject { get; set; }
    }
}
