using Aptiverse.Application.Admins.Dtos;

namespace Aptiverse.Application.Admins.Services
{
    public class AdminService : IAdminService
    {
        public Task<AdminDto> CreateAdminAsync(AdminDto adminDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAdminAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AdminDto>> GetManyAdminsAsync(string filter)
        {
            throw new NotImplementedException();
        }

        public Task<AdminDto> GetOneAdminAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<AdminDto> UpdateAdminAsync(long id, AdminDto adminDto)
        {
            throw new NotImplementedException();
        }
    }
}
