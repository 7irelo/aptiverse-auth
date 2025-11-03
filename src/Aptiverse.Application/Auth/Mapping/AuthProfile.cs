using Aptiverse.Application.Auth.Dtos;
using Aptiverse.Domain.Models.Users;
using AutoMapper;

namespace Aptiverse.Application.Auth.Mapping
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<ApplicationUser, RegisterDto>().ReverseMap();
        }
    }
}