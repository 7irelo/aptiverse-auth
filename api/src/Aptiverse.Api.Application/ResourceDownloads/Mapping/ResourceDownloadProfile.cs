using Aptiverse.Api.Application.ResourceDownloads.Dtos;
using Aptiverse.Api.Domain.Models.Resources;
using AutoMapper;

namespace Aptiverse.Api.Application.ResourceDownloads.Mapping
{
    public class ResourceDownloadProfile : Profile
    {
        public ResourceDownloadProfile()
        {
            CreateMap<ResourceDownload, ResourceDownloadDto>()
                .ReverseMap();

            CreateMap<ResourceDownload, CreateResourceDownloadDto>()
                .ReverseMap()
                .ForMember(dest => dest.DownloadedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}