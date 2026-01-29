using Aptiverse.Api.Domain.Models.Students;

namespace Aptiverse.Api.Domain.Models.Parents
{
    public class ParentStudent
    {
        public long Id { get; set; }
        public long ParentId { get; set; }
        public long StudentId { get; set; }
        public string Relationship { get; set; } // "Mother", "Father", "Guardian"
        public bool IsPrimaryContact { get; set; }

        public virtual Parent Parent { get; set; }
        public virtual Student Student { get; set; }
    }
}
