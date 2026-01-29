using Aptiverse.Api.Application.CourseModules.Dtos;
using Aptiverse.Api.Domain.Models.Courses;
using AutoMapper;

namespace Aptiverse.Api.Application.CourseModules.Mapping
{
    public class CourseModuleProfile : Profile
    {
        public CourseModuleProfile()
        {
            CreateMap<CourseModule, CourseModuleDto>()
                .ReverseMap();

            CreateMap<CourseModule, CreateCourseModuleDto>()
                .ReverseMap();

            CreateMap<CourseModule, UpdateCourseModuleDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}