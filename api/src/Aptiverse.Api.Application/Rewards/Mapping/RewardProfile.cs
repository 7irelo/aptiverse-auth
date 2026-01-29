using Aptiverse.Api.Application.Rewards.Dtos;
using Aptiverse.Api.Domain.Models.Rewards;
using AutoMapper;

namespace Aptiverse.Api.Application.Rewards.Mapping
{
    public class RewardProfile : Profile
    {
        public RewardProfile()
        {
            CreateMap<Reward, RewardDto>().ReverseMap();
            CreateMap<Reward, CreateRewardDto>().ReverseMap();

            CreateMap<Reward, UpdateRewardDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}