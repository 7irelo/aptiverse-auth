using Aptiverse.Api.Application.Assessments.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using AutoMapper;

namespace Aptiverse.Api.Application.Assessments.Mapping
{
    public class AssessmentProfile : Profile
    {
        public AssessmentProfile()
        {
            CreateMap<Assessment, AssessmentDto>()
                .ForMember(dest => dest.StudentName,
                    opt => opt.MapFrom(src => src.Student != null ? src.Student.UserId : null))
                .ForMember(dest => dest.SubjectName,
                    opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Name : null))
                .ReverseMap();

            CreateMap<Assessment, CreateAssessmentDto>().ReverseMap();

            CreateMap<Assessment, UpdateAssessmentDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}