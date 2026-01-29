using Aptiverse.Api.Application.ModuleLessons.Dtos;
using Aptiverse.Api.Domain.Models.Courses;
using AutoMapper;

namespace Aptiverse.Api.Application.ModuleLessons.Mapping
{
    public class ModuleLessonProfile : Profile
    {
        public ModuleLessonProfile()
        {
            CreateMap<ModuleLesson, ModuleLessonDto>()
                .ReverseMap();

            CreateMap<ModuleLesson, CreateModuleLessonDto>()
                .ReverseMap();

            CreateMap<ModuleLesson, UpdateModuleLessonDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}