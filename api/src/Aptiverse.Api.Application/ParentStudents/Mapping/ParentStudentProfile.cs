using Aptiverse.Api.Application.ParentStudents.Dtos;
using Aptiverse.Api.Domain.Models.Parents;
using AutoMapper;

namespace Aptiverse.Api.Application.ParentStudents.Mapping
{
    public class ParentStudentProfile : Profile
    {
        public ParentStudentProfile()
        {
            CreateMap<ParentStudent, ParentStudentDto>()
                .ReverseMap();

            CreateMap<ParentStudent, CreateParentStudentDto>()
                .ReverseMap();

            CreateMap<ParentStudent, UpdateParentStudentDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}