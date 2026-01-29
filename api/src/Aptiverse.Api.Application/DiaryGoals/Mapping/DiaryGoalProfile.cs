using Aptiverse.Api.Application.DiaryGoals.Dtos;
using Aptiverse.Api.Domain.Models.Psychology;
using AutoMapper;

namespace Aptiverse.Api.Application.DiaryGoals.Mapping
{
    public class DiaryGoalProfile : Profile
    {
        public DiaryGoalProfile()
        {
            CreateMap<DiaryGoal, DiaryGoalDto>()
                .ReverseMap();

            CreateMap<DiaryGoal, CreateDiaryGoalDto>()
                .ReverseMap();

            CreateMap<DiaryGoal, UpdateDiaryGoalDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}