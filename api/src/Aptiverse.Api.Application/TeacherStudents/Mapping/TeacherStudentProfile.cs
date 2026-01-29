using Aptiverse.Api.Application.TeacherStudents.Dtos;
using Aptiverse.Api.Domain.Models.Teachers;
using AutoMapper;

namespace Aptiverse.Api.Application.TeacherStudents.Mapping
{
    public class TeacherStudentProfile : Profile
    {
        public TeacherStudentProfile()
        {
            CreateMap<TeacherStudent, TeacherStudentDto>().ReverseMap();
            CreateMap<TeacherStudent, CreateTeacherStudentDto>().ReverseMap();

            CreateMap<TeacherStudent, UpdateTeacherStudentDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}