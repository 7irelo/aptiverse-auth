using Aptiverse.Api.Application.KnowledgeGaps.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using AutoMapper;

namespace Aptiverse.Api.Application.KnowledgeGaps.Mapping
{
    public class KnowledgeGapProfile : Profile
    {
        public KnowledgeGapProfile()
        {
            CreateMap<KnowledgeGap, KnowledgeGapDto>().ReverseMap();
            CreateMap<KnowledgeGap, CreateKnowledgeGapDto>().ReverseMap();

            CreateMap<KnowledgeGap, UpdateKnowledgeGapDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}