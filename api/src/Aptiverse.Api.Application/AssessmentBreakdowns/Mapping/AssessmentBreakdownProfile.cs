using Aptiverse.Api.Application.AssessmentBreakdowns.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using AutoMapper;

namespace Aptiverse.Api.Application.AssessmentBreakdowns.Mapping
{
    public class AssessmentBreakdownProfile : Profile
    {
        public AssessmentBreakdownProfile()
        {
            CreateMap<AssessmentBreakdown, AssessmentBreakdownDto>()
                .ForMember(dest => dest.StudentName,
                    opt => opt.MapFrom(src => src.StudentSubject != null &&
                                             src.StudentSubject.Student != null ?
                                             src.StudentSubject.Student.UserId : null))
                .ForMember(dest => dest.SubjectName,
                    opt => opt.MapFrom(src => src.StudentSubject != null &&
                                             src.StudentSubject.Subject != null ?
                                             src.StudentSubject.Subject.Name : null))
                .ReverseMap();

            CreateMap<AssessmentBreakdown, CreateAssessmentBreakdownDto>().ReverseMap();

            CreateMap<AssessmentBreakdown, UpdateAssessmentBreakdownDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}