using System.Runtime.Serialization;

namespace Aptiverse.Application.Teachers.Dtos
{
    [DataContract]
    public class TeacherDto
    {
        [DataMember(Name = "id")] public long Id { get; set; }
        [DataMember(Name = "userId")] public string UserId { get; set; }
    }
}
