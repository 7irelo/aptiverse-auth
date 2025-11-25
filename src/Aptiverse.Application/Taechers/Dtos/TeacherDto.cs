using System.Runtime.Serialization;

namespace Aptiverse.Application.Taechers.Dtos
{
    [DataContract]
    public class TeacherDto
    {
        [DataMember(Name = "id")] public long Id { get; set; }
        [DataMember(Name = "userId")] public required string UserId { get; set; }
    }
}
