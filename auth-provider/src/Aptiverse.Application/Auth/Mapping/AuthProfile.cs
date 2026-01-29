using Aptiverse.Application.Auth.Dtos;
using Aptiverse.Domain.Models;
using AutoMapper;

namespace Aptiverse.Application.Auth.Mapping
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<User, RegisterDto>().ReverseMap();
        }
    }
}
