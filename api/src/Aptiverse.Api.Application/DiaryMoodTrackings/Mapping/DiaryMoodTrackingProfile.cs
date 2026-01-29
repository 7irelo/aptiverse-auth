using Aptiverse.Api.Application.DiaryMoodTrackings.Dtos;
using Aptiverse.Api.Domain.Models.Psychology;
using AutoMapper;

namespace Aptiverse.Api.Application.DiaryMoodTrackings.Mapping
{
    public class DiaryMoodTrackingProfile : Profile
    {
        public DiaryMoodTrackingProfile()
        {
            CreateMap<DiaryMoodTracking, DiaryMoodTrackingDto>()
                .ReverseMap();

            CreateMap<DiaryMoodTracking, CreateDiaryMoodTrackingDto>()
                .ReverseMap();

            CreateMap<DiaryMoodTracking, UpdateDiaryMoodTrackingDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}