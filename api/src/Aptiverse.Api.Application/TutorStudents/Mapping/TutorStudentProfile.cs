using Aptiverse.Api.Application.TutorStudents.Dtos;
using Aptiverse.Api.Domain.Models.Tutors;
using AutoMapper;

namespace Aptiverse.Api.Application.TutorStudents.Mapping
{
    public class TutorStudentProfile : Profile
    {
        public TutorStudentProfile()
        {
            CreateMap<TutorStudent, TutorStudentDto>().ReverseMap();
            CreateMap<TutorStudent, CreateTutorStudentDto>().ReverseMap();

            CreateMap<TutorStudent, UpdateTutorStudentDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}