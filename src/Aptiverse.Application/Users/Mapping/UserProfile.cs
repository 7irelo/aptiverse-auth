using Aptiverse.Application.Users.Dtos;
using Aptiverse.Domain.Models.Users;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Aptiverse.Application.Users.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            CreateMap<UserDto, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
        }
    }
}