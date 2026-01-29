using Aptiverse.Api.Application.ImprovementTips.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using AutoMapper;

namespace Aptiverse.Api.Application.ImprovementTips.Mapping
{
    public class ImprovementTipProfile : Profile
    {
        public ImprovementTipProfile()
        {
            CreateMap<ImprovementTip, ImprovementTipDto>().ReverseMap();
            CreateMap<ImprovementTip, CreateImprovementTipDto>().ReverseMap();

            CreateMap<ImprovementTip, UpdateImprovementTipDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}