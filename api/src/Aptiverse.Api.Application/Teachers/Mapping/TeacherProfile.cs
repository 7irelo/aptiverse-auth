using Aptiverse.Api.Application.Teachers.Dtos;
using Aptiverse.Api.Domain.Models.Teachers;
using AutoMapper;

namespace Aptiverse.Api.Application.Teachers.Mapping
{
    public class TeacherProfile : Profile
    {
        public TeacherProfile()
        {
            CreateMap<Teacher, TeacherDto>().ReverseMap();
            CreateMap<Teacher, CreateTeacherDto>().ReverseMap();

            CreateMap<Teacher, UpdateTeacherDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}