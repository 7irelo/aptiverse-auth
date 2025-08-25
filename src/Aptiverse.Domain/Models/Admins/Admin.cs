using Aptiverse.Domain.Models.Junctions;
using Aptiverse.Domain.Models.Users;

namespace Aptiverse.Domain.Models.Admins
{
    public class Admin
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public virtual ICollection<StudentAdmin> StudentAdmins { get; set; } = new List<StudentAdmin>();
    }
}
