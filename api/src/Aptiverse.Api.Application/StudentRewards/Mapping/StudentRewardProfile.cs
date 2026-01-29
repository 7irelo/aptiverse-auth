using Aptiverse.Api.Application.StudentRewards.Dtos;
using Aptiverse.Api.Domain.Models.Rewards;
using AutoMapper;

namespace Aptiverse.Api.Application.StudentRewards.Mapping
{
    public class StudentRewardProfile : Profile
    {
        public StudentRewardProfile()
        {
            CreateMap<StudentReward, StudentRewardDto>().ReverseMap();
            CreateMap<StudentReward, CreateStudentRewardDto>().ReverseMap();

            CreateMap<StudentReward, UpdateStudentRewardDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}