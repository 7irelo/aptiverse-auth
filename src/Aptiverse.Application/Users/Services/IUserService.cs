using Aptiverse.Application.Auth.Dtos;
using Aptiverse.Application.Users.Dtos;

namespace Aptiverse.Application.Users.Services
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(RegisterDto registerDto);
        Task<UserDto> GetOneUserAsync(string id);
        Task<IEnumerable<UserDto>> GetManyUsersAsync(string? email = null, string? username = null, string? firstName = null, string? lastName = null, string? phoneNumber = null);
        Task<UserDto> UpdateUserAsync(string id, UserDto userDto);
        Task DeleteUserAsync(string id);
    }
}
