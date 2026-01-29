using Aptiverse.Api.Application.StudentPointss.Dtos;
using Aptiverse.Api.Domain.Models.Rewards;
using AutoMapper;

namespace Aptiverse.Api.Application.StudentPointss.Mapping
{
    public class StudentPointsProfile : Profile
    {
        public StudentPointsProfile()
        {
            CreateMap<StudentPoints, StudentPointsDto>().ReverseMap();
            CreateMap<StudentPoints, CreateStudentPointsDto>().ReverseMap();

            CreateMap<StudentPoints, UpdateStudentPointsDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}