using Aptiverse.Api.Application.UserFeatures.Dtos;
using Aptiverse.Api.Domain.Models.Features;
using AutoMapper;

namespace Aptiverse.Api.Application.UserFeatures.Mapping
{
    public class UserFeatureProfile : Profile
    {
        public UserFeatureProfile()
        {
            CreateMap<UserFeature, UserFeatureDto>()
                .ReverseMap();

            CreateMap<UserFeature, CreateUserFeatureDto>()
                .ReverseMap()
                .ForMember(dest => dest.GrantedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UserFeature, UpdateUserFeatureDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}