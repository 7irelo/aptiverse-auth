using Aptiverse.Api.Application.WeeklyStudyHours.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using AutoMapper;

namespace Aptiverse.Api.Application.WeeklyStudyHours.Mapping
{
    public class WeeklyStudyHourProfile : Profile
    {
        public WeeklyStudyHourProfile()
        {
            CreateMap<WeeklyStudyHour, WeeklyStudyHourDto>().ReverseMap();
            CreateMap<WeeklyStudyHour, CreateWeeklyStudyHourDto>().ReverseMap();

            CreateMap<WeeklyStudyHour, UpdateWeeklyStudyHourDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}