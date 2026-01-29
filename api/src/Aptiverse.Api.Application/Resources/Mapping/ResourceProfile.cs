using Aptiverse.Api.Application.Resources.Dtos;
using Aptiverse.Api.Domain.Models.Resources;
using AutoMapper;

namespace Aptiverse.Api.Application.Resources.Mapping
{
    public class ResourceProfile : Profile
    {
        public ResourceProfile()
        {
            CreateMap<Resource, ResourceDto>()
                .ReverseMap();

            CreateMap<Resource, CreateResourceDto>()
                .ReverseMap()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.DownloadCount, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => 0.0));

            CreateMap<Resource, UpdateResourceDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}