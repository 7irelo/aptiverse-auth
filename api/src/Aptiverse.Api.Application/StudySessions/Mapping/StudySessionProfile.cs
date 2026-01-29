using Aptiverse.Api.Application.StudySessions.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using AutoMapper;

namespace Aptiverse.Api.Application.StudySessions.Mapping
{
    public class StudySessionProfile : Profile
    {
        public StudySessionProfile()
        {
            CreateMap<StudySession, StudySessionDto>().ReverseMap();
            CreateMap<StudySession, CreateStudySessionDto>().ReverseMap();

            CreateMap<StudySession, UpdateStudySessionDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}