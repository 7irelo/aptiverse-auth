using Aptiverse.Api.Application.RoleFeatures.Dtos;
using Aptiverse.Api.Domain.Models.Features;
using AutoMapper;

namespace Aptiverse.Api.Application.RoleFeatures.Mapping
{
    public class RoleFeatureProfile : Profile
    {
        public RoleFeatureProfile()
        {
            CreateMap<RoleFeature, RoleFeatureDto>()
                .ReverseMap();

            CreateMap<RoleFeature, CreateRoleFeatureDto>()
                .ReverseMap()
                .ForMember(dest => dest.AssignedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<RoleFeature, UpdateRoleFeatureDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}