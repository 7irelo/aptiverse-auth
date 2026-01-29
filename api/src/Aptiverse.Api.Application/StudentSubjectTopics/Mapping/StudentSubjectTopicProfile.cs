using Aptiverse.Api.Application.StudentSubjectTopics.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using AutoMapper;

namespace Aptiverse.Api.Application.StudentSubjectTopics.Mapping
{
    public class StudentSubjectTopicProfile : Profile
    {
        public StudentSubjectTopicProfile()
        {
            CreateMap<StudentSubjectTopic, StudentSubjectTopicDto>().ReverseMap();
            CreateMap<StudentSubjectTopic, CreateStudentSubjectTopicDto>().ReverseMap();

            CreateMap<StudentSubjectTopic, UpdateStudentSubjectTopicDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}