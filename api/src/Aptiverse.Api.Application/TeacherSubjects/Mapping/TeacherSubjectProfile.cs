using Aptiverse.Api.Application.TeacherSubjects.Dtos;
using Aptiverse.Api.Domain.Models.Teachers;
using AutoMapper;

namespace Aptiverse.Api.Application.TeacherSubjects.Mapping
{
    public class TeacherSubjectProfile : Profile
    {
        public TeacherSubjectProfile()
        {
            CreateMap<TeacherSubject, TeacherSubjectDto>().ReverseMap();
            CreateMap<TeacherSubject, CreateTeacherSubjectDto>().ReverseMap();

            CreateMap<TeacherSubject, UpdateTeacherSubjectDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}