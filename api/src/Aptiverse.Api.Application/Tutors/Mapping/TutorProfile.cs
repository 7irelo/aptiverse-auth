using Aptiverse.Api.Application.Tutors.Dtos;
using Aptiverse.Api.Domain.Models.Tutors;
using AutoMapper;

namespace Aptiverse.Api.Application.Tutors.Mapping
{
    public class TutorProfile : Profile
    {
        public TutorProfile()
        {
            CreateMap<Tutor, TutorDto>().ReverseMap();
            CreateMap<Tutor, CreateTutorDto>().ReverseMap();

            CreateMap<Tutor, UpdateTutorDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}