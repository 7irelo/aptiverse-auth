using Aptiverse.Api.Application.DiaryEntries.Dtos;
using Aptiverse.Api.Domain.Models.Psychology;
using AutoMapper;

namespace Aptiverse.Api.Application.DiaryEntries.Mapping
{
    public class DiaryEntryProfile : Profile
    {
        public DiaryEntryProfile()
        {
            CreateMap<DiaryEntry, DiaryEntryDto>()
                .ReverseMap();

            CreateMap<DiaryEntry, CreateDiaryEntryDto>()
                .ReverseMap()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<DiaryEntry, UpdateDiaryEntryDto>()
                .ReverseMap()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}