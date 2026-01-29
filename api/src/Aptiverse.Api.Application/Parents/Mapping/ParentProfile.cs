using Aptiverse.Api.Application.Parents.Dtos;
using Aptiverse.Api.Domain.Models.Parents;
using AutoMapper;

namespace Aptiverse.Api.Application.Parents.Mapping
{
    public class ParentProfile : Profile
    {
        public ParentProfile()
        {
            CreateMap<Parent, ParentDto>()
                .ReverseMap();

            CreateMap<Parent, CreateParentDto>()
                .ReverseMap()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Parent, UpdateParentDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}