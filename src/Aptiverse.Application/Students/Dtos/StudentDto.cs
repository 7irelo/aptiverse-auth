using System.Runtime.Serialization;

namespace Aptiverse.Application.Students.Dtos
{
    [DataContract]
    public class StudentDto
    {
        [DataMember(Name = "id")] public long Id { get; set; }
        [DataMember(Name = "userId")] public string UserId { get; set; }
    }
}
