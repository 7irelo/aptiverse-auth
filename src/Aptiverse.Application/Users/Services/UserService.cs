using Aptiverse.Application.Users.Dtos;
using Aptiverse.Domain.Models.Users;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Aptiverse.Application.Users.Services
{
    public class UserService(UserManager<ApplicationUser> userManager, IMapper mapper) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;

        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            var user = new ApplicationUser
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (!result.Succeeded)
            {
                throw new Exception($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetOneUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }
            return _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetManyUsersAsync(string filter)
        {
            try
            {
                var filters = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
                var allUsers = _userManager.Users.AsEnumerable();

                foreach (var filterItem in filters)
                {
                    switch (filterItem.Key.ToLower())
                    {
                        case "email":
                            allUsers = allUsers.Where(u => u.Email.Contains(filterItem.Value.ToString()));
                            break;
                        case "username":
                            allUsers = allUsers.Where(u => u.UserName.Contains(filterItem.Value.ToString()));
                            break;
                        case "phonenumber":
                            allUsers = allUsers.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(filterItem.Value.ToString()));
                            break;
                    }
                }

                return _mapper.Map<IEnumerable<UserDto>>(allUsers);
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("Invalid filter format", nameof(filter), ex);
            }
        }

        public async Task<UserDto> UpdateUserAsync(string id, UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            user.UserName = userDto.Email;
            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception($"User update failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception($"User deletion failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}