using Aptiverse.Api.Application.Subjects.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using AutoMapper;

namespace Aptiverse.Api.Application.Subjects.Mapping
{
    public class SubjectProfile : Profile
    {
        public SubjectProfile()
        {
            CreateMap<Subject, SubjectDto>().ReverseMap();
            CreateMap<Subject, CreateSubjectDto>().ReverseMap();

            CreateMap<Subject, UpdateSubjectDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}