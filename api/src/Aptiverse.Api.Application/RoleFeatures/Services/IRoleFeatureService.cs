using Aptiverse.Api.Application.RoleFeatures.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.RoleFeatures.Services
{
    public interface IRoleFeatureService
    {
        Task<RoleFeatureDto> CreateRoleFeatureAsync(CreateRoleFeatureDto createRoleFeatureDto);
        Task<RoleFeatureDto?> GetRoleFeatureByIdAsync(long id);

        Task<PaginatedResult<RoleFeatureDto>> GetRoleFeaturesAsync(
            ClaimsPrincipal currentUser,
            string? roleName = null,
            long? featureId = null,
            bool? isDefault = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);

        Task<RoleFeatureDto> UpdateRoleFeatureAsync(long id, UpdateRoleFeatureDto updateRoleFeatureDto);
        Task<bool> DeleteRoleFeatureAsync(long id);
        Task<int> CountRoleFeaturesAsync(ClaimsPrincipal currentUser,
            string? roleName = null,
            long? featureId = null,
            bool? isDefault = null);
        Task<bool> RoleFeatureExistsAsync(long id);

        Task<IEnumerable<RoleFeatureDto>> GetRoleFeaturesByRoleAsync(string roleName);
        Task<IEnumerable<RoleFeatureDto>> GetRoleFeaturesByFeatureAsync(long featureId);
        Task<IEnumerable<RoleFeatureDto>> GetDefaultFeaturesForRoleAsync(string roleName);
        Task<bool> HasFeatureAccessAsync(string roleName, long featureId);
        Task<bool> ExistsAsync(string roleName, long featureId);
    }
}