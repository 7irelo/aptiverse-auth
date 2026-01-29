using Aptiverse.Api.Application.TeacherAdmins.Dtos;
using Aptiverse.Api.Domain.Models.Teachers;
using AutoMapper;

namespace Aptiverse.Api.Application.TeacherAdmins.Mapping
{
    public class TeacherAdminProfile : Profile
    {
        public TeacherAdminProfile()
        {
            CreateMap<TeacherAdmin, TeacherAdminDto>().ReverseMap();
            CreateMap<TeacherAdmin, CreateTeacherAdminDto>().ReverseMap();

            CreateMap<TeacherAdmin, UpdateTeacherAdminDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}