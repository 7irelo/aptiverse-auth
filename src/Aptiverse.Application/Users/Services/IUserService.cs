using Aptiverse.Application.Users.Dtos;

namespace Aptiverse.Application.Users.Services
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(UserDto userDto);
        Task<UserDto> GetOneUserAsync(string id);
        Task<IEnumerable<UserDto>> GetManyUsersAsync(string? email = null, string? username = null, string? firstName = null, string? lastName = null, string? phoneNumber = null);
        Task<UserDto> UpdateUserAsync(string id, UserDto userDto);
        Task DeleteUserAsync(string id);
    }
}
