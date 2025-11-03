using Aptiverse.Application.Users.Dtos;
using Aptiverse.Domain.Models.Users;
using AutoMapper;

namespace Aptiverse.Application.Users.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserDto>().ReverseMap();
        }
    }
}