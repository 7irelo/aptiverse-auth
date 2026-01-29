using Aptiverse.Api.Application.TutorSubjects.Dtos;
using Aptiverse.Api.Domain.Models.Teachers;
using AutoMapper;

namespace Aptiverse.Api.Application.TutorSubjects.Mapping
{
    public class TutorSubjectProfile : Profile
    {
        public TutorSubjectProfile()
        {
            CreateMap<TutorSubject, TutorSubjectDto>().ReverseMap();
            CreateMap<TutorSubject, CreateTutorSubjectDto>().ReverseMap();

            CreateMap<TutorSubject, UpdateTutorSubjectDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}