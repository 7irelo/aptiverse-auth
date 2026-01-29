using Aptiverse.Api.Application.ImprovementTips.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.ImprovementTips.Services
{
    public interface IImprovementTipService
    {
        Task<ImprovementTipDto> CreateImprovementTipAsync(CreateImprovementTipDto createImprovementTipDto);
        Task<ImprovementTipDto?> GetImprovementTipByIdAsync(long id);
        Task<PaginatedResult<ImprovementTipDto>> GetImprovementTipsAsync(
            long? studentSubjectId = null,
            string? search = null,
            int? minPriority = null,
            int? maxPriority = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<ImprovementTipDto> UpdateImprovementTipAsync(long id, UpdateImprovementTipDto updateImprovementTipDto);
        Task<bool> DeleteImprovementTipAsync(long id);
        Task<int> CountImprovementTipsAsync(long? studentSubjectId = null);
        Task<bool> ImprovementTipExistsAsync(long id);
    }
}