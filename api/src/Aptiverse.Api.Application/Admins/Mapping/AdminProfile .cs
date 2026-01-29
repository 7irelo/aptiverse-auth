using Aptiverse.Api.Application.Admins.Dtos;
using Aptiverse.Api.Domain.Models.Admins;
using AutoMapper;

namespace Aptiverse.Api.Application.Admins.Mapping
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<Admin, AdminDto>()
                .ForMember(dest => dest.StudentCount,
                    opt => opt.MapFrom(src => src.Students != null ? src.Students.Count : 0))
                .ForMember(dest => dest.TeacherCount,
                    opt => opt.MapFrom(src => src.Teachers != null ? src.Teachers.Count : 0))
                .ReverseMap();

            CreateMap<Admin, CreateAdminDto>().ReverseMap();

            CreateMap<Admin, UpdateAdminDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}