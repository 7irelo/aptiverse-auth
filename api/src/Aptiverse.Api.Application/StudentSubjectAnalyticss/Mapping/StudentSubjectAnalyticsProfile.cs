using Aptiverse.Api.Application.StudentSubjectAnalyticss.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using AutoMapper;

namespace Aptiverse.Api.Application.StudentSubjectAnalyticss.Mapping
{
    public class StudentSubjectAnalyticsProfile : Profile
    {
        public StudentSubjectAnalyticsProfile()
        {
            CreateMap<StudentSubjectAnalytics, StudentSubjectAnalyticsDto>().ReverseMap();
            CreateMap<StudentSubjectAnalytics, CreateStudentSubjectAnalyticsDto>().ReverseMap();

            CreateMap<StudentSubjectAnalytics, UpdateStudentSubjectAnalyticsDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}