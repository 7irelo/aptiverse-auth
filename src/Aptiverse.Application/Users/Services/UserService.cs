using Aptiverse.Application.Users.Dtos;
using Aptiverse.Domain.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Aptiverse.Application.Users.Services
{
    public class UserService(UserManager<User> userManager, IMapper mapper) : IUserService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IMapper _mapper = mapper;

        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            user.CreatedAt = DateTime.UtcNow;

            var result = await _userManager.CreateAsync(user, userDto.Password ?? string.Empty);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                if (result.Errors.Any(e => e.Code.Contains("Duplicate")))
                {
                    throw new InvalidOperationException($"User already exists: {errors}");
                }
                else if (result.Errors.Any(e => e.Code.Contains("Password")))
                {
                    throw new ArgumentException($"Invalid password: {errors}");
                }
                else
                {
                    throw new InvalidOperationException($"User creation failed: {errors}");
                }
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetOneUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user == null ? throw new KeyNotFoundException($"User with ID {id} not found") : _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetManyUsersAsync(string? email = null, string? username = null, string? firstName = null, string? lastName = null, string? phoneNumber = null)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(u => u.Email != null && u.Email.Contains(email));
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                query = query.Where(u => u.UserName != null && u.UserName.Contains(username));
            }

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                query = query.Where(u => u.FirstName.Contains(firstName));
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                query = query.Where(u => u.LastName.Contains(lastName));
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                query = query.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(phoneNumber));
            }

            var users = await query.ToListAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> UpdateUserAsync(string id, UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(id) ?? throw new KeyNotFoundException($"User with ID {id} not found");

            var dtoProperties = typeof(UserDto).GetProperties();

            foreach (var dtoProperty in dtoProperties)
            {
                var dtoValue = dtoProperty.GetValue(userDto);

                if (dtoValue == null || (dtoValue is string str && string.IsNullOrWhiteSpace(str)))
                    continue;

                var userProperty = typeof(User).GetProperty(dtoProperty.Name);
                if (userProperty != null && userProperty.CanWrite)
                {
                    userProperty.SetValue(user, dtoValue);
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"User update failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id) ?? throw new KeyNotFoundException($"User with ID {id} not found");
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception($"User deletion failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
