using System.Runtime.Serialization;

namespace Aptiverse.Application.Parents.Dtos
{
    [DataContract]
    public class ParentDto
    {
        [DataMember(Name = "id")] public long Id { get; set; }
        [DataMember(Name = "userId")] public required string UserId { get; set; }
    }
}
