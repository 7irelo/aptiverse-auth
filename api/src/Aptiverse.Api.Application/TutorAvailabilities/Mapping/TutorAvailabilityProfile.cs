using Aptiverse.Api.Application.TutorAvailabilities.Dtos;
using Aptiverse.Api.Domain.Models.Tutors;
using AutoMapper;

namespace Aptiverse.Api.Application.TutorAvailabilities.Mapping
{
    public class TutorAvailabilityProfile : Profile
    {
        public TutorAvailabilityProfile()
        {
            CreateMap<TutorAvailability, TutorAvailabilityDto>().ReverseMap();
            CreateMap<TutorAvailability, CreateTutorAvailabilityDto>().ReverseMap();

            CreateMap<TutorAvailability, UpdateTutorAvailabilityDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}