using Aptiverse.Api.Application.RewardFeatures.Dtos;
using Aptiverse.Api.Domain.Models.Features;
using AutoMapper;

namespace Aptiverse.Api.Application.RewardFeatures.Mapping
{
    public class RewardFeatureProfile : Profile
    {
        public RewardFeatureProfile()
        {
            CreateMap<RewardFeature, RewardFeatureDto>()
                .ReverseMap();

            CreateMap<RewardFeature, CreateRewardFeatureDto>()
                .ReverseMap();

            CreateMap<RewardFeature, UpdateRewardFeatureDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}