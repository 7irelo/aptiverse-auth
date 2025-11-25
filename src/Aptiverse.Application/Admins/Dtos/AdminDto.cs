using System.Runtime.Serialization;

namespace Aptiverse.Application.Admins.Dtos
{
    [DataContract]
    public class AdminDto
    {
        [DataMember(Name = "id")] public long Id { get; set; }
        [DataMember(Name = "userId")] public string? UserId { get; set; }
    }
}
