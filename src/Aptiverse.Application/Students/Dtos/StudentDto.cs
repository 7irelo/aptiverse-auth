using System.Runtime.Serialization;

namespace Aptiverse.Application.Students.Dtos
{
    public class StudentDto
    {
        [DataMember(Name = "id")] public long Id { get; set; }
        [DataMember(Name = "userId")] public required string UserId { get; set; }
    }
}
