using Aptiverse.Domain.Models.Junctions;
using Aptiverse.Domain.Models.Users;

namespace Aptiverse.Domain.Models.Parents
{
    public class Parent
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public virtual ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
    }
}
