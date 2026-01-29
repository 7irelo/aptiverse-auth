using Aptiverse.Api.Application.AdminStudents.Dtos;
using Aptiverse.Api.Domain.Models.Admins;
using AutoMapper;

namespace Aptiverse.Api.Application.AdminStudents.Mapping
{
    public class AdminStudentProfile : Profile
    {
        public AdminStudentProfile()
        {
            CreateMap<AdminStudent, AdminStudentDto>()
                .ForMember(dest => dest.SchoolName,
                    opt => opt.MapFrom(src => src.SchoolAdmin != null ? src.SchoolAdmin.SchoolName : null))
                .ForMember(dest => dest.StudentName,
                    opt => opt.MapFrom(src => src.Student != null ? src.Student.UserId : null))
                .ForMember(dest => dest.StudentGrade,
                    opt => opt.MapFrom(src => src.Student != null ? src.Student.Grade : null))
                .ReverseMap();

            CreateMap<AdminStudent, CreateAdminStudentDto>().ReverseMap();

            CreateMap<AdminStudent, UpdateAdminStudentDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}