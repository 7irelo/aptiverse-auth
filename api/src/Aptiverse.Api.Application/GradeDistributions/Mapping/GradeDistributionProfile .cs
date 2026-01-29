using Aptiverse.Api.Application.GradeDistributions.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using AutoMapper;

namespace Aptiverse.Api.Application.GradeDistributions.Mapping
{
    public class GradeDistributionProfile : Profile
    {
        public GradeDistributionProfile()
        {
            CreateMap<GradeDistribution, GradeDistributionDto>().ReverseMap();
            CreateMap<GradeDistribution, CreateGradeDistributionDto>().ReverseMap();

            CreateMap<GradeDistribution, UpdateGradeDistributionDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}