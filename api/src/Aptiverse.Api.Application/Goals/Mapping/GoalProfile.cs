using Aptiverse.Api.Application.Goals.Dtos;
using Aptiverse.Api.Domain.Models.Goals;
using AutoMapper;

namespace Aptiverse.Api.Application.Goals.Mapping
{
    public class GoalProfile : Profile
    {
        public GoalProfile()
        {
            CreateMap<Goal, GoalDto>()
                .ReverseMap();

            CreateMap<Goal, CreateGoalDto>()
                .ReverseMap()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Goal, UpdateGoalDto>()
                .ReverseMap()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}