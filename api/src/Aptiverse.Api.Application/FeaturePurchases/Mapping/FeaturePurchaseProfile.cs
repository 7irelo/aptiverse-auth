using Aptiverse.Api.Application.FeaturePurchases.Dtos;
using Aptiverse.Api.Domain.Models.Features;
using AutoMapper;

namespace Aptiverse.Api.Application.FeaturePurchases.Mapping
{
    public class FeaturePurchaseProfile : Profile
    {
        public FeaturePurchaseProfile()
        {
            CreateMap<FeaturePurchase, FeaturePurchaseDto>()
                .ReverseMap();

            CreateMap<FeaturePurchase, CreateFeaturePurchaseDto>()
                .ReverseMap()
                .ForMember(dest => dest.PurchaseDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<FeaturePurchase, UpdateFeaturePurchaseDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}