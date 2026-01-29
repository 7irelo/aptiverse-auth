using System;
using System.Collections.Generic;
using System.Text;

namespace Aptiverse.Api.Application.Students.Dtos
{
    public record UpdateStudentDto
    {
        public long? AdminId { get; init; }
        public string Grade { get; init; }
    }
}
