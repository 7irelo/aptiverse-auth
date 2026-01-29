using Aptiverse.Api.Application.Topics.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using AutoMapper;

namespace Aptiverse.Api.Application.Topics.Mapping
{
    public class TopicProfile : Profile
    {
        public TopicProfile()
        {
            CreateMap<Topic, TopicDto>().ReverseMap();
            CreateMap<Topic, CreateTopicDto>().ReverseMap();

            CreateMap<Topic, UpdateTopicDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}