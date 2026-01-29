using Aptiverse.Api.Application.PredictionMetricss.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using AutoMapper;

namespace Aptiverse.Api.Application.PredictionMetricss.Mapping
{
    public class PredictionMetricsProfile : Profile
    {
        public PredictionMetricsProfile()
        {
            CreateMap<PredictionMetrics, PredictionMetricsDto>().ReverseMap();
            CreateMap<PredictionMetrics, CreatePredictionMetricsDto>().ReverseMap();

            CreateMap<PredictionMetrics, UpdatePredictionMetricsDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}