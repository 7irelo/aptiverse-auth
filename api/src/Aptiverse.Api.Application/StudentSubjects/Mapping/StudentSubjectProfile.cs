using Aptiverse.Api.Application.StudentSubjects.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using AutoMapper;

namespace Aptiverse.Api.Application.StudentSubjects.Mapping
{
    public class StudentSubjectProfile : Profile
    {
        public StudentSubjectProfile()
        {
            CreateMap<StudentSubject, StudentSubjectDto>()
                .ForMember(dest => dest.SubjectName,
                    opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Name : null))
                .ForMember(dest => dest.SubjectCode,
                    opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Code : null))
                .ForMember(dest => dest.SubjectDescription,
                    opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Description : null))
                .ForMember(dest => dest.SubjectColor,
                    opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Color : null))
                .ForMember(dest => dest.SubjectTextColor,
                    opt => opt.MapFrom(src => src.Subject != null ? src.Subject.TextColor : null))
                .ForMember(dest => dest.SubjectBorderColor,
                    opt => opt.MapFrom(src => src.Subject != null ? src.Subject.BorderColor : null));

            CreateMap<StudentSubjectDto, StudentSubject>()
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.SubjectId));

            CreateMap<StudentSubject, CreateStudentSubjectDto>();
            CreateMap<CreateStudentSubjectDto, StudentSubject>()
                .ForMember(dest => dest.Subject, opt => opt.Ignore());
 
            CreateMap<StudentSubject, UpdateStudentSubjectDto>();

            CreateMap<UpdateStudentSubjectDto, StudentSubject>()
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}