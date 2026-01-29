using Aptiverse.Api.Application.PeerComparisons.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using AutoMapper;

namespace Aptiverse.Api.Application.PeerComparisons.Mapping
{
    public class PeerComparisonProfile : Profile
    {
        public PeerComparisonProfile()
        {
            CreateMap<PeerComparison, PeerComparisonDto>().ReverseMap();
            CreateMap<PeerComparison, CreatePeerComparisonDto>().ReverseMap();

            CreateMap<PeerComparison, UpdatePeerComparisonDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}