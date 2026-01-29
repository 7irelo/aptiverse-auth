using Aptiverse.Api.Application.Students.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using AutoMapper;

namespace Aptiverse.Api.Application.Students.Mapping
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.AdminName,
                    opt => opt.MapFrom(src => src.Admin != null ? src.Admin.SchoolName : null))
                .ReverseMap();

            CreateMap<Student, CreateStudentDto>().ReverseMap();

            CreateMap<Student, UpdateStudentDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}