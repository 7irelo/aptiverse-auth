namespace Aptiverse.Api.Domain.Models.Students
{
    public class ImprovementTip
    {
        public long Id { get; set; }
        public long StudentSubjectId { get; set; }
        public string Tip { get; set; }
        public int Priority { get; set; }

        public virtual StudentSubject StudentSubject { get; set; }
    }
}
