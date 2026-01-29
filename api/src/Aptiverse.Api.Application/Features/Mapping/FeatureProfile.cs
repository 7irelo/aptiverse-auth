using Aptiverse.Api.Application.Features.Dtos;
using Aptiverse.Api.Domain.Models.Features;
using AutoMapper;

namespace Aptiverse.Api.Application.Features.Mapping
{
    public class FeatureProfile : Profile
    {
        public FeatureProfile()
        {
            CreateMap<Feature, FeatureDto>()
                .ReverseMap();

            CreateMap<Feature, CreateFeatureDto>()
                .ReverseMap()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Feature, UpdateFeatureDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}