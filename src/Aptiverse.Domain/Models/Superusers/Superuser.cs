using Aptiverse.Domain.Models.Users;

namespace Aptiverse.Domain.Models.Superusers
{
    public class Superuser
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
