using Aptiverse.Api.Application.Courses.Dtos;
using Aptiverse.Api.Domain.Models.Courses;
using AutoMapper;

namespace Aptiverse.Api.Application.Courses.Mapping
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseDto>()
                .ReverseMap();

            CreateMap<Course, CreateCourseDto>()
                .ReverseMap()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Course, UpdateCourseDto>()
                .ReverseMap()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}