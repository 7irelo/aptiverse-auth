using Aptiverse.Application.Admins.Dtos;

namespace Aptiverse.Application.Admins.Services
{
    public interface IAdminService
    {
        Task<AdminDto> CreateAdminAsync(AdminDto adminDto);
        Task<AdminDto> GetOneAdminAsync(long id);
        Task<IEnumerable<AdminDto>> GetManyAdminsAsync(string filter);
        Task<AdminDto> UpdateAdminAsync(long id, AdminDto adminDto);
        Task DeleteAdminAsync(long id);
    }
}
