using Aptiverse.Api.Application.CourseEnrollments.Dtos;
using Aptiverse.Api.Domain.Models.Courses;
using AutoMapper;

namespace Aptiverse.Api.Application.CourseEnrollments.Mapping
{
    public class CourseEnrollmentProfile : Profile
    {
        public CourseEnrollmentProfile()
        {
            CreateMap<CourseEnrollment, CourseEnrollmentDto>()
                .ReverseMap();

            CreateMap<CourseEnrollment, CreateCourseEnrollmentDto>()
                .ReverseMap()
                .ForMember(dest => dest.EnrolledAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<CourseEnrollment, UpdateCourseEnrollmentDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}