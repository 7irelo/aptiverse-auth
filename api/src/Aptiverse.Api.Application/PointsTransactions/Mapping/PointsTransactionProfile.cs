using Aptiverse.Api.Application.PointsTransactions.Dtos;
using Aptiverse.Api.Domain.Models.Rewards;
using AutoMapper;

namespace Aptiverse.Api.Application.PointsTransactions.Mapping
{
    public class PointsTransactionProfile : Profile
    {
        public PointsTransactionProfile()
        {
            CreateMap<PointsTransaction, PointsTransactionDto>()
                .ReverseMap();

            CreateMap<PointsTransaction, CreatePointsTransactionDto>()
                .ReverseMap()
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<PointsTransaction, UpdatePointsTransactionDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}